using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField] public CharacterController charCtrl;
	[SerializeField] private Transform groundCheck;
	[SerializeField] private Camera vCam;

	[SerializeField] private float moveSpeed = 10.0f;
	private float speed = 10.0f;
	public float gravity = -20f;
	
	public float groundDistance = .4f;
	public LayerMask groundMask;
	private bool isGrounded;
	private bool wasGrounded;

	public float jumpHeight = 2.0f;

	Vector3 velocity;
	Vector3 move = Vector3.zero;

	float gracePeriod = 0.1f;

	private static Player instance;
	public static Player Instance
    {
		get 
		{ 
			if (instance == null)
            {
				instance = FindObjectOfType<Player>();
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
    }

    void Start()
	{
		//hide the mouse
		Cursor.lockState = CursorLockMode.Locked;
		charCtrl = GetComponent<CharacterController>();
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Interact();
		}

		//make character model rotate with the camera
		var rot = vCam.transform.rotation;
		rot.x = 0;
		rot.z = 0;
		transform.rotation = rot;

		////running
		//if (Input.GetButton("Fire3") == true)
		//{
		//	speed = moveSpeed * 2;
		//}
		//else
		//{
		//	speed = moveSpeed;
		//}

		//ground check
		isGrounded = (Physics.CheckBox(new Vector3(groundCheck.position.x - .5f, groundCheck.position.y, groundCheck.position.z), new Vector3(groundDistance, 0.3f, groundDistance), Quaternion.identity, groundMask) &&
				Physics.CheckBox(new Vector3(groundCheck.position.x, groundCheck.position.y, groundCheck.position.z - .5f), new Vector3(groundDistance, 0.3f, groundDistance), Quaternion.identity, groundMask) &&
				Physics.CheckBox(new Vector3(groundCheck.position.x + .5f, groundCheck.position.y, groundCheck.position.z), new Vector3(groundDistance, 0.3f, groundDistance), Quaternion.identity, groundMask) &&
				Physics.CheckBox(new Vector3(groundCheck.position.x, groundCheck.position.y, groundCheck.position.z + .5f), new Vector3(groundDistance, 0.3f, groundDistance), Quaternion.identity, groundMask));

		if (isGrounded == false)
		{
			if (gracePeriod > 0)
			{
				gracePeriod -= Time.deltaTime;
			}
			else
			{
				wasGrounded = false;
			}
		}
		else if (isGrounded == true)
		{
			wasGrounded = true;
			gracePeriod = 0.1f;
		}

		//reset gravity if grounded
		if (isGrounded && velocity.y < 0)
		{
			velocity.y = -2f;
		}

		float x = Input.GetAxisRaw("Horizontal");
		float z = Input.GetAxisRaw("Vertical");

		//movement
		move = transform.right * x + transform.forward * z;
		move.Normalize();
		charCtrl.Move(move * speed * Time.deltaTime);

		//gravity
		if (Input.GetButtonDown("Jump") && (isGrounded || wasGrounded))
		{
			velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
		}

		velocity.y += gravity * Time.deltaTime;

		charCtrl.Move(velocity * Time.deltaTime);
	}

	private void Interact()
	{
		GameObject target = vCam.GetComponent<InteractionTarget>().target;
		if (target != null)
		{
			if (target.tag == "Interactable")
			{
				target.GetComponent<InvestResource>().Invest();
				AudioManager.Instance.interactSFX.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(target.gameObject));
				AudioManager.Instance.PlaySFX(AudioType.SFX_INTERACT);
				target = null;
			}
		}
	}
}