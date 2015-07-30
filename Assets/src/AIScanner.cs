using UnityEngine;
using System.Collections;

public class AIScanner : MonoBehaviour {

	// This should be a generic AI script for universal targeting.
	private WanderProbe _wanderProbe;

	void Start() {

		_wanderProbe = transform.parent.GetComponent<WanderProbe>();
	}

	void OnTriggerEnter(Collider collider) {

		if (collider.CompareTag("Player")) {
			_wanderProbe.SelectTarget(collider.gameObject);
		}
	}


}
