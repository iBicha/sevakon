using System.Collections;
using System.Linq;
using NUnit.Framework;
using Sevakon.Components;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Sevakon.Tests
{
    internal class BulletTests : BaseTests
    {
        /// <summary>
        /// Tests if bullets fire correctly
        /// </summary>
        [UnityTest]
        public IEnumerator BulletShootingTest()
        {
            yield return LoadGameScene();
            yield return SimulateFire();

            //Check if there are bullets in scene
            var bullets = Object.FindObjectsOfType<Bullet>();
            Assert.Positive(bullets.Length, "No bullets fired");

            //Wait for physics to kick in
            yield return new WaitForSeconds(0.1f);

            //Check if bullet is moving
            var rigidBody = bullets.First().GetComponent<Rigidbody>();
            Assert.Positive(rigidBody.velocity.magnitude, "Bullet is not moving (rigidBody.velocity = 0)");
        }

        /// <summary>
        /// Tests if bullet sounds play correctly
        /// </summary>
        [UnityTest]
        public IEnumerator BulletShootingSoundTest()
        {
            yield return LoadGameScene();
            yield return SimulateFire();

            //Check if bullet has sound playing
            var bulletSfx = GameObject.Find("BulletSfx");
            var bulletSfxAudio = bulletSfx.GetComponent<AudioSource>();
            Assert.True(bulletSfxAudio.isPlaying, "Bullet did not play a sound (bulletSfxAudio.isPlaying = false)");
        }

    }
}
