using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class ShowQuestion : MonoBehaviour {
    // INSPECTOR SETTINGS
	[Tooltip("The range from which the question is shown")]
	public float Range = 4.0F;
	[Tooltip("The answer to the question")]
	public string Answer = "";
	[HideInInspector]
	public string stringToEdit = "";

	private bool inReach = false;
	[HideInInspector]
	public bool isAnswered = false;
	public bool isActive = false;
	private bool isCorrectShown = false;
	private GameObject lastAnswerText;
	private GameObject fpsController;
	private GameObject previousPlayer;
	private GameObject gameManager;
	private bool previousPlayerSaved = false;
	private GameObject emptyGameObject;
	private bool isBoxShown = false;
	private GameObject globalGameManager; //BB, 20170828

	void Start() {
        
		lastAnswerText = gameObject.transform.Find ("AnswerText").gameObject;
		fpsController = GameObject.Find ("/CustomFPSController");
		gameManager = this.gameObject.transform.parent.parent.Find ("GameManager").gameObject;
		emptyGameObject = new GameObject ();
		try {
			globalGameManager = GameObject.Find("/GlobalGameManager"); //BB, 20170828

		}
		catch (System.Exception ) {
		}
	}

	void Update(){
		if (!isAnswered && Answer != "") {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Range)) {
				if (hit.transform == transform) {
					//if (!previousPlayerSaved) {
					//	previousPlayer = gameManager.GetComponent<GameManager> ().currentlySelectedPlayer;
					//	if (previousPlayer == null) {
					//		previousPlayer = fpsController;
					//	}
					//	previousPlayerSaved = true;
					//}
					inReach = true;
					return;
				}

			}
		}
		inReach = false;
		if (previousPlayerSaved) {
			if (gameManager.GetComponent<GameManager> ().previousSelectedPlayer != null) {
				gameManager.SendMessage ("changePlayer", gameManager.GetComponent<GameManager> ().previousSelectedPlayer);
			} else {
				gameManager.SendMessage ("changePlayer", fpsController);
			}
		}
		previousPlayerSaved = false;
		isBoxShown = false;
		stringToEdit = "";
	}

	void OnGUI ()
	{
         TextManager.GetInstance(); //BB, 20171208

		if (inReach == true && !isAnswered)
		{
			GUI.color = Color.white;
			Rect guiBox, answerBox;
			float x, y;
			if (Input.mousePosition.x + 20 + 400 > Screen.width) {
				x = Screen.width - 400;
			} else {
				x = Input.mousePosition.x + 20;

			}
			if (Input.mousePosition.y + 100 > Screen.height) {
				y = Screen.height - 50;
			} else {
				y = Input.mousePosition.y + 20;
			}
			guiBox = new Rect (x, y - 30, 400, 30);
			answerBox = new Rect (x, y, 400, 30);


			if (!isActive) {
                GUI.Box(guiBox, TextManager.Get("solvePuzzle____")); //"Solve the game puzzle to unlock the question."
			} else {
				if (!previousPlayerSaved) {
					previousPlayerSaved = true;
				}
				gameManager.SendMessage ("changePlayer", emptyGameObject);
                GUI.Box(guiBox, TextManager.Get("enterAnswer____")); //"Enter the answer to the question."
				if (!isBoxShown) {
					StartCoroutine (ShowBoxAfter (1.5f));
				}

				AudioSource[] audioSources = globalGameManager.GetComponents<AudioSource> ();

				if (isBoxShown) {
					if (!isCorrectShown) {
						GUI.SetNextControlName ("textbox");
						stringToEdit = GUI.TextField (new Rect (guiBox.x, Input.mousePosition.y + 60, 200, 25), stringToEdit);
						GUI.FocusControl ("textbox");
						if (stringToEdit == Answer) {
							//isAnswered = true;
							isCorrectShown = true;
							lastAnswerText.GetComponent<TextMesh> ().text = TextManager.Get("lastAnswer_____") + " " + stringToEdit + TextManager.Get("rightAnswer____");
							StartCoroutine (DisapearBoxAfter (1.5f));
						} else if (stringToEdit != "") {
                            lastAnswerText.GetComponent<TextMesh>().text = TextManager.Get("lastAnswer_____") + " " + stringToEdit + TextManager.Get("wrongAnswer____");
                            GUI.Box(answerBox, TextManager.Get("tryAgain_______"));

							if (audioSources[3] != null && 
								!audioSources[3].isPlaying &&
								audioSources[3].clip != null) {
								audioSources[3].Play ();
							}
						}
					} else {
						GUI.Box (new Rect (guiBox.x, Input.mousePosition.y + 60, 200, 25), Answer);
					}
				}

				if (isCorrectShown) {
                    GUI.Box(answerBox, TextManager.Get("correctAnswer__")); //"Correct answer!"
					if (audioSources[4] != null && 
						!audioSources[4].isPlaying &&
						audioSources[4].clip != null) {
						audioSources[4].Play ();
					}
				}
			}

		}
	}

	IEnumerator DisapearBoxAfter(float waitTime) { 
		// suspend execution for waitTime seconds
		yield return new WaitForSeconds(waitTime);
		isCorrectShown = false;
		isAnswered = true;
	}

	IEnumerator ShowBoxAfter(float waitTime) { 
		// suspend execution for waitTime seconds
		yield return new WaitForSeconds(waitTime);
		isBoxShown = true;
	}

}