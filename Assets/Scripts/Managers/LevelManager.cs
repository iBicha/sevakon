using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sevakon.Managers
{
    public class LevelManager : MonoBehaviour
    {
        public Object MenuScene;
        public Object GameScene;

        /// <summary>
        /// Load game scene
        /// </summary>
        public void LoadGameScene()
        {
            SceneManager.LoadScene("Gameplay");
//            SceneManager.LoadScene(GameScene.name);
        }
    
        /// <summary>
        /// Load menu scene
        /// </summary>
        public void LoadMenuScene()
        {
            SceneManager.LoadScene("Menu");
            SceneManager.LoadScene(MenuScene.name);
        }
    }
}
