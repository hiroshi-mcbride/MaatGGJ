using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeTrigger : MonoBehaviour
{

    private bool PlayerInTrigger
    {
        get { return playerInTrigger; }
        set
        {
            if (playerInTrigger != value)
            {
                if (!AudioManager.Instance.bankLoaded)
                {
                    playerInTrigger = value;
                    return;
                }
                
                if (value)
                {
                    AudioManager.Instance.StartFade(audioType, false);
                }
                else
                {
                    AudioManager.Instance.StartFade(audioType, true);
                }

                playerInTrigger = value;

                //if (startStop)
                //{
                //    AudioManager.Instance.StartFade(audioType, true);
                //}
                //else
                //{
                //    AudioManager.Instance.StartFade(audioType, false);
                //}

            }
        }
    }
    private bool playerInTrigger = true;

    public bool startStop;
    public AudioType audioType;

    private void Awake()
    {
        EventManager.AddListener(EventType.STAIRS_MUSIC_FINISHED, OnStairsFinished);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Player.Instance.gameObject)
        {
            PlayerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == Player.Instance.gameObject)
        {
            PlayerInTrigger = false;
        }
    }

    void OnStairsFinished() 
    { 
        if (!PlayerInTrigger)
        {
            AudioManager.Instance.StartFade(audioType, true);
        }
    }
}
