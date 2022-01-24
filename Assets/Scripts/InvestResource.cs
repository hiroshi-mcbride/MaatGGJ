using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InvestResource : MonoBehaviour
{
	public List<GameObject> UIElements;
	public Renderer pole;
	public Material poleActive, poleInactive;
	[SerializeField] private int cost;
	public int Cost { get { return cost; } private set { cost = value; } }
	[SerializeField] private EventType eventType;
	EventType otherEvent;

    private void Awake()
    {

        if (eventType == EventType.UNLOCK_STAIR_NORTH)
        {
			otherEvent = EventType.UNLOCK_STAIR_WEST;
			EventManager.AddListener(EventType.UNLOCK_STAIR_WEST, ResetMaterial);
        } 
		else if (eventType == EventType.UNLOCK_STAIR_WEST)
        {
			otherEvent = EventType.UNLOCK_STAIR_NORTH;
			EventManager.AddListener(EventType.UNLOCK_STAIR_NORTH, ResetMaterial);
		}
    }


    public void Invest()
	{
		if (GameManager.Instance.rm.RemoveResource(Cost))
		{
			Unlock();
		}
	}

	void Unlock()
	{
		if(Cost>0)
        {
			AudioManager.Instance.PlayMusic(AudioType.MUSIC_UNLOCK);
			Cost = 0;
        }
		EventManager.RaiseEvent(eventType);
		UIElements[1].GetComponent<TMP_Text>().text = Cost.ToString();
		for(int i=0; i<UIElements.Count; i++)
        {
			UIElements[i].SetActive(false);
        }
		pole.material = poleActive;
	}

	public void ResetMaterial()
    {
		for (int i = 0; i < UIElements.Count; i++)
		{
			UIElements[i].SetActive(true);
		}
		pole.material = poleInactive;
	}

    private void OnDestroy()
    {
		EventManager.RemoveListener(otherEvent, ResetMaterial);
    }
}