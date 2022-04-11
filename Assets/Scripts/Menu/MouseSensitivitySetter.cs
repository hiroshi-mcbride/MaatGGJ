using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivitySetter : MonoBehaviour
{
    Slider slider;
    private void Awake()
    {
        slider = GetComponent<Slider>();
        if (PlayerPrefs.HasKey("Mouse Sensitivity"))
        {
            slider.value = PlayerPrefs.GetFloat("Mouse Sensitivity");
        }
        else
        {
            slider.value = 200f;
        }
    }
    public void SetSensitivity()
    {
        PlayerPrefs.SetFloat("Mouse Sensitivity", slider.value);
        EventManager.RaiseEvent(EventType.SENSITIVITY_CHANGED);
    }
}
