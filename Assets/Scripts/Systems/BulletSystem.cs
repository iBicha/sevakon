using System.Collections.Generic;
using Sevakon.Components;
using Sevakon.Managers;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sevakon.Systems
{
    /// <summary>
    /// Bullet system to control bullet spawning and life cycle
    /// </summary>
    public class BulletSystem : BaseComponentSystem
    {
        private Stack<Bullet> bulletsToDestroy = new Stack<Bullet>();
        private BulletSoundSystem bulletSoundSystem;
        private Camera mainCamera;

        private struct Data
        {
            public Bullet bullet;
        }

        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            bulletSoundSystem = World.Active.GetOrCreateManager<BulletSoundSystem>();
        }
        
        /// <summary>
        /// Initializes the component system with a game manager
        /// </summary>
        /// <param name="gameManager"></param>
        public override void SetComponentData(GameManager gameManager)
        {
            base.SetComponentData(gameManager);
            mainCamera = Camera.main;
        }

        protected override void OnUpdate()
        {
            //TODO: null check not really needed
            if(gameManager == null)
                return;

            var settings = gameManager.bulletSettings;

            var time = Time.time;
            foreach (var entity in GetEntities<Data>())
            {
                var bullet = entity.bullet;
                var progress =
                    Mathf.Clamp01((time - bullet.spawnTime) / settings.animationDuration - settings.animationDelay);
                var scale = settings.curve.Evaluate(progress);
                bullet.transform.localScale = settings.bulletPrefab.transform.localScale * scale;

                if (Mathf.Approximately(progress, 1f))
                {
                    bulletsToDestroy.Push(bullet);
                }
            }

            while (bulletsToDestroy.Count > 0)
            {
                Object.Destroy(bulletsToDestroy.Pop().gameObject);
            }
        }

        /// <summary>
        /// Spawns a bullet instance and apply force to it
        /// </summary>
        public void Spawn()
        {
            if(gameManager == null)
                return;

            var settings = gameManager.bulletSettings;

            Vector2 midScreen = Vector2.one * 0.5f;

            var position = mainCamera.ViewportPointToRay(midScreen).GetPoint(0.3f);

            var bulletInstance = Object.Instantiate(gameManager.bulletSettings.bulletPrefab);
            var bullet = bulletInstance.GetComponent<Bullet>();
            bulletInstance.transform.position = position;
            bulletInstance.transform.rotation = Quaternion.identity;

            bullet.spawnTime = Time.time;

            var rigidBody = bullet.GetComponent<Rigidbody>();
            rigidBody.AddForce(mainCamera.transform.forward * settings.shootForce);

            bulletSoundSystem.PlaySound();
        }
    }
}