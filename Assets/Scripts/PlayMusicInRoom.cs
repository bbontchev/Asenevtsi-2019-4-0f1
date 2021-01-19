using UnityEngine;
using System.Collections;

public class PlayMusicInRoom : MonoBehaviour {
	private bool alreadyPlayed = false;
	private GameObject fpsController;
	private GameObject gameManager;
	private GameObject globalGameManager;

	void Start() {
		try {
			gameManager = transform.Find("GameManager").gameObject;
			fpsController = GameObject.Find ("/CustomFPSController");
			globalGameManager = GameObject.Find ("/GlobalGameManager");
		}
		catch (System.Exception ) {
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.transform == fpsController.transform) {
			if (!alreadyPlayed) {
				alreadyPlayed = true;
				if (gameManager.GetComponent<AudioSource> () != null && 
					!gameManager.GetComponent<AudioSource> ().isPlaying &&
					gameManager.GetComponent<AudioSource> ().clip != null) {
 	
					gameManager.GetComponent<AudioSource> ().Play ();
				}
			}
			globalGameManager.SendMessage ("setCurrentGameManager", gameManager);
		}
		
	}

	void OnTriggerExit(Collider other)
	{
		if (other.transform == fpsController.transform) {
			alreadyPlayed = false;
			if (gameManager.GetComponent<AudioSource> () != null && 
				gameManager.GetComponent<AudioSource> ().clip != null) {

				gameManager.GetComponent<AudioSource> ().Stop ();
			}
		}
	}
}
