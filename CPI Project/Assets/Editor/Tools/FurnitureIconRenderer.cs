using UnityEngine;
using UnityEditor;
using System.IO;

public class FurnitureIconRenderer : EditorWindow
{
    private GameObject prefab;
    private GameObject spawnedInstance;

    private Vector3 position = Vector3.zero;
    private Vector3 rotation = new Vector3(0, 180, 0);
    private Vector3 scale = Vector3.one;

    private float cameraDistance;
    private bool isDragging;
    private bool isRotatingVertical;
    private bool isPanning;
    private bool showGridLines = true;
    private bool snapToGrid = false;
    private float snapIncrement = 0.1f;
    private Vector2 cameraPan = Vector2.zero;

    private Color lightColor = Color.white;
    private float lightIntensity = 1.0f;
    private float lightXRotation = 50f;
    private float lightYRotation = -30f;

    private static readonly int[] iconSizes = { 64, 128, 256, 512, 1024 };
    private static readonly string[] iconSizeLabels = { "64x64", "128x128", "256x256", "512x512", "1024x1024" };
    private int iconSizeIndex = 2;
    private int iconSize => iconSizes[iconSizeIndex];
    private Color backgroundColor = new Color(0.439f, 0.537f, 0.863f, 1f);
    private bool transparentBackground = false;

    private PreviewRenderUtility previewRenderUtility;
    private RenderTexture previewTexture;

    private GameObject previewLight;

    private Vector2 scrollPos;

    [MenuItem("Project/Tools/Furniture Icon Renderer")]
    public static void ShowWindow()
    {
        var window = GetWindow<FurnitureIconRenderer>("Furniture Icon Renderer");
        window.minSize = new Vector2(300, 620);
    }

    private void OnEnable()
    {
        InitPreview();
    }

    private void OnDisable()
    {
        CleanupPreview();
    }

    private void InitPreview()
    {
        CleanupPreview();

        previewRenderUtility = new PreviewRenderUtility();
        previewRenderUtility.camera.fieldOfView = 30f;
        previewRenderUtility.camera.nearClipPlane = 0.01f;
        previewRenderUtility.camera.farClipPlane = 100f;
        previewRenderUtility.camera.transform.position = new Vector3(0, 1, -5);
        previewRenderUtility.camera.transform.LookAt(Vector3.zero);
        previewRenderUtility.camera.clearFlags = CameraClearFlags.SolidColor;
        previewRenderUtility.camera.backgroundColor = backgroundColor;

        previewLight = new GameObject("PreviewLight");
        previewLight.hideFlags = HideFlags.HideAndDontSave;
        var light = previewLight.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = lightColor;
        light.intensity = lightIntensity;
        light.transform.rotation = Quaternion.Euler(lightXRotation, lightYRotation, 0);
        previewRenderUtility.AddSingleGO(previewLight);
    }

    private void CleanupPreview()
    {
        if (spawnedInstance != null)
        {
            DestroyImmediate(spawnedInstance);
            spawnedInstance = null;
        }

        if (previewLight != null)
        {
            DestroyImmediate(previewLight);
            previewLight = null;
        }

        if (previewRenderUtility != null)
        {
            previewRenderUtility.Cleanup();
            previewRenderUtility = null;
        }

        if (previewTexture != null)
        {
            DestroyImmediate(previewTexture);
            previewTexture = null;
        }
    }

    private void SpawnPrefabInPreview()
    {
        if (spawnedInstance != null)
        {
            previewRenderUtility.Cleanup();
            previewRenderUtility = null;
            spawnedInstance = null;
            previewLight = null;
            InitPreview();
        }

        if (prefab == null) return;

        spawnedInstance = Instantiate(prefab);
        spawnedInstance.hideFlags = HideFlags.HideAndDontSave;
        spawnedInstance.transform.position = position;
        spawnedInstance.transform.eulerAngles = rotation;
        spawnedInstance.transform.localScale = scale;

        previewRenderUtility.AddSingleGO(spawnedInstance);

        AutoFrameObject();
    }

