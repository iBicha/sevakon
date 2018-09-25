using Sevakon.Managers;
using UnityEngine;

namespace Sevakon.Behaviours
{
    /// <summary>
    /// Represents a target that the player can shoot at
    /// </summary>
    public class Target : MonoBehaviour
    {
        /// <summary>
        /// Number of hits the target can take before getting destroyed 
        /// </summary>
        public int hitCount = 5;

        /// <summary>
        /// Pitch of the sound when hit
        /// </summary>
        public float pitchStart = 0.7f;
        
        /// <summary>
        /// Pitch of the sound increase after each hit
        /// </summary>
        public float pitchStep = 0.5f;

        private AudioSource audioSource;
        private GameManager gameManager;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.pitch = pitchStart;
        
            gameManager = FindObjectOfType<GameManager>();
            gameManager.IncreaseTargetCount();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Bullet"))
            {
                ApplyHit();
            }
        }

        private void ApplyHit()
        {
            if(hitCount <= 0)
                return;
        
            gameManager.IncreaseScore();

            hitCount--;
      
            audioSource.pitch += pitchStep;
            audioSource.PlayOneShot(audioSource.clip);

            if (hitCount == 0)
            {
                Destroy(gameObject, 0.5f);
            }
        }

        private void OnDestroy()
        {
            if(gameManager != null)
                gameManager.DecreaseTargetCount();
        }
    }

}
