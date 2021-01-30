using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//source: https://www.youtube.com/watch?v=cWpFZbjtSQg&ab_channel=SebastianLague
public class Portal : MonoBehaviour
{
    [SerializeField] private bool used = false;
    public bool IsUsed() => used;

	[Header("Main Settings")]
	public Portal linkedPortal;
	public MeshRenderer screen;
	public int recursionLimit = 5;
	[SerializeField] Camera portalCam;

	[Header("Advanced Settings")]
	public float nearClipOffset = 0.05f;
	public float nearClipLimit = 0.2f;

	// Private variables
	RenderTexture viewTexture;

	Camera playerCam;
	Material firstRecursionMat;
	List<PortalTraveller> trackedTravellers;
	MeshFilter screenMeshFilter;

	void Awake()
	{
		playerCam = Camera.main;
		portalCam = GetComponentInChildren<Camera>();
		portalCam.enabled = false;
		trackedTravellers = new List<PortalTraveller>();
		screenMeshFilter = screen.GetComponent<MeshFilter>();
		screen.material.SetInt("displayMask", 1);
	}

	void LateUpdate()
	{
		HandleTravellers();
	}

	void HandleTravellers()
	{

		for (int i = 0; i < trackedTravellers.Count; i++)
		{
			PortalTraveller traveller = trackedTravellers[i];
			Transform travellerT = traveller.transform;
			var m = linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * travellerT.localToWorldMatrix;

			Vector3 offsetFromPortal = travellerT.position - transform.position;
			int portalSide = System.Math.Sign(Vector3.Dot(offsetFromPortal, transform.forward));
			int portalSideOld = System.Math.Sign(Vector3.Dot(traveller.previousOffsetFromPortal, transform.forward));
			// Teleport the traveller if it has crossed from one side of the portal to the other
			if (portalSide != portalSideOld)
			{
				var positionOld = travellerT.position;
				var rotOld = travellerT.rotation;
				traveller.Teleport(transform, linkedPortal.transform, m.GetColumn(3), m.rotation);
				// Can't rely on OnTriggerEnter/Exit to be called next frame since it depends on when FixedUpdate runs
				linkedPortal.OnTravellerEnterPortal(traveller);
				trackedTravellers.RemoveAt(i);
				i--;
			}
			else
			{
				traveller.previousOffsetFromPortal = offsetFromPortal;
			}
		}
	}

	// Manually render the camera attached to this portal
	// Called after PrePortalRender, and before PostPortalRender
	public void Render()
	{
		// Skip rendering the view from this portal if player is not looking at the linked portal
		if (!CameraUtility.VisibleFromCamera(linkedPortal.screen, playerCam))
		{
			return;
		}

		CreateViewTexture();

		var localToWorldMatrix = playerCam.transform.localToWorldMatrix;
		var renderPositions = new Vector3[recursionLimit];
		var renderRotations = new Quaternion[recursionLimit];

		int startIndex = 0;
		portalCam.projectionMatrix = playerCam.projectionMatrix;
		for (int i = 0; i < recursionLimit; i++)
		{
			if (i > 0)
			{
				// No need for recursive rendering if linked portal is not visible through this portal
				if (!CameraUtility.BoundsOverlap(screenMeshFilter, linkedPortal.screenMeshFilter, portalCam))
				{
					break;
				}
			}
			localToWorldMatrix = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * localToWorldMatrix;
			int renderOrderIndex = recursionLimit - i - 1;
			renderPositions[renderOrderIndex] = localToWorldMatrix.GetColumn(3);
			renderRotations[renderOrderIndex] = localToWorldMatrix.rotation;

			portalCam.transform.SetPositionAndRotation(renderPositions[renderOrderIndex], renderRotations[renderOrderIndex]);
			startIndex = renderOrderIndex;
		}

		// Hide screen so that camera can see through portal
		screen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
		linkedPortal.screen.material.SetInt("displayMask", 0);

		for (int i = startIndex; i < recursionLimit; i++)
		{
			portalCam.transform.SetPositionAndRotation(renderPositions[i], renderRotations[i]);
			SetNearClipPlane();
			portalCam.Render();

			if (i == startIndex)
			{
				linkedPortal.screen.material.SetInt("displayMask", 1);
			}
		}

		// Unhide objects hidden at start of render
		screen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
	}