    private void AutoFrameObject()
    {
        if (spawnedInstance == null) return;

        var bounds = GetBounds(spawnedInstance);
        float size = bounds.size.magnitude;
        if (size < 0.001f) size = 1f;

        float distance = size / (2f * Mathf.Tan(previewRenderUtility.camera.fieldOfView * 0.5f * Mathf.Deg2Rad));
        distance *= 1.2f;

        cameraDistance = distance;

        var cam = previewRenderUtility.camera;
        cam.transform.position = bounds.center + new Vector3(cameraPan.x, cameraPan.y, -distance);
        cam.transform.LookAt(bounds.center + new Vector3(cameraPan.x, cameraPan.y, 0));
    }

    private Bounds GetBounds(GameObject go)
    {
        var renderers = go.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(go.transform.position, Vector3.one);

        var bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        return bounds;
    }

    private void UpdatePreviewLight()
    {
        if (previewLight == null) return;
        var light = previewLight.GetComponent<Light>();
        if (light == null) return;
        light.color = lightColor;
        light.intensity = lightIntensity;
        light.transform.rotation = Quaternion.Euler(lightXRotation, lightYRotation, 0);
    }

    private void UpdateInstanceTransform()
    {
        if (spawnedInstance == null) return;
        if (snapToGrid)
        {
            position.x = Mathf.Round(position.x / snapIncrement) * snapIncrement;
            position.y = Mathf.Round(position.y / snapIncrement) * snapIncrement;
            position.z = Mathf.Round(position.z / snapIncrement) * snapIncrement;
        }
        spawnedInstance.transform.position = position;
        spawnedInstance.transform.eulerAngles = rotation;
        spawnedInstance.transform.localScale = scale;
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(500));

        GUILayout.FlexibleSpace();

