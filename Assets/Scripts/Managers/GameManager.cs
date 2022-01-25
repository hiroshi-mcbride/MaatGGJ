using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	private static GameManager instance = null;
	public static GameManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<GameManager>();
			}
			return instance;
		}
	}

	public ResourceManager rm;
	public Player player;
	public GameObject pauseMenu;

	private void Awake()
    {
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}

		rm = new ResourceManager();
		player = Player.Instance;

		//StartCoroutine(SkipLevel1());
	}

    private void Update()
    {
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			PauseGame(); 
		}
	}

	public void PauseGame()
    {
		if (!pauseMenu.activeInHierarchy)
		{
			Time.timeScale = 0.0f;
			pauseMenu.SetActive(true);
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			return;
		}
		else
		{
			Time.timeScale = 1.0f;
			pauseMenu.SetActive(false);
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	public void GotoMainMenu()
	{
		SceneManager.LoadScene(0);
		Time.timeScale = 1.0f;
	}

 //   IEnumerator SkipLevel1()
	//{
	//	yield return new WaitForSeconds(1.0f);
	//	if (SceneManager.GetActiveScene().buildIndex == 1)
	//	{
	//		EventManager.RaiseEvent(EventType.UNLOCK_NEXT_LEVEL);
	//	}
	//}
	
}
