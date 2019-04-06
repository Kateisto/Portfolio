using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Position : MonoBehaviour {

	[SerializeField]
	private GameObject _player;
	private Player_Movement _playerScript;
	private Quaternion _rotate;
	Vector3 playerPosition;
	Vector3 camLookDirection;

	[SerializeField]
	private GameObject _pauseMenuObj;
	private PauseMenu _pauseMenu;
	private bool _onPause;

	void Update()
	{
		_pauseMenu = _pauseMenuObj.GetComponent<PauseMenu> ();

		_onPause = _pauseMenu._paused;

		Vector3 playerPosition = GameObject.Find ("Player").transform.position;

		_playerScript = _player.GetComponent<Player_Movement> ();

		_rotate = _playerScript.GetAngleRotation();

		transform.position = (_player.transform.position + _rotate * Vector3.forward * -6f + Vector3.up * 6f);

		if (_onPause == false)
		{
			Vector3 camLookDirection = (playerPosition - transform.position).normalized * Time.deltaTime;

			transform.rotation = Quaternion.LookRotation (camLookDirection);
		}
	}
}
