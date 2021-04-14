using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private Transform playerBody;
    [SerializeField] [Range(0, 400)] private float mouseSensitivity = 100f;
#pragma warning restore 0649

    [HideInInspector] public Camera camera;
    private float xAxisClamp;

    private static PlayerLook instance;
    public static PlayerLook Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerLook>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        if (instance != this)
        {
            Destroy(gameObject);
        }
        camera = GetComponent<Camera>();

        LockCursor();
        mouseSensitivity = PlayerPrefs.GetFloat("Mouse Sensitivity");
        EventManager.AddListener(EventType.SENSITIVITY_CHANGED, SetSensitivity);
    }

    private void Update()
    {
        CameraRotation();

        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if (Cursor.lockState == CursorLockMode.Locked)
        //    {
        //        Cursor.lockState = CursorLockMode.None;
        //    }
        //    else
        //    {
        //        Cursor.lockState = CursorLockMode.Locked;
        //    }
        //}
    }
    
    private void CameraRotation()
    {
        //set mouse movement values
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xAxisClamp += mouseY;

        //clamp rotation when looking up
        if (xAxisClamp > 90.0f)
        {
            xAxisClamp = 90.0f;
            mouseY = 0.0f;
            ClampXAxisRotationToValue(270.0f);
        }

        //clamp rotation when looking down
        if (xAxisClamp < -80.0f)
        {
            xAxisClamp = -80.0f;
            mouseY = 0.0f;
            ClampXAxisRotationToValue(80.0f);
        }

        transform.Rotate(Vector3.left * mouseY);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void ClampXAxisRotationToValue(float _value)
    {
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = _value;
        transform.eulerAngles = eulerRotation;
    }

    public void SetSensitivity()
    {
        mouseSensitivity = PlayerPrefs.GetFloat("Mouse Sensitivity");
    }

    private void LockCursor() => Cursor.lockState = CursorLockMode.Locked;
    
}