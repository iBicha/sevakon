using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sevakon.Settings
{
    /// <summary>
    /// Contains bullet settings
    /// </summary>
    [CreateAssetMenu(fileName = "BulletSettings", menuName = "Game Settings/Bullet Settings")]
    public class BulletSettings : ScriptableObject
    {
        /// <summary>
        /// The animation duration of a bullet before it is destroyed
        /// </summary>
        [FormerlySerializedAs("duration")] 
        public float animationDuration = 1;
    
        /// <summary>
        /// Time to wait before the animation starts
        /// </summary>
        [FormerlySerializedAs("offset")] 
        public float animationDelay = 3;
    
        /// <summary>
        /// Animation curve for the bullet
        /// </summary>
        public AnimationCurve curve;
    
        /// <summary>
        /// Bullet prefab to create instances from
        /// </summary>
        public GameObject bulletPrefab;
    
        /// <summary>
        /// Force applied to bullets when shooting
        /// </summary>
        [FormerlySerializedAs("force")] 
        public float shootForce;

    }
}
