using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LiftBehaviour : MonoBehaviour
{
	[SerializeField] private GameObject leftDoor;
	[SerializeField] private GameObject rightDoor;
	[SerializeField] private Transform leftDoorGoal;
	[SerializeField] private Transform rightDoorGoal;
	[SerializeField] private Transform leftDoorStart;
	[SerializeField] private Transform rightDoorStart;

	private bool opening = false;
	private bool closing = false;
	private bool raising = false;
	bool playerInLift;

	private bool LiftStopped
    {
		get { return liftStopped; }
		set
        {
			if (!liftStopped && value==true && SceneManager.GetActiveScene().buildIndex == 2)
            {
				StopRaisingLift();
            }
			liftStopped = value;
        }
    }
	private bool liftStopped;
	bool levelInProgress;
	private float step = 20;

	private IEnumerator openCoroutine, closeCoroutine;

	private void Start()
	{
		openCoroutine = OpenDoors(3f);
		closeCoroutine = CloseDoors(3f);
		EventManager.AddListener(EventType.LIFT_UNLOCKED, OpenLift);
		EventManager.AddListener(EventType.UNLOCK_NEXT_LEVEL, RaiseLift);
		EventManager.AddListener(EventType.END_GAME, EndGame);
		levelInProgress = true;
		if (SceneManager.GetActiveScene().buildIndex == 2)
		{
			liftStopped = false;
		}
		else
        {
			liftStopped = true;
        }
	}

	private void Update()
	{

		if (Input.GetKeyDown(KeyCode.T)) { EventManager.RaiseEvent(EventType.LIFT_UNLOCKED); }
		//if (Input.GetKeyDown(KeyCode.G)) { StartCoroutine(closeCoroutine); }
        if (opening && !closing)
        {
            //leftDoor.transform.position = Vector3.MoveTowards(leftDoor.transform.position, leftDoorGoal.position, step * Time.deltaTime);
            //rightDoor.transform.position = Vector3.MoveTowards(rightDoor.transform.position, rightDoorGoal.position, step * Time.deltaTime);
            leftDoor.transform.localScale = Vector3.MoveTowards(leftDoor.transform.localScale, new Vector3(0.0f, 1.0f, 1.0f), 0.5f * Time.deltaTime);
            rightDoor.transform.localScale = Vector3.MoveTowards(rightDoor.transform.localScale, new Vector3(0.0f, 1.0f, 1.0f), 0.5f * Time.deltaTime);
        }

        if (opening && closing)
        {
			//leftDoor.transform.position = Vector3.MoveTowards(leftDoor.transform.position, leftDoorStart.position, step * Time.deltaTime);
			//rightDoor.transform.position = Vector3.MoveTowards(rightDoor.transform.position, rightDoorStart.position, step * Time.deltaTime);
			Player.Instance.transform.SetParent(transform);
			Player.Instance.transform.localPosition = Vector3.Lerp(Player.Instance.transform.localPosition, new Vector3(0f, 1f, 0.25f), 0.04f);
			Player.Instance.transform.localRotation = Quaternion.Lerp(Player.Instance.transform.localRotation, transform.localRotation, 0.04f);
			leftDoor.transform.localScale = Vector3.MoveTowards(leftDoor.transform.localScale, Vector3.one, 0.5f * Time.deltaTime);
            rightDoor.transform.localScale = Vector3.MoveTowards(rightDoor.transform.localScale, Vector3.one, 0.5f * Time.deltaTime);
            if (leftDoor.transform.localScale == Vector3.one && rightDoor.transform.localScale == Vector3.one)
            {
                //EventManager.RaiseEvent(EventType.END_GAME);
                if (SceneManager.GetActiveScene().buildIndex == 1)
                {
                    EventManager.RaiseEvent(EventType.UNLOCK_NEXT_LEVEL);
                    opening = false;
                }
                if (SceneManager.GetActiveScene().buildIndex == 2)
                {
                    EventManager.RaiseEvent(EventType.END_GAME);
                    opening = false;
                }
            }
        }

        if (raising)
		{
			step += 0.01f;
			transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 1000, transform.position.z), step * Time.deltaTime);
			if (transform.position.y > 450)
			{
				if (SceneManager.GetActiveScene().buildIndex == 1)
				{
					SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
				}
			}
		}
		else if (!raising && transform.position.y < 0)
		{
			transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 0, transform.position.z), step * Time.deltaTime);
			if (transform.position.y > -25)
			{
				step -= 0.0175f;
			}
		}
		else if (!raising && SceneManager.GetActiveScene().buildIndex == 2)
		{
			LiftStopped = true;
		}
	}

	private IEnumerator OpenDoors(float _seconds)
    {
		while(leftDoor.transform.localScale.x > 0f)
        {
			leftDoor.transform.localScale = new Vector3(leftDoor.transform.localScale.x - 0.01f / _seconds, 1f, 1f);
			rightDoor.transform.localScale = new Vector3(rightDoor.transform.localScale.x - 0.01f / _seconds, 1f, 1f);
			yield return new WaitForSeconds(0.01f);
		}
		Debug.Log("Doors Opened");

    }

	private IEnumerator CloseDoors(float _seconds)
    {
		while (leftDoor.transform.localScale.x < 1f)
		{
			if (playerInLift)
			{
				StopCoroutine(closeCoroutine);
				//StartCoroutine(openCoroutine);
				yield return null;
			}
			leftDoor.transform.localScale = new Vector3(leftDoor.transform.localScale.x + 0.01f / _seconds, 1f, 1f);
			rightDoor.transform.localScale = new Vector3(rightDoor.transform.localScale.x + 0.01f / _seconds, 1f, 1f);
			yield return new WaitForSeconds(0.01f);
		}
		Debug.Log("Doors Closed");

	}

	private IEnumerator WaitOnPlayerExit()
    {
		yield return null;
	}

	private void OpenLift()
	{
		step = 1;
		opening = true;
		levelInProgress = false;
		//StartCoroutine(OpenDoors(2f));
	}

	private void RaiseLift()
	{
		AudioManager.Instance.PlayMusic(AudioType.MUSIC_ASCEND);
		step = 1;
		
		raising = true;
	}

	private void EndGame()
    {
		AudioManager.Instance.PlayMusic(AudioType.MUSIC_ENDING);
		step = 1;
		raising = true;
		//StartCoroutine(FadeInCredits);
	}

	private IEnumerator FadeInCredits()
    {
		yield return null;
    }

	private void StopRaisingLift()
	{
		step = 1;
		Player.Instance.transform.parent = null;
		Player.Instance.charCtrl.enabled = true;
		StartCoroutine(openCoroutine);
	}

	private void OnTriggerEnter(Collider other)
	{

		if (other.gameObject == Player.Instance.gameObject)
		{
			playerInLift = true;
			if (!opening || levelInProgress)
			{
				return;
			}
			step = 1;
			closing = true;
			Player.Instance.charCtrl.enabled = false;
			//Player.Instance.GetComponent<PlayerLook>().enabled = false;
			
			
			//Player.Instance.transform.SetParent(transform);
			//Player.Instance.charCtrl.enabled = false;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject == Player.Instance.gameObject)
		{
			Debug.Log("player left lift");
			playerInLift = false;
			if (opening || !levelInProgress)
			{
				return;
			}
			StopCoroutine(openCoroutine);
			StartCoroutine(closeCoroutine);
		}
	}
}