using UnityEngine;

namespace Sevakon.Systems
{
    /// <summary>
    /// Component system to control bullet sound effects
    /// </summary>
    public class BulletSoundSystem : BaseComponentSystem
    {
        private int index;
        private float pitchShiftProgress;
    
        protected override void OnUpdate()
        {
        }
    
        /// <summary>
        /// Plays a sound for a bullet
        /// </summary>
        public void PlaySound()
        {
            if(gameManager == null)
                return;
        
            var settings = gameManager.bulletSoundSettings;
            var audioSource = gameManager.bulletSfx;
        
            if (settings.audioClips.Length == 0)
                return;
        
            if (settings.randomizePitchShift)
            {
                index = Random.Range(0,settings.audioClips.Length);
                audioSource.pitch = 1 + Random.Range(-settings.pitchShiftRange / 2, settings.pitchShiftRange / 2);
            }
            else
            {
                pitchShiftProgress += settings.pitchShiftStep;
                index = (index + 1) % settings.audioClips.Length;
                audioSource.pitch = 1 + Mathf.Sin(pitchShiftProgress) * (settings.pitchShiftRange / 2);
            }

            var clip = settings.audioClips[index];
            audioSource.PlayOneShot(clip);
        }

    }
}
