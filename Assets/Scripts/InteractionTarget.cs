using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTarget : MonoBehaviour
{
	[SerializeField] [Range(0, 20)] private float raycastDistance = 10f;

	[HideInInspector] public GameObject target;
	private GameObject lastTarget;
	public LayerMask lm;

    private void Update()
	{
		RaycastHit hit;
		//Cast a ray and scan for an Interactable target
		if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, raycastDistance, lm))
		{
			//Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
			GameObject i = hit.transform.gameObject;

			if (i != null && lastTarget != i)
			{
				if (i.activeInHierarchy)
				{
					target = i;
					//Debug.Log(target);
				}

			}
			else if (lastTarget != i)
			{
				target = null;
			}
		}
	}
}