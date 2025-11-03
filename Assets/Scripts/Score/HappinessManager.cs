using UnityEngine;

namespace Score
{
    public class HappinessManager : MonoBehaviour
    {
        [SerializeField] public float happiness = 50f;
        [SerializeField] public float increaseAmount = 10f;
        [SerializeField] public float decreaseAmount = 5f;
        [SerializeField] public float decreaseWileWaiting = 5f;
        [SerializeField] public float happinessMultiplier;
        [SerializeField] public float timeWaitedForDecrease = 5f;
        public float timeSinceLastDelivery;


        public void Awake()
        {
            happinessMultiplier = Mathf.Clamp(happiness / 50f, 0, 2);
        }       

        public void StartWaitingTime()
        {
            timeSinceLastDelivery += Time.deltaTime;
            if (timeSinceLastDelivery > timeWaitedForDecrease)
            {
                timeSinceLastDelivery = 0;
                DecreaseWileWaiting();
            }
        }

        public void Increase()
        {
            SetHappiness(happiness + increaseAmount);
        }

        public void Decrease()
        {
            SetHappiness(happiness - decreaseAmount);
        }

        public void DecreaseWileWaiting()
        {
            SetHappiness(happiness - decreaseWileWaiting);
        }

        public void SetHappiness(float value)
        {
            happiness = Mathf.Clamp(value, 0, 100);
        }
    }
}
