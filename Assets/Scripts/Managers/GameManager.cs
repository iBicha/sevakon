using Sevakon.Input;
using Sevakon.Settings;
using Sevakon.Systems;
using Sevakon.Utils;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Experimental.Input;

namespace Sevakon.Managers
{
    /// <summary>
    /// Manages the life cycle of a game session
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// Contains the settings of bullet spawning
        /// </summary>
        public BulletSettings bulletSettings;
        
        /// <summary>
        /// Contains the settings of the bullet sound effects
        /// </summary>
        public BulletSoundSettings bulletSoundSettings;
        
        /// <summary>
        /// Contains input settings
        /// </summary>
        public Controls controls;

        /// <summary>
        /// The audio source to play the bullet sound
        /// </summary>
        public AudioSource bulletSfx;
        
        /// <summary>
        /// The text label to display user score
        /// </summary>
        public TMP_Text scoreText;

        /// <summary>
        /// Game objects that make up the "Win" dialog
        /// </summary>
        public GameObject[] winDialogObjects;

        private int score;

        private int targetCount;

        private void Start()
        {
            Cursor.visible = false;
        }

        private void OnEnable()
        {
            EcsUtils.EnableSystems(this);
        }

        private void OnDisable()
        {
            EcsUtils.DisableSystems();
        }

        /// <summary>
        /// Increases the score of the player, and updates the UI
        /// </summary>
        public void IncreaseScore()
        {
            score += 10;
            scoreText.text = $"Score:{score}";
        }

        /// <summary>
        /// Increases the number of targets in the scene
        /// </summary>
        public void IncreaseTargetCount()
        {
            targetCount++;
        }

        /// <summary>
        /// Decreases the number of targets in the scene
        /// Displays the win dialog if the target count reaches zero
        /// </summary>
        public void DecreaseTargetCount()
        {
            targetCount--;
            if (targetCount <= 0)
            {
                ShowWinDialog();
            }
        }

        /// <summary>
        /// Displays the win dialog
        /// </summary>
        public void ShowWinDialog()
        {
            Cursor.visible = true;
            foreach (var winDialogObject in winDialogObjects)
            {
                if (winDialogObject != null)
                    winDialogObject.SetActive(true);
            }
            //We can disable the player here, but let's leave him roaming
        }
    }
}