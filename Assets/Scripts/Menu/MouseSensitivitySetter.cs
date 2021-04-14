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
    }
    public void SetSensitivity()
    {
        PlayerPrefs.SetFloat("Mouse Sensitivity", slider.value);
        if (PlayerLook.Instance!=null)
        {
            EventManager.RaiseEvent(EventType.SENSITIVITY_CHANGED);
        }
    }
}
