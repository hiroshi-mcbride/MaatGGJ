using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lift : MonoBehaviour
{

    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;
    float doorTime;

    private IEnumerator openCoroutine, closeCoroutine;
    bool coroutineRunning, ascending;
    bool arrived = true;

    private void Awake()
    {
        openCoroutine = OpenDoors(doorTime);
        closeCoroutine = CloseDoors(doorTime);
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.buildIndex == 2)
        {
            arrived = false;
        }
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.gameObject.GetComponent<Player>() != null && !coroutineRunning && !ascending)
        {
            StartCoroutine(closeCoroutine);
        }
    }

    private void OnTriggerExit(Collider _other)
    {
        if (_other.gameObject.GetComponent<Player>() != null && !coroutineRunning && !ascending)
        {
            StartCoroutine(closeCoroutine);
        }
    }

    
    private IEnumerator OpenDoors(float _seconds)
    {
        coroutineRunning = true;
        while (leftDoor.transform.localScale.x > 0f)
        {
            leftDoor.transform.localScale = new Vector3(leftDoor.transform.localScale.x - 0.01f / _seconds, 1f, 1f);
            rightDoor.transform.localScale = new Vector3(rightDoor.transform.localScale.x - 0.01f / _seconds, 1f, 1f);
            yield return new WaitForSeconds(0.01f);
        }
        Debug.Log("Doors Opened");
        coroutineRunning = false;
    }

    private IEnumerator CloseDoors(float _seconds)
    {
        coroutineRunning = true;
        while (leftDoor.transform.localScale.x < 1f)
        {
            leftDoor.transform.localScale = new Vector3(leftDoor.transform.localScale.x + 0.01f / _seconds, 1f, 1f);
            rightDoor.transform.localScale = new Vector3(rightDoor.transform.localScale.x + 0.01f / _seconds, 1f, 1f);
            yield return new WaitForSeconds(0.01f);
        }
        Debug.Log("Doors Closed");
        coroutineRunning = false;
    }
}
