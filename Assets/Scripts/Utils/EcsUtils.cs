using System.Collections;
using System.Collections.Generic;
using Sevakon.Managers;
using Sevakon.Systems;
using Unity.Entities;
using UnityEngine;

namespace Sevakon.Utils
{
    /// <summary>
    /// ECS utilities
    /// </summary>
    public class EcsUtils
    {
        /// <summary>
        /// Disable component systems
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void DisableSystems()
        {
            ToggleSystems(false);
        }

        /// <summary>
        /// Enable component systems
        /// </summary>
        /// <param name="gameManager">Game manager to feed to the systems</param>
        public static void EnableSystems(GameManager gameManager)
        {
            ToggleSystems(true, gameManager);
        }

        private static void ToggleSystems(bool enable, GameManager gameManager = null)
        {
            if (World.Active == null)
                return;

            var systemTypes = new[]
            {
                typeof(BulletSystem),
                typeof(BulletSoundSystem)
            };
            
            foreach (var systemType in systemTypes)
            {
                var system = (BaseComponentSystem)World.Active.GetOrCreateManager(systemType);
                system.Enabled = enable;
                system.SetComponentData(gameManager);
            }
        }
    }
}