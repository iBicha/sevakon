using System.Collections;
using UnityEngine;

namespace Sevakon.Behaviours
{
    /// <summary>
    /// Plays two audio clips in sync, with the ability to fade one of them
    /// </summary>
    public class IntroAudioController : MonoBehaviour
    {
        /// <summary>
        /// Track 1
        /// </summary>
        public AudioSource Intro;
       
        /// <summary>
        /// Track 2
        /// </summary>
        public AudioSource IntroScratch;

        /// <summary>
        /// Fading speed of the second track
        /// </summary>
        public float IntroFadeSpeed = 1f;
    
        private bool isFadingIn;

        private IEnumerator Start()
        {
            IntroScratch.volume = 0f;
            //We wait a frame to ensure initialization happened, and clips are playing (out of sync probably)
            yield return null;
            //Resync
            Intro.time = IntroScratch.time = 0f;
        }

        private void Update()
        {
            var volumeDirection = isFadingIn ? 1f : -1f;
            IntroScratch.volume = Mathf.Clamp01(IntroScratch.volume + Time.deltaTime * volumeDirection * IntroFadeSpeed);

            var volumeDestination = isFadingIn ? 1f : 0f;
            if (Mathf.Approximately(IntroScratch.volume, volumeDestination))
            {
                enabled = false;
            }
        }

        //Start fading IN the second track
        public void FadeInScratch()
        {
            isFadingIn = true;
            enabled = true;
        }

        //Start fading OUT the second track
        public void FadeOutScratch()
        {
            isFadingIn = false;
            enabled = true;
        }
    }
}
