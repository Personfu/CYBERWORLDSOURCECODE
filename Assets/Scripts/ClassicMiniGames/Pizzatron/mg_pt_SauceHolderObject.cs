using System.Collections;
using UnityEngine;

namespace Pizzatron
{
    public class mg_pt_SauceHolderObject : mg_pt_ToppingHolderObject
    {
        private static string ANIM_TRIGGER_GRABBED = "OnGrabbed";

        private Animator m_animator;
        private Transform spriteTransform;
        private Vector3 originalSpriteLocalPos;

        private Coroutine resetCoroutine;
        private Coroutine holdPositionCoroutine;

        public override bool IsSauce => true;

        public override void Initialize(GameObject p_resource, mg_pt_EToppingType p_toppingType, string p_grabbedTagSFX, string p_heldedTagSFX)
        {
            base.Initialize(p_resource, p_toppingType, p_grabbedTagSFX, p_heldedTagSFX);

            m_animator = GetComponentInChildren<Animator>();
            spriteTransform = m_animator.transform;
            originalSpriteLocalPos = spriteTransform.localPosition;
        }

        public override void OnGrabbed()
        {
            base.OnGrabbed();

            m_animator.SetTrigger(ANIM_TRIGGER_GRABBED);

            if (resetCoroutine != null)
            {
                StopCoroutine(resetCoroutine);
            }
            resetCoroutine = StartCoroutine(ResetPositionAfterDelay(0.21f));

            if (holdPositionCoroutine != null)
            {
                StopCoroutine(holdPositionCoroutine);
            }
            holdPositionCoroutine = StartCoroutine(HoldSpritePositionCoroutine(0.5f));
        }

        private IEnumerator ResetPositionAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (spriteTransform != null)
            {
                spriteTransform.localPosition = originalSpriteLocalPos;
            }

            // Reset parent position too, just in case
            transform.localPosition = new Vector3(originalLocalPos.x, originalLocalPos.y, transform.localPosition.z);
        }

        private IEnumerator HoldSpritePositionCoroutine(float duration)
        {
            float timer = 0f;
            while (timer < duration)
            {
                if (spriteTransform != null)
                {
                    spriteTransform.localPosition = originalSpriteLocalPos;
                }
                timer += Time.deltaTime;
                yield return null;
            }
            holdPositionCoroutine = null;
        }

        // Adjust access to m_originalPos (make it protected in base class for direct access)
        protected Vector2 originalLocalPos
        {
            get
            {
                // Assuming m_originalPos is protected in base class
                var field = typeof(mg_pt_ToppingHolderObject).GetField("m_originalPos", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    return (Vector2)field.GetValue(this);
                }
                return Vector2.zero;
            }
        }
    }
}