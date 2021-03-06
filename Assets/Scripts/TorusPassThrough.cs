﻿using UnityEngine;
using System.Collections;

public class TorusPassThrough : MonoBehaviour {
	
	public GameObject ball;
	private CustomBallUserControl isSelectedBall;
	private GameObject fpsController;
	private GameObject gameManager;
	private GameObject globalGameManager; //BB, 20170828

	// Use this for initialization
	void Start () {
		if (ball != null) {
			isSelectedBall = ball.GetComponent<CustomBallUserControl> ();
		}
		try {
			fpsController = GameObject.Find ("CustomFPSController");
			gameManager = this.gameObject.transform.parent.parent.parent.Find ("GameManager").gameObject;
			globalGameManager = GameObject.Find("/GlobalGameManager"); //BB, 20170828
		}
		catch (System.Exception ) {
		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {

		if (other.gameObject == ball) { 	
			isSelectedBall.isSelected = false;
			isSelectedBall.isActive = false;
			ball.transform.parent.gameObject.transform.Find("Board").Find ("Spotlight").gameObject.SetActive (false);
			setEmission (ball, new Color(0.25f, 0.25f, 0.25f));
			ball.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			ball.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
			fpsController.GetComponent<CustomFirstPersonController> ().isSelected = true;
			ball.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
			if (gameManager != null) {
				gameManager.GetComponent<GameManager> ().currentlySelectedPlayer = null;
			}
			AudioSource[] audioSources = globalGameManager.GetComponents<AudioSource> (); //BB, 20170828
			if (audioSources[5] != null && 
				!audioSources[5].isPlaying &&
				audioSources[5].clip != null) {
				audioSources[5].Play ();
			}
		}
	}

	void setEmission(GameObject player, Color color) {
		Material tempMaterial = new Material (player.GetComponent<MeshRenderer> ().sharedMaterial);
		tempMaterial.EnableKeyword ("_EMISSION");
		tempMaterial.SetColor("_EmissionColor", color);
		player.GetComponent<MeshRenderer> ().sharedMaterial = tempMaterial;

	}
}
