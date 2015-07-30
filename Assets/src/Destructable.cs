using UnityEngine;
using System.Collections;

namespace Client {
	public class Destructable : MonoBehaviour {

		public string Name;
		[SerializeField]
		private int _health;
		[SerializeField]
		private int _maxHealth;

		/// <summary>
		/// Do not modify this directly. Call ModifyHealth.
		/// </summary>
		public int Health {
			get {
				return this._health;
			}
			set {
				_health = value;
				if (_health > _maxHealth) {
					_health = _maxHealth;
				} else if (_health <= 0) {
					Destroy();
				}
			}
		}
		public int MaxHealth {
			get {
				return _maxHealth;
			}
			protected set {
				_maxHealth = value;
			}
		}

		public delegate void ValueModified(int change, GameObject whoDealtIt);
		public event ValueModified OnHealthChanged;

		void Start() {

			// For players this won't work, as it will just display their GameObject name.
			Name = name;

			MaxHealth = _maxHealth;
			_health = MaxHealth;
		}

		/// <summary>
		/// Changes the health of the object. Use negative numbers to decrease.
		/// </summary>
		/// <param name="change">The int to modify health by.</param>
		/// <param name="owner">The GameObject responsible for the change.</param>
		/// <returns>The new health value</returns>
		public int ModifyHealth(int change, GameObject responsible) {

			int previousHealth = _health;
			Health += change;
			if (OnHealthChanged != null)
				OnHealthChanged(_health - previousHealth, responsible);
			return Health;
		}

		public bool SafeForRespawn = false;

		internal virtual void Destroy() {

			if (SafeForRespawn) {
				Respawn();
			} else if (tag != "Player") {
				gameObject.SetActive(false);
			}
		}

		private void Respawn() {

			_health = _maxHealth;
			gameObject.SetActive(true);
			Debug.Log(gameObject.name + " respawned");
		}
	}

}