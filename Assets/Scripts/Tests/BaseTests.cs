using System.Collections;
using NUnit.Framework;
using Sevakon.Managers;
using UnityEngine;
using UnityEngine.Experimental.Input;
using UnityEngine.SceneManagement;

namespace Sevakon.Tests
{
    internal abstract class BaseTests
    {
        protected IEnumerator LoadMenuScene()
        {
            yield return LoadScene("Menu");
        }

        protected IEnumerator LoadGameScene()
        {
            yield return LoadScene("Gameplay");
        }

        protected IEnumerator LoadScene(string sceneName)
        {
            var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            yield return new WaitUntil(() => asyncLoad.isDone);
            yield return new WaitForSeconds(0.1f);
        }

        protected IEnumerator SimulateFire()
        {
            //Simulate fire
            var gameManager = FindGameManager();
            gameManager.controls.Gameplay.FirePerformed.Invoke(new InputAction.CallbackContext());
            yield return new WaitForEndOfFrame();
        }

        protected IEnumerator SimulateJump()
        {
            //Simulate jump
            var gameManager = FindGameManager();
            gameManager.controls.Gameplay.JumpPerformed.Invoke(new InputAction.CallbackContext());
            yield return new WaitForEndOfFrame();
        }

        protected GameManager FindGameManager()
        {
            var gameManager = Object.FindObjectOfType<GameManager>();
            Assert.NotNull(gameManager, "GameManager is null");
            return gameManager;
        }
    }
}