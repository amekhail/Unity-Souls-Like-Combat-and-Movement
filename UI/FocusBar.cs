using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AVE
{
    public class FocusBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        public void SetMaxFocus(float maxFocus)
        {
            slider.maxValue = maxFocus;
            slider.value = maxFocus;
        }

        public void SetCurrentFocus(float currentFocus)
        {
            slider.value = currentFocus;
        }
    }
}
