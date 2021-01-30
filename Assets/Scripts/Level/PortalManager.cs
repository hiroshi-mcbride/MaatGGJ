using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public Portal[] portals;
    public GameObject[] lights;
    public Material onMaterial;

    //public Transform destination;
    private int numActivated;

    private bool[] lightsOn;
    public bool this[int i]
    {
        set
        {
            if (!lightsOn[i] && value)
            {
                // turn on light
                lightsOn[i] = value;
                lights[i].GetComponent<Renderer>().material = onMaterial;

                numActivated++;
                if (numActivated == portals.Length)
                {
					EventManager.RaiseEvent(EventType.UNLOCKNEXTLEVEL);
				}
            }
        }
    }

    private void Awake()
    {
        lightsOn = new bool[portals.Length];
    }


    private void Update()
    {
        for(int i = 0; i < portals.Length; i++)
        {
            if (portals[i].IsUsed())
            {
                this[i] = true;
            }
        }
    }

}
