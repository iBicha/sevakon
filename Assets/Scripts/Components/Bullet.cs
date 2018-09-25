using UnityEngine;

namespace Sevakon.Components
{
    /// <summary>
    /// Bullet component. Controlled by the BulletSystem
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
    public class Bullet : MonoBehaviour
    {
        /// <summary>
        /// The time of the creation of this bullet instance
        /// </summary>
        public float spawnTime;
    }
}