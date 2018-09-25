using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Sevakon.Tests
{
    internal class MenuTests : BaseTests
    {
        /// <summary>
        /// Tests if the start button loads the game scene correctly
        /// </summary>
        [UnityTest]
        public IEnumerator MenuStartButtonTest()
        {
            //Load menu
            yield return LoadMenuScene();
        
            //Simulate click on button
            var startText = GameObject.Find("StartText");
            startText.SendMessage("OnMouseDown");

            //Wait and see if game scene was loaded
            yield return null; 
            Assert.AreEqual(SceneManager.GetActiveScene().name, "Gameplay", "Gameplay scene was not loaded");
        }
    } 
}
