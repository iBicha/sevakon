using System.Collections;
using NUnit.Framework;
using Sevakon.Behaviours;
using UnityEngine;
using UnityEngine.TestTools;

namespace Sevakon.Tests
{
    internal class PlayerTests : BaseTests
    {
        /// <summary>
        /// Tests player jumping
        /// </summary>
        [UnityTest]
        public IEnumerator PlayerJumpTest()
        {
            //Load game scene
            yield return LoadGameScene();

            var playerController = Object.FindObjectOfType<PlayerController>();
            var rigidBody = playerController.GetComponent<Rigidbody>();

            //Wait for player to idle (In case he's moving/falling)
            yield return new WaitUntil(() => rigidBody.velocity.magnitude < 0.0001f);

            //Record old player position
            var yPosition = rigidBody.position.y;

            //Jump
            yield return SimulateJump();

            //Wait for physics to kick in
            yield return new WaitForSeconds(0.1f);

            Assert.Greater(rigidBody.position.y - yPosition, 0.01f,
                "Player did not jump (Y position didn't change)");
            Assert.Positive(Vector3.Dot(rigidBody.velocity.normalized, Vector3.up),
                "Player did not jump (velocity is not going up)");
        }
    }
}