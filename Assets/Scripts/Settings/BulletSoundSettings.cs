using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sevakon.Settings
{
    /// <summary>
    /// Contains bullet sound effects settings
    /// </summary>
    [CreateAssetMenu(fileName = "BulletSoundSettings", menuName = "Game Settings/Bullet Sound Settings")]
    public class BulletSoundSettings : ScriptableObject
    {
        /// <summary>
        /// Variations of the bullet sound
        /// </summary>
        public AudioClip[] audioClips;

        /// <summary>
        /// The range of the change applied to sound pitch
        /// </summary>
        [Range(0f, 0.9f)] public float pitchShiftRange = 0.2f;

        /// <summary>
        /// The step of the change applied to sound pitch
        /// </summary>
        [Range(0f, 0.9f)] public float pitchShiftStep = 0.45f;

        /// <summary>
        /// Whether or not to randomize the pitch
        /// </summary>
        public bool randomizePitchShift;
    }
}