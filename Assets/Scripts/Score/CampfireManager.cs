using UnityEngine;

namespace Score
{
    public class CampfireManager : MonoBehaviour
    {
        [SerializeField] private float percentage;
        [SerializeField] private float baseCampfireScore;
        [SerializeField] private float amountOfFuelPerLog;
        [SerializeField] private float decreaseAmount = 1f;
        [SerializeField] private float thresholdToScore = 50f;


        public void UpdateCampfire() //campfire logic not relly sure if it should be on score
        {
            percentage -= decreaseAmount * Time.deltaTime;
            percentage = Mathf.Clamp(percentage, 0, 100);
        }

        public void AddCampfirePassiveScore()
        {
            if (percentage > thresholdToScore)
            {
                ScoreManager.Instance.AddScore(baseCampfireScore);
            }
        }

        public void AddSupply(float amountOfFuelPerLog)
        {
            percentage = Mathf.Clamp(percentage + amountOfFuelPerLog, 0, 100);
        }
    }
}
