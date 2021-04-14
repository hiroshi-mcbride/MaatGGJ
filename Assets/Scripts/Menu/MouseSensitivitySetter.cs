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
    }
    private void OnEnable()
    {
        
    }
    public void SetSensitivity()
    {

    }
}