	// Called once all portals have been rendered, but before the player camera renders
	public void PostPortalRender()
	{
		ProtectScreenFromClipping(playerCam.transform.position);
	}

	void CreateViewTexture()
	{
		if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height)
		{
			if (viewTexture != null)
			{
				viewTexture.Release();
			}
			viewTexture = new RenderTexture(Screen.width, Screen.height, 0);
			// Render the view from the portal camera to the view texture
			portalCam.targetTexture = viewTexture;
			// Display the view texture on the screen of the linked portal
			linkedPortal.screen.material.SetTexture("_MainTex", viewTexture);
		}
	}

	// Sets the thickness of the portal screen so as not to clip with camera near plane when player goes through
	float ProtectScreenFromClipping(Vector3 viewPoint)
	{
		float halfHeight = playerCam.nearClipPlane * Mathf.Tan(playerCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
		float halfWidth = halfHeight * playerCam.aspect;
		float dstToNearClipPlaneCorner = new Vector3(halfWidth, halfHeight, playerCam.nearClipPlane).magnitude;
		float screenThickness = dstToNearClipPlaneCorner;

		Transform screenT = screen.transform;
		bool camFacingSameDirAsPortal = Vector3.Dot(transform.forward, transform.position - viewPoint) > 0;
		screenT.localScale = new Vector3(screenT.localScale.x, screenT.localScale.y, screenThickness);
		screenT.localPosition = Vector3.forward * screenThickness * ((camFacingSameDirAsPortal) ? 0.5f : -0.5f) + new Vector3(0, 0.15f, 0);
		return screenThickness;
	}

	// Use custom projection matrix to align portal camera's near clip plane with the surface of the portal
	// Note that this affects precision of the depth buffer, which can cause issues with effects like screenspace AO
	void SetNearClipPlane()
	{
		// Learning resource:
		// http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
		Transform clipPlane = transform;
		int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, transform.position - portalCam.transform.position));

		Vector3 camSpacePos = portalCam.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
		Vector3 camSpaceNormal = portalCam.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
		float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal) + nearClipOffset;

		// Don't use oblique clip plane if very close to portal as it seems this can cause some visual artifacts
		if (Mathf.Abs(camSpaceDst) > nearClipLimit)
		{
			Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

			// Update projection based on new clip plane
			// Calculate matrix with player cam so that player camera settings (fov, etc) are used
			portalCam.projectionMatrix = playerCam.CalculateObliqueMatrix(clipPlaneCameraSpace);
		}
		else
		{
			portalCam.projectionMatrix = playerCam.projectionMatrix;
		}
	}

	void OnTravellerEnterPortal(PortalTraveller traveller)
	{
		if (!trackedTravellers.Contains(traveller))
		{
			traveller.previousOffsetFromPortal = traveller.transform.position - transform.position;
			trackedTravellers.Add(traveller);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		var traveller = other.GetComponent<PortalTraveller>();
		if (traveller)
		{
			used = true;
			OnTravellerEnterPortal(traveller);
		}
	}

	void OnTriggerExit(Collider other)
	{
		var traveller = other.GetComponent<PortalTraveller>();
		if (traveller && trackedTravellers.Contains(traveller))
		{
			trackedTravellers.Remove(traveller);
		}
	}

	/*
     ** Some helper/convenience stuff:
     */

	int SideOfPortal(Vector3 pos)
	{
		return System.Math.Sign(Vector3.Dot(pos - transform.position, transform.forward));
	}

	bool SameSideOfPortal(Vector3 posA, Vector3 posB)
	{
		return SideOfPortal(posA) == SideOfPortal(posB);
	}

	Vector3 portalCamPos
	{
		get
		{
			return portalCam.transform.position;
		}
	}

	void OnValidate()
	{
		if (linkedPortal != null)
		{
			linkedPortal.linkedPortal = this;
		}
	}
}