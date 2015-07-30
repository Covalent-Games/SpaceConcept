using UnityEngine;
using System.Collections.Generic;
using Client;
using System.Collections;

public class WanderProbe : MonoBehaviour {

	public float Speed = 20;
	public float MaxSpeed = 20;
	public int ShotVelocity = 200;
	public int RateOfFire = 1;
	public int Damage = 3;
	public int ShotRange = 100;
	public float RotationSpeed = 35f;
	public int DistancePref = 20;
	public GameObject Projectile;
	public GameObject DestinationMarker;

	[SerializeField]
	private Transform _projectileSpawnPoint;
	[SerializeField]
	private Light Spotlight;
	[SerializeField]
	private Light LeftEngine;
	[SerializeField]
	private Light RightEngine;
	private Vector3 _lastTargetPosition;
	[SerializeField]
	private List<GameObject> _projectilePool = new List<GameObject>();

	private Vector3 Destination = new Vector3(0, 0, 50);
	private float WanderDistance = 100f;
	private Destructable _target;
	private Camera _viewPort;
	private float _shotTimer = 0f;
	private float _currentRotationSpeed = 0f;
	private float _currentRotationVelocity = 0f;
	private float _accuracy = -3f;


	private bool Surpise = true;

	void Start() {

		DestinationMarker.transform.position = Destination;
		_viewPort = transform.FindChild("ViewPort").GetComponent<Camera>();
		_projectileSpawnPoint = transform.FindChild("ProjectileSpawnPoint");
		GetComponent<Destructable>().OnHealthChanged += Retaliate;

	}

	// Update is called once per frame
	void Update() {

		MoveForward();

		if (_target != null && _target.gameObject.activeSelf) {
			//if (Vector3.Distance(transform.position, Destination) < 20) {
			//	Speed = Mathf.MoveTowards(Speed, 2, 7 * Time.deltaTime);
			//} else 
			Spotlight.intensity = 1f;
			if (Speed != MaxSpeed) {
				Speed = Mathf.MoveTowards(Speed, MaxSpeed, 5 * Time.deltaTime);
			}
			if (_target.Health <= 0) {
				_target = null;
			}
			Vector3 pos = _target.transform.position - (_target.transform.forward * DistancePref);
			Destination = pos;

			// Shooting should be the last process in case this shot kills the player.
			_shotTimer += Time.deltaTime;
			if (_shotTimer > 1f / RateOfFire) {
				ShootAtTarget();
			}

		} else {
			if (!Surpise) { return; }
			Speed = Mathf.MoveTowards(Speed, MaxSpeed / 2, 5 * Time.deltaTime);
			if (Vector3.Distance(transform.position, Destination) < 10) {
				GetNewDestination();
			}
		}

		// Increase engine brightness based on speed.
		float engineIntensity = (Speed / (float)MaxSpeed) * 4;
		LeftEngine.intensity = engineIntensity;
		RightEngine.intensity = engineIntensity;
	}

	private void Retaliate(int change, GameObject target) {

		_target = target.GetComponent<Destructable>();
	}

	private void ShootAtTarget() {

		Vector3 viewPos = _viewPort.WorldToViewportPoint(_target.transform.position);
		if (viewPos.x < 1 && viewPos.x > 0 && viewPos.y < 1 && viewPos.y > 0 && viewPos.z > 0) {
			if (Projectile) {
				bool fired = false;
				GameObject projectile;
				for (int i = 0; i < _projectilePool.Count; i++) {
					projectile = _projectilePool[i];
					if (!projectile.activeSelf) {
						BaseProjectile baseProjectile = projectile.GetComponent<BaseProjectile>();
						projectile.transform.position = _projectileSpawnPoint.transform.position;
						projectile.transform.rotation = _projectileSpawnPoint.transform.rotation;
						baseProjectile.Range = ShotRange;
						baseProjectile.Speed = ShotVelocity;
						baseProjectile.Damage = Damage;
						baseProjectile.Owner = gameObject;
						baseProjectile.transform.LookAt(PredictTargetPosition());
						projectile.SetActive(true);
						fired = true;
						break;
					}
				}
				if (!fired) {
					GameObject newProjectile = (GameObject)Instantiate(Projectile);
					newProjectile.SetActive(false);
					_projectilePool.Add(newProjectile);
					ShootAtTarget();
				}
			} else {
				Debug.LogError(name + " doesn't have a projectile prefab set.");
			}
			_shotTimer = 0f;
		}
	}

	private Vector3 PredictTargetPosition() {

		float deltaDistance = Vector3.Distance(_target.transform.position, _lastTargetPosition);
		float deltaTimeToTarget =
			Vector3.Distance(_target.transform.position, transform.position) / ShotVelocity;
		Vector3 targetFuturePos =
			_target.transform.position + _target.transform.forward * deltaDistance * deltaTimeToTarget;
		_lastTargetPosition = _target.transform.position;

		// Modify location with random values from _accuracy
		targetFuturePos.x += Random.Range(_accuracy, Mathf.Abs(_accuracy));
		targetFuturePos.y += Random.Range(_accuracy, Mathf.Abs(_accuracy));
		targetFuturePos.z += Random.Range(_accuracy, Mathf.Abs(_accuracy));

		return targetFuturePos;
	}

	public void SelectTarget(GameObject target) {

		if (_target == null) {
			_target = target.GetComponent<Destructable>();
			_lastTargetPosition = target.transform.position;
			Surpise = true;
			Spotlight.intensity = 1f;
		}
	}

	private void GetNewDestination() {

		Destination = new Vector3(Random.Range(-WanderDistance, WanderDistance), Random.Range(-WanderDistance, WanderDistance), Random.Range(-WanderDistance + 30, WanderDistance + 30));
		DestinationMarker.transform.position = Destination;
	}

	private void MoveForward() {

		transform.position = transform.position + transform.forward * Speed * Time.deltaTime;
		Quaternion oldRotation = transform.rotation;
		transform.LookAt(Destination);
		Quaternion newRotation = transform.rotation;
		_currentRotationSpeed = Mathf.SmoothDamp(
			_currentRotationSpeed,
			RotationSpeed,
			ref _currentRotationVelocity,
			0.5f);
		transform.rotation = Quaternion.RotateTowards(
			oldRotation,
			newRotation,
			_currentRotationSpeed * Time.deltaTime);
	}
}
