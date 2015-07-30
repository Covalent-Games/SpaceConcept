using UnityEngine;
using System.Collections;

namespace Client {
	public class BaseProjectile : MonoBehaviour {

		public int Damage = 10;
		public int Speed = 200;
		public float Range = 0;
		public GameObject Owner;
		public TrailRenderer Trail;

		private Vector3 StartPosition;

		void OnEnable() {

			if (Trail == null) {
				Trail = GetComponent<TrailRenderer>();
			}
			Trail.time = .05f;
			Trail.enabled = true;
			StartCoroutine(RangeCheckRoutine());
			StartPosition = transform.position;
			GetComponent<Rigidbody>().velocity = transform.forward * Speed;

		}

		IEnumerator RangeCheckRoutine() {

			while (Vector3.Distance(StartPosition, transform.position) < Range) {
				yield return new WaitForSeconds(.15f);
			}
			Trail.enabled = false;
			gameObject.SetActive(false);
		}

		void OnTriggerEnter(Collider collider) {

			Destructable destructable = collider.gameObject.GetComponent<Destructable>();

			if (destructable != null) {
				destructable.ModifyHealth(-Damage, Owner);
				if (!collider.CompareTag("Player")) {
					if (PlayerShipAction.Target == null) {
						PlayerShipAction.Target = destructable;
						Owner.GetComponent<PlayerShipAction>().SetTargetUI(destructable);
					}
				}
			}
			if (collider.CompareTag("Environment")) {
				gameObject.SetActive(false);
			}
		}
	}
}

