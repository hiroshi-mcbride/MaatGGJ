using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeInit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FMODUnity.RuntimeManager.GetBus("bus:/").setVolume(0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
