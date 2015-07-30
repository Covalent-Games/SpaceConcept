using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Client {
	public class PlayerAiming : MonoBehaviour {



		private PlayerShipAction _playerShipAction;


		void Awake() {

			Cursor.visible = false;


			_playerShipAction = GetComponent<PlayerShipAction>();

		}

		// Update is called once per frame
		void Update() {


		}


	}


}