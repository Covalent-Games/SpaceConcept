using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public GameObject AsteroidContainer;
	public GameObject AsteroidPrefab;
	public int AsteroidCount;
	public int SpawnRadias;
	public float ScaleLimit;

	void Awake() {

		for (int i = 0; i < AsteroidCount; i++) {
			GameObject asteroid = Instantiate(AsteroidPrefab);
			asteroid.transform.position = new Vector3(
				Random.Range(-SpawnRadias, SpawnRadias),
				Random.Range(-SpawnRadias, SpawnRadias),
				Random.Range(-SpawnRadias, SpawnRadias));
			asteroid.transform.rotation = Quaternion.Euler(new Vector3(
				Random.Range(0, 360),
				Random.Range(0, 360),
				Random.Range(0, 360)));
			asteroid.transform.localScale = new Vector3(
				Random.Range(1, ScaleLimit),
				Random.Range(1, ScaleLimit),
				Random.Range(1, ScaleLimit));
			asteroid.transform.parent = AsteroidContainer.transform;
			asteroid.GetComponent<Rigidbody>().mass =
				(asteroid.transform.localScale.x + asteroid.transform.localScale.y + asteroid.transform.localScale.z) / 3f;
		}

		GameObject.Find("Drone").transform.position = new Vector3(
			Random.Range(-SpawnRadias, SpawnRadias),
			Random.Range(-SpawnRadias, SpawnRadias),
			Random.Range(-SpawnRadias, SpawnRadias));
	}

	// Update is called once per frame
	void Update() {

		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
		if (Input.GetKeyDown(KeyCode.Return)) {
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}
