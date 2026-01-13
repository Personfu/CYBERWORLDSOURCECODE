using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class AnimationClipSpriteReplacer : EditorWindow
{
    AnimationClip animClip;
    Vector2 scrollPos;

    class SpriteReplacement
    {
        public Sprite originalSprite;
        public Sprite replacementSprite;
        public List<KeyframeRef> keyframes = new List<KeyframeRef>();
    }
    class KeyframeRef
    {
        public EditorCurveBinding binding;
        public int keyIndex;
    }

    List<SpriteReplacement> spriteReplacements = new List<SpriteReplacement>();

    [MenuItem("Project/Editor/Animation Clip Sprite Replacer")]
    public static void ShowWindow()
    {
        GetWindow<AnimationClipSpriteReplacer>("Animation Clip Sprite Replacer");
    }

    void OnGUI()
    {
        EditorGUILayout.Space();
        animClip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip", animClip, typeof(AnimationClip), false);

        if (animClip != null)
        {
            if (GUILayout.Button("Scan Animation Clip for Sprites"))
            {
                ScanSprites();
            }

            if (spriteReplacements.Count > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Sprites Used in Animation:", EditorStyles.boldLabel);

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));
                for (int i = 0; i < spriteReplacements.Count; i++)
                {
                    var rep = spriteReplacements[i];
                    EditorGUILayout.BeginHorizontal("box");
                    GUILayout.Label(rep.originalSprite ? AssetPreview.GetAssetPreview(rep.originalSprite) : null, GUILayout.Width(40), GUILayout.Height(40));
                    EditorGUILayout.ObjectField("Original", rep.originalSprite, typeof(Sprite), false, GUILayout.Width(120));
                    // The replacement field auto-selects the original sprite by default
                    rep.replacementSprite = (Sprite)EditorGUILayout.ObjectField(rep.replacementSprite, typeof(Sprite), false, GUILayout.Width(120));
                    EditorGUILayout.LabelField($"{rep.keyframes.Count} uses", GUILayout.Width(60));
                    if (GUILayout.Button("Assign", GUILayout.Width(80)))
                    {
                        ApplySingleReplacement(rep);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();

                if (GUILayout.Button("Replace All Sprites"))
                {
                    ReplaceSprites();
                }
            }
        }
    }

    void ScanSprites()
    {
        spriteReplacements.Clear();
        if (animClip == null)
            return;

        var bindings = AnimationUtility.GetObjectReferenceCurveBindings(animClip);
        var spriteToKeyframes = new Dictionary<Sprite, SpriteReplacement>();

        foreach (var binding in bindings)
        {
            var keyframes = AnimationUtility.GetObjectReferenceCurve(animClip, binding);
            for (int i = 0; i < keyframes.Length; i++)
            {
                var sprite = keyframes[i].value as Sprite;
                if (sprite != null)
                {
                    if (!spriteToKeyframes.TryGetValue(sprite, out var rep))
                    {
                        // replacementSprite defaults to the original sprite for convenience
                        rep = new SpriteReplacement { originalSprite = sprite, replacementSprite = sprite };
                        spriteToKeyframes[sprite] = rep;
                    }
                    rep.keyframes.Add(new KeyframeRef { binding = binding, keyIndex = i });
                }
            }
        }
        spriteReplacements = spriteToKeyframes.Values.ToList();
    }

    void ApplySingleReplacement(SpriteReplacement rep)
    {
        if (animClip == null || rep.replacementSprite == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a replacement sprite before assigning.", "OK");
            return;
        }

        Undo.RecordObject(animClip, "Replace Sprite In Animation Clip");

        var bindingToKeyframes = new Dictionary<EditorCurveBinding, ObjectReferenceKeyframe[]>();

        foreach (var kfRef in rep.keyframes)
        {
            if (!bindingToKeyframes.ContainsKey(kfRef.binding))
                bindingToKeyframes[kfRef.binding] = AnimationUtility.GetObjectReferenceCurve(animClip, kfRef.binding);
        }

        foreach (var kfRef in rep.keyframes)
        {
            var kfs = bindingToKeyframes[kfRef.binding];
            if (kfs[kfRef.keyIndex].value == rep.originalSprite)
            {
                kfs[kfRef.keyIndex].value = rep.replacementSprite;
            }
        }

        foreach (var kv in bindingToKeyframes)
        {
            AnimationUtility.SetObjectReferenceCurve(animClip, kv.Key, kv.Value);
        }

        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Done", $"Sprite '{rep.originalSprite?.name}' replaced!", "OK");
        ScanSprites(); // Refresh list after replacement
    }

    void ReplaceSprites()
    {
        if (animClip == null)
            return;

        Undo.RecordObject(animClip, "Replace Sprites In Animation Clip");

        var bindingToKeyframes = new Dictionary<EditorCurveBinding, ObjectReferenceKeyframe[]>();

        foreach (var rep in spriteReplacements)
        {
            foreach (var kfRef in rep.keyframes)
            {
                if (!bindingToKeyframes.ContainsKey(kfRef.binding))
                    bindingToKeyframes[kfRef.binding] = AnimationUtility.GetObjectReferenceCurve(animClip, kfRef.binding);
            }
        }

        foreach (var rep in spriteReplacements)
        {
            if (rep.replacementSprite == null)
                continue;
            foreach (var kfRef in rep.keyframes)
            {
                var kfs = bindingToKeyframes[kfRef.binding];
                if (kfs[kfRef.keyIndex].value == rep.originalSprite)
                {
                    kfs[kfRef.keyIndex].value = rep.replacementSprite;
                }
            }
        }

        foreach (var kv in bindingToKeyframes)
        {
            AnimationUtility.SetObjectReferenceCurve(animClip, kv.Key, kv.Value);
        }

        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Done", "All selected sprite replacements applied to animation clip!", "OK");
        ScanSprites(); // Refresh list
    }
}