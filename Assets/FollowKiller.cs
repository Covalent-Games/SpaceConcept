using UnityEngine;
using System.Collections;

public class FollowKiller : MonoBehaviour {

	public GameObject Killer;
	private Camera _camera;

	void Awake() {

		_camera = GetComponent<Camera>();
		enabled = false;
	}

	public void Enable(GameObject killer) {

		Killer = killer;
		transform.parent = Killer.transform;
		enabled = true;
	}
	// Update is called once per frame
	void Update() {

		transform.position = Killer.transform.position + new Vector3(15, 5, 0);
		transform.LookAt(Killer.transform);
	}
}