        EditorGUILayout.LabelField("Furniture Icon Renderer", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        EditorGUI.BeginChangeCheck();
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        if (EditorGUI.EndChangeCheck())
        {
            position = Vector3.zero;
            rotation = new Vector3(0, 180, 0);
            scale = Vector3.one;
            SpawnPrefabInPreview();
        }

        EditorGUILayout.Space(5);

        if (prefab != null)
        {
            EditorGUILayout.LabelField("Transform", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            position = EditorGUILayout.Vector3Field("Position", position);
            rotation = EditorGUILayout.Vector3Field("Rotation", rotation);
            scale = EditorGUILayout.Vector3Field("Scale", scale);
            if (EditorGUI.EndChangeCheck())
            {
                UpdateInstanceTransform();
            }

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Directional Light", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            lightColor = EditorGUILayout.ColorField("Light Color", lightColor);
            lightIntensity = EditorGUILayout.Slider("Light Intensity", lightIntensity, 0f, 8f);
            lightXRotation = EditorGUILayout.Slider("Light X Rotation", lightXRotation, -180f, 180f);
            lightYRotation = EditorGUILayout.Slider("Light Y Rotation", lightYRotation, -180f, 180f);
            if (EditorGUI.EndChangeCheck())
            {
                UpdatePreviewLight();
            }

            EditorGUILayout.Space(5);

            iconSizeIndex = EditorGUILayout.Popup("Image Size", iconSizeIndex, iconSizeLabels);

            transparentBackground = EditorGUILayout.Toggle("Transparent Background", transparentBackground);
            if (!transparentBackground)
            {
                EditorGUI.BeginChangeCheck();
                backgroundColor = EditorGUILayout.ColorField("Background Color", backgroundColor);
                if (EditorGUI.EndChangeCheck() && previewRenderUtility != null)
                {
                    previewRenderUtility.camera.backgroundColor = backgroundColor;
                }
            }
            if (previewRenderUtility != null)
            {
                previewRenderUtility.camera.clearFlags = transparentBackground ? CameraClearFlags.SolidColor : CameraClearFlags.SolidColor;
                previewRenderUtility.camera.backgroundColor = transparentBackground ? new Color(0, 0, 0, 0) : backgroundColor;
            }

            EditorGUILayout.Space(10);

            showGridLines = EditorGUILayout.Toggle("Show Grid Lines", showGridLines);

            EditorGUILayout.BeginHorizontal();
            snapToGrid = EditorGUILayout.Toggle("Snap to Grid", snapToGrid);
            if (snapToGrid)
            {
                snapIncrement = EditorGUILayout.FloatField(snapIncrement, GUILayout.Width(50));
                snapIncrement = Mathf.Max(0.01f, snapIncrement);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Live Preview", EditorStyles.boldLabel);

            if (cameraDistance > 0)
            {
                EditorGUI.BeginChangeCheck();
                cameraDistance = EditorGUILayout.Slider("Camera Zoom", cameraDistance, 0.1f, cameraDistance * 5f);
                if (EditorGUI.EndChangeCheck())
                {
                    UpdateCameraPosition();
                }
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            var previewRect = GUILayoutUtility.GetRect(256, 256, GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (spawnedInstance != null && previewRenderUtility != null)
            {
                HandlePreviewInput(previewRect);

                previewRenderUtility.BeginPreview(previewRect, GUIStyle.none);
                previewRenderUtility.camera.backgroundColor = transparentBackground ? new Color(0, 0, 0, 0) : backgroundColor;
                previewRenderUtility.camera.Render();
                var tex = previewRenderUtility.EndPreview();
                GUI.DrawTexture(previewRect, tex, ScaleMode.ScaleToFit);
                if (showGridLines)
                    DrawGridOverlay(previewRect);
            }
            else
            {
                EditorGUI.DrawRect(previewRect, backgroundColor);
            }

            EditorGUILayout.Space(10);

            string prefabPath = AssetDatabase.GetAssetPath(prefab);
            string prefabName = Path.GetFileNameWithoutExtension(prefabPath);
            string saveDir = Path.GetDirectoryName(prefabPath);
            string defaultPath = Path.Combine(saveDir, prefabName + "_FurnitureIcon.png");
            EditorGUILayout.LabelField("Save Path:", defaultPath);

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Center Item", GUILayout.Height(30)))
            {
                position = Vector3.zero;
                UpdateInstanceTransform();
            }
            if (GUILayout.Button("Reset All", GUILayout.Height(30)))
            {
                position = Vector3.zero;
                rotation = new Vector3(0, 180, 0);
                scale = Vector3.one;
                lightColor = Color.white;
                lightIntensity = 1.0f;
                lightXRotation = 50f;
                lightYRotation = -30f;
                backgroundColor = new Color(0.439f, 0.537f, 0.863f, 1f);
                transparentBackground = false;
                showGridLines = true;
                snapToGrid = false;
                snapIncrement = 0.1f;
                cameraPan = Vector2.zero;
                UpdatePreviewLight();
                if (previewRenderUtility != null)
                    previewRenderUtility.camera.backgroundColor = backgroundColor;
                SpawnPrefabInPreview();
            }
            if (GUILayout.Button("Save Icon", GUILayout.Height(30)))
            {
                SaveIcon(defaultPath);
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Save Icon As...", GUILayout.Height(25)))
            {
                string dir = Path.GetDirectoryName(Path.GetFullPath(defaultPath));
                string fileName = prefabName + "_FurnitureIcon.png";
                string chosen = EditorUtility.SaveFilePanel("Save Furniture Icon", dir, fileName, "png");
                if (!string.IsNullOrEmpty(chosen))
                {
                    SaveIconToPath(chosen);
                }
            }
        }

        GUILayout.FlexibleSpace();

        EditorGUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();

        if (spawnedInstance != null)
            Repaint();
    }

    private void HandlePreviewInput(Rect previewRect)
    {
        Event e = Event.current;
        int controlId = GUIUtility.GetControlID(FocusType.Passive);

        switch (e.GetTypeForControl(controlId))
        {
            case EventType.MouseDown:
                if (previewRect.Contains(e.mousePosition) && e.button == 0)
                {
                    isDragging = true;
                    GUIUtility.hotControl = controlId;
                    e.Use();
                }
                else if (previewRect.Contains(e.mousePosition) && e.button == 1)
                {
                    isRotatingVertical = true;
                    GUIUtility.hotControl = controlId;
                    e.Use();
                }
                else if (previewRect.Contains(e.mousePosition) && e.button == 2)
                {
                    isPanning = true;
                    GUIUtility.hotControl = controlId;
                    e.Use();
                }
                break;

            case EventType.MouseDrag:
                if (isDragging)
                {
                    rotation.y -= e.delta.x * 0.5f;
                    UpdateInstanceTransform();
                    e.Use();
                }
                else if (isRotatingVertical)
                {
                    rotation.x -= e.delta.y * 0.5f;
                    UpdateInstanceTransform();
                    e.Use();
                }
                else if (isPanning)
                {
                    cameraPan.x -= e.delta.x * cameraDistance * 0.002f;
                    cameraPan.y += e.delta.y * cameraDistance * 0.002f;
                    if (snapToGrid)
                    {
                        cameraPan.x = Mathf.Round(cameraPan.x / snapIncrement) * snapIncrement;
                        cameraPan.y = Mathf.Round(cameraPan.y / snapIncrement) * snapIncrement;
                    }
                    UpdateCameraPosition();
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                if (isDragging || isRotatingVertical || isPanning)
                {
                    isDragging = false;
                    isRotatingVertical = false;
                    isPanning = false;
                    GUIUtility.hotControl = 0;
                    e.Use();
                }
                break;

            case EventType.ScrollWheel:
                if (previewRect.Contains(e.mousePosition) && cameraDistance > 0)
                {
                    cameraDistance += e.delta.y * cameraDistance * 0.1f;
                    cameraDistance = Mathf.Max(cameraDistance, 0.1f);

                    UpdateCameraPosition();
                    e.Use();
                }
                break;
        }
    }

    private void UpdateCameraPosition()
    {
        if (spawnedInstance == null || previewRenderUtility == null) return;
        var bounds = GetBounds(spawnedInstance);
        var cam = previewRenderUtility.camera;
        cam.transform.position = bounds.center + new Vector3(cameraPan.x, cameraPan.y, -cameraDistance);
        cam.transform.LookAt(bounds.center + new Vector3(cameraPan.x, cameraPan.y, 0));
    }

    private void DrawGridOverlay(Rect rect)
    {
        Color gridColor = new Color(1f, 1f, 1f, 0.3f);
        float cx = rect.x + rect.width * 0.5f;
        float cy = rect.y + rect.height * 0.5f;
        float third = rect.width / 3f;
        float thirdH = rect.height / 3f;

        Handles.BeginGUI();
        Handles.color = gridColor;

        Handles.DrawLine(new Vector3(cx, rect.y), new Vector3(cx, rect.yMax));
        Handles.DrawLine(new Vector3(rect.x, cy), new Vector3(rect.xMax, cy));

        Color subColor = new Color(1f, 1f, 1f, 0.15f);
        Handles.color = subColor;

        Handles.DrawLine(new Vector3(rect.x + third, rect.y), new Vector3(rect.x + third, rect.yMax));
        Handles.DrawLine(new Vector3(rect.x + third * 2f, rect.y), new Vector3(rect.x + third * 2f, rect.yMax));
        Handles.DrawLine(new Vector3(rect.x, rect.y + thirdH), new Vector3(rect.xMax, rect.y + thirdH));
        Handles.DrawLine(new Vector3(rect.x, rect.y + thirdH * 2f), new Vector3(rect.xMax, rect.y + thirdH * 2f));

        Handles.EndGUI();
    }

    private void SaveIcon(string savePath)
    {
        string fullPath = Path.GetFullPath(savePath);
        SaveIconToPath(fullPath);

        AssetDatabase.Refresh();
        var savedAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(savePath);
        if (savedAsset != null)
            EditorGUIUtility.PingObject(savedAsset);
    }

    private void SaveIconToPath(string fullPath)
    {
        if (spawnedInstance == null || previewRenderUtility == null)
        {
            EditorUtility.DisplayDialog("Error", "No prefab is spawned to capture.", "OK");
            return;
        }

        var rt = new RenderTexture(iconSize, iconSize, 24, RenderTextureFormat.ARGB32);
        rt.antiAliasing = 8;

        var cam = previewRenderUtility.camera;
        var prevRT = cam.targetTexture;
        cam.targetTexture = rt;
        cam.backgroundColor = transparentBackground ? new Color(0, 0, 0, 0) : backgroundColor;
        cam.Render();
        cam.targetTexture = prevRT;

        RenderTexture.active = rt;
        var tex2D = new Texture2D(iconSize, iconSize, TextureFormat.RGBA32, false);
        tex2D.ReadPixels(new Rect(0, 0, iconSize, iconSize), 0, 0);
        tex2D.Apply();
        RenderTexture.active = null;

        byte[] pngData = tex2D.EncodeToPNG();
        File.WriteAllBytes(fullPath, pngData);

        DestroyImmediate(tex2D);
        DestroyImmediate(rt);

        Debug.Log("Furniture icon saved to: " + fullPath);
        EditorUtility.DisplayDialog("Success", "Icon saved to:\n" + fullPath, "OK");
    }
}
