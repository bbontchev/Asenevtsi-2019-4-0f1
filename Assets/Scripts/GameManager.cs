using UnityEngine;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour {

	[HideInInspector]
	public ArrayList players = new ArrayList();
	public GameObject fpsCharacter;
	[HideInInspector]
	public GameObject currentlySelectedPlayer = null;
	public GameObject previousSelectedPlayer = null;
	public int hiddenObjectsCount = 0;
	public int hiddenObjectsFound = 0;

	void Start(){
		IEnumerable playersArray = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "Ball");
		IEnumerator ie = playersArray.GetEnumerator ();
		try {
			while (ie.MoveNext()) {
				if (((GameObject)ie.Current).transform.parent.gameObject.transform.parent != null &&
					((GameObject)ie.Current).transform.parent.gameObject.transform.parent.transform.parent !=null &&
					((GameObject)ie.Current).transform.parent.gameObject.transform.parent.transform.parent == this.transform.parent) {
					players.Add (ie.Current);
				}
			}
		}
		catch (System.Exception ) {
		}
		hiddenObjectsCount = gameObject.transform.parent.Find ("HiddenObjects").childCount;
	}

	// Update is called once per frame
	void Update () {
		if (currentlySelectedPlayer != null && fpsCharacter != null && !fpsCharacter.GetComponent<CustomFirstPersonController> ().isSelected) {
			if (Input.GetKey (KeyCode.Escape)) {
				//reset to the fps character
				fpsCharacter.GetComponent<CustomFirstPersonController> ().isSelected = true;
				previousSelectedPlayer = currentlySelectedPlayer;
				currentlySelectedPlayer = null;

				for (int i = 0; i < players.Count; i++) {
					((GameObject)players [i]).GetComponent<CustomBallUserControl> ().isSelected = false;
					((GameObject)players [i]).transform.parent.gameObject.transform.Find("Board").Find ("Spotlight").gameObject.SetActive (false);
					setEmission ((GameObject)players [i], Color.black);
				}
			}
		}

		bool activeQuestionRemains = false;
		for (int i = 0; i < players.Count; i++) {
			if (((GameObject)players [i]).GetComponent<CustomBallUserControl> ().isActive) {
				activeQuestionRemains = true;
				break;
			}
		}

		if (!activeQuestionRemains) {
			//unlock questions for doors
			GameObject questionsCollection = gameObject.transform.parent.gameObject.transform.Find("Questions").gameObject;
			if (questionsCollection.transform.Find ("QuestionN").gameObject.activeSelf) {
				questionsCollection.transform.Find ("QuestionN").gameObject.GetComponent<ShowQuestion> ().isActive = true;
			}

			if (questionsCollection.transform.Find ("QuestionE").gameObject.activeSelf) {
				questionsCollection.transform.Find ("QuestionE").gameObject.GetComponent<ShowQuestion> ().isActive = true;
			}
			if (questionsCollection.transform.Find ("QuestionW").gameObject.activeSelf) {
				questionsCollection.transform.Find ("QuestionW").gameObject.GetComponent<ShowQuestion> ().isActive = true;
			}
			if (questionsCollection.transform.Find ("QuestionS").gameObject.activeSelf) {
				questionsCollection.transform.Find ("QuestionS").gameObject.GetComponent<ShowQuestion> ().isActive = true;
			}
				
		}
	}

	void changePlayer(object currentPlayer){
		previousSelectedPlayer = currentlySelectedPlayer;

		if (currentPlayer.Equals (fpsCharacter)) {
			fpsCharacter.GetComponent<CustomFirstPersonController> ().isSelected = true;
			currentlySelectedPlayer = null;

			for (int i = 0; i < players.Count; i++) {
				((GameObject)players [i]).GetComponent<CustomBallUserControl> ().isSelected = false;
				((GameObject)players [i]).transform.parent.gameObject.transform.Find("Board").Find ("Spotlight").gameObject.SetActive (false);
				setEmission ((GameObject)players [i], Color.black);
			}
		} else {
			fpsCharacter.GetComponent<CustomFirstPersonController> ().isSelected = false;
			int currentPlayerIndex = players.IndexOf (currentPlayer);
			if (currentPlayerIndex >= 0) {
				currentlySelectedPlayer = (GameObject)players [currentPlayerIndex];
				((GameObject)players [currentPlayerIndex]).GetComponent<CustomBallUserControl> ().isSelected = true;
				((GameObject)players [currentPlayerIndex]).transform.parent.gameObject.transform.Find("Board").Find ("Spotlight").gameObject.SetActive (true);
				setEmission ((GameObject)players [currentPlayerIndex], Color.blue);
				fpsCharacter.GetComponent<CustomFirstPersonController> ().isSelected = false;
			}

			for (int i = 0; i < players.Count; i++) {
				if (i != currentPlayerIndex) {
 					((GameObject)players [i]).GetComponent<CustomBallUserControl> ().isSelected = false;
					((GameObject)players [i]).transform.parent.gameObject.transform.Find("Board").Find ("Spotlight").gameObject.SetActive (false);
					setEmission ((GameObject)players [i], Color.black);
				}
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
