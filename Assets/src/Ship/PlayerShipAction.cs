using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Client {
	public class PlayerShipAction : MonoBehaviour {

		public static Destructable Target;
		public int Range = 500;
		[Tooltip("Shots per second")]
		public int RateOfFire = 10;
		public LayerMask TargetingLayerMask;
		public Canvas TargetUI;
		public Light CabinLight;
		public Light WarningLight;
		[HideInInspector]
		public Image TargetHealthBar;
		[HideInInspector]
		public Text TargetName;
		[HideInInspector]
		public Text TargetDistance;
		[HideInInspector]
		public RectTransform TargetingArrow;

		[SerializeField]
		private Image HealthBar;
		[SerializeField]
		private ParticleSystem[] CabinSparks;
		private PlayerShipControl _playerShipControl;
		private Destructable _destructable;
		private GameObject ProjectileSpawnPoint;
		private GameObject Projectile;
		private Image _reticle;
		private GameObject _turret;
		private float ShotTimer = 0f;

		private List<GameObject> ProjectilePool = new List<GameObject>();

		void Awake() {

			Cursor.visible = false;
			_reticle = GameObject.Find("Reticle").GetComponent<Image>();
			_turret = GameObject.Find("turret_tmp");
			_playerShipControl = GetComponent<PlayerShipControl>();
			_destructable = GetComponent<Destructable>();
			ProjectileSpawnPoint = GameObject.Find("PProjectileSpawnPoint");
			Projectile = (GameObject)Resources.Load("Projectiles/Projectile");
			TargetName = TargetUI.transform.FindChild("TargetName").GetComponent<Text>();
			TargetDistance = TargetUI.transform.FindChild("TargetDistance").GetComponent<Text>();
			TargetingArrow = GameObject.Find("TargetIndicator").GetComponent<RectTransform>();

			_destructable.OnHealthChanged += UpdateHealthBar;

		}

		internal void SetTargetUI(Destructable newTarget) {

			TargetUI.GetComponent<CanvasGroup>().alpha = 1f;
			TargetHealthBar.fillAmount = newTarget.Health / (float)newTarget.MaxHealth;
			TargetName.text = newTarget.Name;
			TargetDistance.text = string.Format(
				"{0}m", (int)Vector3.Distance(transform.position, newTarget.transform.position));
		}

		void UpdateHealthBar(int change, GameObject guilty) {

			HealthBar.fillAmount = _destructable.Health / (float)_destructable.MaxHealth;
			if (HealthBar.fillAmount < 0.25f) {
				StartCoroutine(PulseWarningLightRoutine());
			}

			if (change < 0) {
				CabinSparks[Random.Range(0, CabinSparks.Length)].Play();
			}

			if (_destructable.Health <= 0) {
				Camera.main.GetComponent<FollowKiller>().Enable(guilty);
				_reticle.gameObject.SetActive(false);
				gameObject.SetActive(false);
			}
		}

		void Update() {

			ShotTimer += Time.deltaTime;
			if (Input.GetKey(KeyCode.Mouse0) && ShotTimer > 1f / RateOfFire) {
				DoFireGroupOne();
				ShotTimer = 0f;
			}

			if (PlayerShipAction.Target != null) {
				UpdateTargetUI();
			}

			if (Input.GetKeyDown(_playerShipControl.SelectCenteredTarget)) {
				GetNewTarget();
			}

			Aim();
		}

		IEnumerator PulseWarningLightRoutine() {

			float cabinLightIntensity = CabinLight.intensity;
			CabinLight.intensity = 0f;
			float lightIntensity = 0f;
			while (HealthBar.fillAmount < 0.25f) {
				lightIntensity = (lightIntensity + Time.deltaTime * 8) % 5f;
				WarningLight.intensity = lightIntensity;
				yield return null;
			}
			WarningLight.intensity = 0f;
			CabinLight.intensity = cabinLightIntensity;
		}

		private void Aim() {

			_reticle.transform.position = Input.mousePosition;
			RaycastHit hitinfo;

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hitinfo, Range, TargetingLayerMask.value)) {
				_turret.transform.LookAt(hitinfo.point, transform.up);
			} else {
				_turret.transform.LookAt(ray.GetPoint(Range), transform.up);
			}

			if (Target != null) {
				Vector3 targetPos = Camera.main.WorldToViewportPoint(Target.transform.position);
				Vector3 cursorPos = Camera.main.ScreenToViewportPoint(_reticle.transform.position);

				Vector3 direction = cursorPos - targetPos;
				float angle = Mathf.Atan(direction.y / direction.x) * Mathf.Rad2Deg - 90;
				if (direction.x > 0) {
					angle += 180;
				}
				if (targetPos.z < 0) {
					angle -= 180;
				}
				Vector3 rotation = TargetingArrow.rotation.eulerAngles;
				rotation.z = angle;
				TargetingArrow.rotation = Quaternion.Euler(rotation);


			}
		}

		private void GetNewTarget() {

			// TODO: This should be a "static" list modified on instantiation of new targetable objects.
			GameObject[] possibleTargets = GameObject.FindGameObjectsWithTag("CanTarget");
			Vector3 reticulePos = Camera.main.ScreenToViewportPoint(_reticle.transform.position);
			Vector3 targetablePos;
			float shortestDist = Range;
			float currentObjectDistance = 0f;
			GameObject likelyTarget = null;

			for (int i = 0; i < possibleTargets.Length; i++) {
				targetablePos = Camera.main.WorldToViewportPoint(possibleTargets[i].transform.position);
				// Check if this object is in the screen.
				if (targetablePos.x > 0 && targetablePos.x < 1 && targetablePos.y > 0 && targetablePos.y < 1) {
					// Calculate screenspace distance from targeting reticule;
					currentObjectDistance = Vector3.Distance(targetablePos, reticulePos);
					// Check if this object is closer to the reticule than a previous object
					if (currentObjectDistance < Range) {
						likelyTarget = possibleTargets[i];
						shortestDist = currentObjectDistance;
					}
				}
			}
			if (likelyTarget != null) {
				var destructable = likelyTarget.GetComponent<Destructable>();
				Target = destructable;
				SetTargetUI(destructable);
			}
		}

		void DoFireGroupOne() {

			bool fired = false;
			GameObject projectile;
			for (int i = 0; i < ProjectilePool.Count; i++) {
				projectile = ProjectilePool[i];
				if (!projectile.activeSelf) {
					BaseProjectile baseProjectile = projectile.GetComponent<BaseProjectile>();
					projectile.transform.position = ProjectileSpawnPoint.transform.position;
					projectile.transform.rotation = ProjectileSpawnPoint.transform.rotation;
					baseProjectile.Range = Range;
					baseProjectile.Trail.time = 0f;
					projectile.SetActive(true);
					fired = true;
					return;
				}
			}
			if (!fired) {
				GameObject newProjectile = (GameObject)Instantiate(Projectile);
				newProjectile.GetComponent<BaseProjectile>().Owner = gameObject;
				newProjectile.SetActive(false);
				ProjectilePool.Add(newProjectile);
				DoFireGroupOne();
			}
		}

		private void UpdateTargetUI() {

			// Can this reasonably and safely be registered to an event?
			float healthPercent = PlayerShipAction.Target.Health / (float)PlayerShipAction.Target.MaxHealth;
			if (healthPercent <= 0) {
				Target = null;
				TargetHealthBar.fillAmount = 1f;
				TargetName.text = "";
				TargetDistance.text = "";
				TargetUI.GetComponent<CanvasGroup>().alpha = 0.2f;
				return;
			}
			TargetHealthBar.fillAmount = healthPercent;

			TargetDistance.text =
				string.Format(
					"{0}m",
					(int)Vector3.Distance(transform.position, PlayerShipAction.Target.transform.position));
		}

		void OnCollisionEnter(Collision collision) {

			Debug.Log("crash");
			if (collision.gameObject.tag == "Environment") {
				_destructable.ModifyHealth(
					(int)-collision.gameObject.GetComponent<Rigidbody>().mass * 3, collision.gameObject);
			}
		}
	}
}
