using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AVE
{
    public class StaminaBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        public void setMaxStamina(float maxStamina)
        {
            slider.maxValue = maxStamina;
            slider.value = maxStamina;
        }

        public void setCurrentStamina(float currentStamina)
        {
            slider.value = currentStamina;
        }

    }
}