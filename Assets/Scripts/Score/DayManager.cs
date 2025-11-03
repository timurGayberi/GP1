using UnityEngine;
using TMPro;

namespace Score
{
    public class DayManager : MonoBehaviour
    {
        [SerializeField] public float dayCount = 1f;
        [SerializeField] public float dayMultiplier= 1f;

        public void NextDay()
        {
            dayCount++;
            if (dayCount > 1f)
                dayMultiplier *= 1.1f;
        }
    }
}
