using UnityEngine;
using System.Collections;
using Interfaces;
using PlayerScripts;

namespace GamePlay.Interactables
{
    public class ChopTarget : MonoBehaviour, ITreeTarget
    {
        [Header("Interaction Settings (ChopTarget)")]
        [Tooltip("The text prompt shown to the player.")]
        [SerializeField] 
        private string interactionPrompt = "Chop Tree";
        public string InteractionPrompt => interactionPrompt;

        [Tooltip("The time (in seconds) required to fully chop down the tree.")]
        [SerializeField]
        private float chopDuration = 1.0f;

        [Header("Resource Output")]
        [Tooltip("The Log Prefab to be spawned when the tree is destroyed.")]
        [SerializeField]
        private GameObject logPrefab; 
        
        [Tooltip("The number of logs that will be spawned.")]
        [SerializeField]
        private int numberOfLogs = 3;

        [Tooltip("Maximum radius logs will scatter from the tree's position.")]
        [SerializeField]
        private float scatterRadius = 0.5f;

        private bool isChopping = false;
        
        public void Chop(GameObject interactor, PlayerMovement playerMovement)
        {
            if (isChopping)
            {
                Debug.Log($"[CHOP IN PROGRESS] Tree is already being chopped. Wait for destruction.");
                return;
            }
            
            if (logPrefab == null)
            {
                Debug.LogError("[CHOP ERROR] Log Prefab is not assigned on ChopTarget component! Tree cannot drop resources.");
                playerMovement.SetPlayerState(PlayerState.IsIdle);
                return;
            }
            
            isChopping = true;
            
            playerMovement.SetPlayerState(PlayerState.IsInteracting);

            Debug.Log($"[CHOP START] Player ({interactor.name}) started chopping! Will take {chopDuration} seconds.");
            
            StartCoroutine(ChopTreeCoroutine(playerMovement));
        }
        
        private IEnumerator ChopTreeCoroutine(PlayerMovement playerMovement)
        {
            yield return new WaitForSeconds(chopDuration);
            
            Debug.Log($"[CHOP COMPLETE] Spawning {numberOfLogs} logs and destroying the tree!");
            
            for (int i = 0; i < numberOfLogs; i++)
            {
                var randomCircle = Random.insideUnitCircle * scatterRadius;
                var spawnPosition = new Vector3(
                    transform.position.x + randomCircle.x, 
                    transform.position.y, 
                    transform.position.z + randomCircle.y
                );
                
                Instantiate(logPrefab, spawnPosition, Quaternion.identity);
            }
            
            playerMovement.SetPlayerState(PlayerState.IsIdle);
            
            Destroy(gameObject);
        }
        
        public void Interact(GameObject interactor, PlayerMovement playerMovement)
        {
            Debug.Log($"[GENERIC INTERACT DEBUG] Player is near {InteractionPrompt}. The player may need the Axe to initiate the Chop action.");
            playerMovement.SetPlayerState(PlayerState.IsIdle);
        }
    }
}
