using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectResource : MonoBehaviour
{
    private void OnEnable()
    {
		EventManager.AddListener(EventType.COINS_FULL, OnCoinsFull);
    }
    private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == Player.Instance.gameObject)
		{
			AudioManager.Instance.pickupSFX.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
			AudioManager.Instance.PlaySFX(AudioType.SFX_PICKUP);
			GameManager.Instance.rm.AddResource(1);
			EventManager.RaiseEvent(EventType.COIN_COLLECTED);
			gameObject.SetActive(false);
		}
	}

	void OnCoinsFull()
    {
		if (SceneManager.GetActiveScene().buildIndex == 1)
        {
			gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
		EventManager.RemoveListener(EventType.COINS_FULL, OnCoinsFull);
	}
}