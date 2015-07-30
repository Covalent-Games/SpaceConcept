using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Client {
	public class PlayerShipControl : MonoBehaviour {

		public float MaxSpeed = 100f;
		public float CurrentSpeed = 0f;
		public float Thrust = 15f;
		public float Braking = 25f;
		public Vector3 Velocity = Vector3.zero;
		public float PitchTopSpeed = 30f;
		public float RollTopSpeed = 80f;

		// Control keys
		public KeyCode ThrottleUp = KeyCode.Space;
		public KeyCode ThrottleDown = KeyCode.Tab;
		public KeyCode PitchUp = KeyCode.S;
		public KeyCode PitchDown = KeyCode.W;
		public KeyCode YawLeft = KeyCode.Z;
		public KeyCode YawRight = KeyCode.C;
		public KeyCode RollLeft = KeyCode.A;
		public KeyCode RollRight = KeyCode.D;
		public KeyCode HorizontalLeft = KeyCode.Q;
		public KeyCode HorizontalRight = KeyCode.E;
		public KeyCode VerticalUp = KeyCode.R;
		public KeyCode VerticalDown = KeyCode.F;
		public KeyCode SelectCenteredTarget = KeyCode.Tab;

		private Image ThrusterBarImage;

		void Awake() {

			ThrusterBarImage = GameObject.Find("Thruster Bar").GetComponent<Image>();
		}

		void Start() {

		}

		void Update() {

			UpdateSecondaryMotion();
			UpdateSpeed();
			UpdateRoll();
			UpdateYaw();
			UpdatePitch();
		}

		private void UpdateSecondaryMotion() {

			Vector3 newPosition = Vector3.zero;

			if (Input.GetKey(VerticalUp)) {
				newPosition.y += Time.deltaTime * Thrust / 2f;
			}
			if (Input.GetKey(VerticalDown)) {
				newPosition.y -= Time.deltaTime * Thrust / 2f;
			}
			if (Input.GetKey(HorizontalRight)) {
				newPosition.x += Time.deltaTime * Thrust / 2f;
			}
			if (Input.GetKey(HorizontalLeft)) {
				newPosition.x -= Time.deltaTime * Thrust / 2f;
			}

			transform.position = transform.position + transform.TransformDirection(newPosition);
		}

		private void UpdateSpeed() {

			if (Input.GetKey(ThrottleUp)) {
				CurrentSpeed += Thrust * Time.deltaTime;
			}
			if (Input.GetKey(ThrottleDown)) {
				CurrentSpeed -= Braking * Time.deltaTime;
			}

			ThrusterBarImage.fillAmount = CurrentSpeed / MaxSpeed;
			if (CurrentSpeed > MaxSpeed)
				CurrentSpeed = MaxSpeed;
			else if (CurrentSpeed < 0)
				CurrentSpeed = 0f;

			transform.position = transform.position + transform.TransformDirection(new Vector3(0, 0, CurrentSpeed * Time.deltaTime));
		}

		private void UpdateRoll() {

			if (Input.GetKey(RollLeft)) {
				transform.Rotate(new Vector3(0f, 0f, Time.deltaTime * RollTopSpeed));
			}
			if (Input.GetKey(RollRight)) {
				transform.Rotate(new Vector3(0f, 0f, Time.deltaTime * -RollTopSpeed));
			}
		}

		private void UpdateYaw() {

			if (Input.GetKey(YawRight)) {
				transform.Rotate(new Vector3(0f, Time.deltaTime * (PitchTopSpeed / 3f), 0f));
			}
			if (Input.GetKey(YawLeft)) {
				transform.Rotate(new Vector3(0f, Time.deltaTime * -(PitchTopSpeed / 3f), 0f));
			}
		}

		private void UpdatePitch() {

			if (Input.GetKey(PitchUp)) {
				transform.Rotate(new Vector3(Time.deltaTime * -(PitchTopSpeed - CurrentSpeed / 6f), 0f, 0f));
			}
			if (Input.GetKey(PitchDown)) {
				transform.Rotate(new Vector3(Time.deltaTime * (PitchTopSpeed + CurrentSpeed / 6f), 0f, 0f));
			}
		}
	}

}