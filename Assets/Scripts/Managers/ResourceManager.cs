using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
	private int resources;
	public int Resources { get => resources; private set => resources = value; }

	int maxCoins; 

	int TotalCollected
    {
		get { return totalCollected; }
		set
        {
			if (value != totalCollected)
            {
				if (value >= maxCoins)
				{
					EventManager.RaiseEvent(EventType.COINS_FULL);
				}
				totalCollected = value;
            }
        }
    }

	int totalCollected;

	public ResourceManager(int _maxCoins)
    {
		EventManager.AddListener(EventType.COIN_COLLECTED, () => TotalCollected++);
		TotalCollected = 0;
		maxCoins = _maxCoins;
    }

	public void AddResource(int amount)
	{
		Resources += amount;
	}

	public bool RemoveResource(int amount)
	{
		if (Resources >= amount)
		{
			Resources -= amount;
			return true;
		}
		return false;
	}
}