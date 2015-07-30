//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections;

//[RequireComponent(typeof(Rigidbody))]
//public class PlayerShipControl_old : MonoBehaviour {

//	[Tooltip("How quickly max speed is reached.")]
//	public float Thrust = 20;
//	[Range(0, 100)]
//	public float MaxSpeed = 75f;
//	public float Damping = 5;
//	public float RotationSpeed = .5f;
//	public Vector3 Velocity = Vector3.zero;
//	public Image Reticule;
//	public new Camera camera;
//	Transform Target;
//	Vector3 OldMousePos = Vector3.zero;

//	void Awake() {

//		Cursor.visible = false;
//		Cursor.lockState = CursorLockMode.Confined;

//	}

//	void Update() {

//		UpdatePosition();

//		UpdatePitch();
//		//UpdateHeading();
//	}

//	private void UpdatePitch() {

//		float input = Input.GetAxis("Pitch");

//		if (input != 0) {

//		}
//	}

//	private void ClampCursor() {

//		if (Reticule.transform.position.x > Screen.width) {
//			Reticule.transform.position =
//				new Vector3(Screen.width, Reticule.transform.position.y, Reticule.transform.position.z);
//		} else if (Reticule.transform.position.x < 0) {
//			Reticule.transform.position =
//				new Vector3(0, Reticule.transform.position.y, Reticule.transform.position.z);
//		}
//		if (Reticule.transform.position.y > Screen.height) {
//			Reticule.transform.position =
//				new Vector3(Reticule.transform.position.x, Screen.height, Reticule.transform.position.z);
//		} else if (Reticule.transform.position.y < 0) {
//			Reticule.transform.position =
//				new Vector3(Reticule.transform.position.x, 0, Reticule.transform.position.z);
//		}
//	}

//	private void UpdateHeading() {

//		Vector3 mousePosDelta = Input.mousePosition - OldMousePos;
//		OldMousePos = Input.mousePosition;
//		Reticule.transform.position = Input.mousePosition;
//		ClampCursor();

//		RaycastHit hitInfo;
//		Ray ray = camera.ScreenPointToRay(Reticule.transform.position);
//		Debug.DrawRay(ray.origin, ray.direction);

//		if (Physics.Raycast(ray, out hitInfo, 500f)) {
//			Debug.Log(hitInfo.transform.name);
//			Target = hitInfo.transform;

//			Quaternion targetRotation = Quaternion.LookRotation(Target.position - transform.position);
//			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.time * RotationSpeed);

//		} else {
//			Target = null;
//		}
//	}

//	private void UpdatePosition() {

//		// Forward, back, and side to side motion
//		float movementX = Input.GetAxis("Horizontal");
//		float movementY = Input.GetAxis("Vertical");
//		Vector3 movement = transform.TransformDirection(new Vector3(movementX, 0, movementY) * Thrust);

//		transform.position = Vector3.SmoothDamp(
//			transform.position,
//			transform.position + movement,
//			ref Velocity,
//			Damping,
//			MaxSpeed,
//			Time.deltaTime);
//	}
//}
