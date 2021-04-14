using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using UnityEngine.UI;

public class VolumeSetter : MonoBehaviour
{
    Bus masterBus;
    Slider slider;
    private void Awake()
    {
        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
        slider = GetComponent<Slider>();
    }
    private void OnEnable()
    {
        float volume;
        masterBus.getVolume(out volume);
        slider.value = volume;
    }
    public void SetVolume()
    {
        masterBus.setVolume(slider.value);
    }
}
