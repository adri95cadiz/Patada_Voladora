using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	public GameObject playerLight;
	[HideInInspector] public bool killCamMove;
	
	private Vector3 targetPosition;
	private Quaternion targetRotation;
	private float targetFieldOfView;
	private float camSpeed;
	
	private Transform player1;
	private Transform player2;
	
	private Vector3 cameraStartingPos;
	private float standardZoom;
	private float standardDistance;
	private Quaternion standardRotation;
	private float fieldOfView;


	void Start(){
		playerLight = GameObject.Find("Player Light");
		player1 = GameObject.Find("Player1").transform;
		player2 = GameObject.Find("Player2").transform;

		ResetCam();
		standardZoom = cameraStartingPos.z;
		standardDistance = Vector3.Distance(player1.position, player2.position);
		UFE.freeCamera = false;
	}

	public void ResetCam(){
		cameraStartingPos = Camera.main.transform.position = UFE.config.cameraOptions.initialDistance;
		standardRotation = Camera.main.transform.localRotation = Quaternion.Euler(UFE.config.cameraOptions.initialRotation);
		fieldOfView = Camera.main.fieldOfView = UFE.config.cameraOptions.initialFieldOfView;

	}
	
	void FixedUpdate () {
		if (killCamMove) return;
		if (UFE.freeCamera) {
			Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFieldOfView, Time.deltaTime * camSpeed * 2);
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPosition, Time.deltaTime * camSpeed * 2);
			Camera.main.transform.localRotation = Quaternion.Slerp(Camera.main.transform.localRotation, targetRotation, Time.deltaTime * camSpeed * 2);

		}else{
			Vector3 newPosition = ((player1.position + player2.position)/2) + cameraStartingPos;
			newPosition.x = Mathf.Clamp(newPosition.x, 
				UFE.config.selectedStage.leftBoundary + 8, 
				UFE.config.selectedStage.rightBoundary - 8);
			newPosition.z = standardZoom - Vector3.Distance(player1.position, player2.position) + standardDistance;
			newPosition.z = Mathf.Clamp(newPosition.z, -UFE.config.cameraOptions.maxZoom, -UFE.config.cameraOptions.minZoom);
			
			Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, fieldOfView, Time.deltaTime * UFE.config.cameraOptions.smooth);
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, newPosition, Time.deltaTime * UFE.config.cameraOptions.smooth);
			Camera.main.transform.localRotation = Quaternion.Slerp(Camera.main.transform.localRotation, standardRotation, Time.deltaTime * UFE.config.cameraOptions.smooth);
			
			if (Camera.main.transform.localRotation == standardRotation)
				UFE.normalizedCam = true;
				if (playerLight != null) playerLight.GetComponent<Light>().enabled = false;
		}
	}
	
	public void moveCameraToLocation(Vector3 targetPos, Vector3 targetRot, float targetFOV, float speed){
		targetFieldOfView = targetFOV;
		targetPosition = targetPos;
		targetRotation = Quaternion.Euler(targetRot);
		camSpeed = speed;
		UFE.freeCamera = true;
		UFE.normalizedCam = false;
		if (playerLight != null) playerLight.GetComponent<Light>().enabled = true;
	}
	
	public Vector3 getRelativePosition(Transform origin, Vector3 position) {
		Vector3 distance = position - origin.position;
		Vector3 relativePosition = Vector3.zero;
		relativePosition.x = Vector3.Dot(distance, origin.right.normalized);
		relativePosition.y = Vector3.Dot(distance, origin.up.normalized);
		relativePosition.z = Vector3.Dot(distance, origin.forward.normalized);
		
		return relativePosition;
	}
}
