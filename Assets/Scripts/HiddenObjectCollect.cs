using UnityEngine;
using System.Collections;

public class HiddenObjectCollect : MonoBehaviour {
	public int Points;

	private GameObject globalGameManager;
	private bool found = false;
	public float Range = 20.0F;

	void Start() {
		try {
			globalGameManager = GameObject.Find("/GlobalGameManager");

		}
		catch (System.Exception ) {
		}
	}

	void Update(){

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, Range)) {

			if (hit.transform == transform && !found) {

				if (Input.GetMouseButtonUp (0)) {
					found = true;
					globalGameManager.SendMessage ("increasePoints", Points);
					gameObject.GetComponent<BoxCollider> ().isTrigger = false;
					Material tempMaterial = new Material(gameObject.GetComponent<MeshRenderer> ().sharedMaterial);
					Color tempColor = tempMaterial.color;
					tempColor.a = 1.0f;
					tempMaterial.color = tempColor;
					gameObject.GetComponent<MeshRenderer> ().sharedMaterial = tempMaterial;
					AudioSource[] audioSources = globalGameManager.GetComponents<AudioSource> ();
					if (audioSources[2] != null && 
						!audioSources[2].isPlaying &&
						audioSources[2].clip != null) {
						audioSources[2].Play ();
					}
				}

			}

		}
	}

	void setPoints(int pointsToAdd) {
		Points = pointsToAdd;
	}
}
