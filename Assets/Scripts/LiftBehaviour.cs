using UnityEngine;
using UnityEngine.SceneManagement;

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
	private float step = 20;

	private void Start()
	{
		EventManager.AddListener(EventType.UNLOCK_LIFT, OpenLift);
		EventManager.AddListener(EventType.UNLOCK_NEXT_LEVEL, RaiseLift);
		

	}

	private void Update()
	{
		if (opening == true && closing == false)
		{
			//leftDoor.transform.position = Vector3.MoveTowards(leftDoor.transform.position, leftDoorGoal.position, step * Time.deltaTime);
			//rightDoor.transform.position = Vector3.MoveTowards(rightDoor.transform.position, rightDoorGoal.position, step * Time.deltaTime);
			leftDoor.transform.localScale = Vector3.MoveTowards(leftDoor.transform.localScale, new Vector3(0.0f, 1.0f, 1.0f), step * Time.deltaTime);
			rightDoor.transform.localScale = Vector3.MoveTowards(rightDoor.transform.localScale, new Vector3(0.0f, 1.0f, 1.0f), step * Time.deltaTime);
		}
		if (opening == true && closing == true)
		{
			//leftDoor.transform.position = Vector3.MoveTowards(leftDoor.transform.position, leftDoorStart.position, step * Time.deltaTime);
			//rightDoor.transform.position = Vector3.MoveTowards(rightDoor.transform.position, rightDoorStart.position, step * Time.deltaTime);
			leftDoor.transform.localScale = Vector3.MoveTowards(leftDoor.transform.localScale, Vector3.one, step * Time.deltaTime);
			rightDoor.transform.localScale = Vector3.MoveTowards(rightDoor.transform.localScale, Vector3.one, step * Time.deltaTime);
			if (leftDoor.transform.localScale == Vector3.one && rightDoor.transform.localScale == Vector3.one)
            {
				//EventManager.RaiseEvent(EventType.END_GAME);
				if (SceneManager.GetActiveScene().buildIndex==1)
                {
					EventManager.RaiseEvent(EventType.UNLOCK_NEXT_LEVEL);
				}
			}
		}
		if (raising == true)
		{
			step += 0.02f;
			transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 1000, transform.position.z), step * Time.deltaTime);
			if (transform.position.y > 750)
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
			}
		}
		else if (raising == false && transform.position.y < 0)
		{
			transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 0, transform.position.z), step * Time.deltaTime);
			if (transform.position.y > -25)
			{
				step -= 0.0175f;
			}
		}
		else if (raising == false)
		{
			StopRaisingLift();
		}
	}

	private void OpenLift()
	{
		step = 1;
		opening = true;
	}

	private void RaiseLift()
	{
		AudioManager.Instance.PlayMusic(AudioType.MUSIC_ASCEND);
		step = 1;
		Player.Instance.transform.SetParent(transform);
		Player.Instance.charCtrl.enabled = false;
		raising = true;
	}

	private void StopRaisingLift()
	{
		step = 1;
		Player.Instance.transform.parent = null;
		Player.Instance.charCtrl.enabled = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == Player.Instance.gameObject)
		{
			closing = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject == Player.Instance.gameObject)
		{
			closing = false;
		}
	}
}