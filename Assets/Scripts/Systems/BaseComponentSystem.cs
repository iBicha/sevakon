using System.Collections;
using System.Collections.Generic;
using Sevakon.Managers;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sevakon.Systems
{
    /// <summary>
    /// Base Component System
    /// </summary>
    public class BaseComponentSystem : ComponentSystem
    {
        protected GameManager gameManager;

        /// <summary>
        /// Initializes the component system with a game manager
        /// </summary>
        /// <param name="gameManager"></param>
        public virtual void SetComponentData(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }
        
        protected override void OnUpdate()
        {
        
        }
    }
}
