using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "GameData/PlayerStats")]
    public class PlayerStatsSo : ScriptableObject
    {
        [Header("PlayerStats")]
        public float movementSpeed;
        public float sprintSpeed;
    }
}
