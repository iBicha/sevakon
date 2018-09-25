using UnityEngine;

namespace Sevakon.Behaviours
{
    /// <summary>
    /// Makes material glow on collision with a bullet.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class GlowOnCollision : MonoBehaviour
    {
        /// <summary>
        /// Curve of the animated glow value
        /// </summary>
        public AnimationCurve glowCurve;
        
        /// <summary>
        /// Animation duration
        /// </summary>
        public float glowDuration = 1f;

        private Color color;
        private Material material;
        private float glowStartTime = -100f;

        private void Awake()
        {
            material = GetComponent<Renderer>().material;
            color = material.GetColor("_EmissionColor");
        }

        private void Update()
        {
            var progress = Mathf.Clamp01((Time.time - glowStartTime) / glowDuration);
            var glowValue = glowCurve.Evaluate(progress);
            var glowColor = color * Mathf.LinearToGammaSpace(glowValue);

            material.SetColor("_EmissionColor", glowColor);
            if (Mathf.Approximately(progress, 1f))
                enabled = false;
        }

        private void Activate()
        {
            glowStartTime = Time.time;
            enabled = true;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Bullet"))
            {
                Activate();
            }
        }
    }
}
