using UnityEngine;

namespace Score
    {
        public class ScoreManager : MonoBehaviour
        {
            public static ScoreManager Instance{get; private set;}

            [SerializeField] public HappinessManager happinessManager;
            [SerializeField] public CampfireManager campfireManager;
            [SerializeField] public DayManager dayManager;
            
            [SerializeField] public float baseCorrectlyCookedFood = 100f;
            [SerializeField] public float baseIncorrectlyCookedFood = 25f;
            [SerializeField] public float baseTrash = 50f;

            public float Score;

            private void Awake()
            {
                if (Instance != null && Instance != this)
                {
                    Destroy(gameObject);
                    return;
                }

                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            public void AddScore(float AddScore)
            {
                Score += AddScore * dayManager.dayMultiplier * happinessManager.happinessMultiplier;
            }

            public void AddCorrectlyCookedFoodScore()
            {
                happinessManager.Increase();
                AddScore(baseCorrectlyCookedFood);
            }

            public void AddIncorrectlyCookedFoodScore()
            {
                happinessManager.Decrease();
                AddScore(baseIncorrectlyCookedFood);
            }

            public void AddTrashScore()
            {
                happinessManager.Increase();
                AddScore(baseTrash);
            }

            public void UpdateCampfirePassiveScore()
            {
                campfireManager.AddCampfirePassiveScore();
            }

        }

    }

