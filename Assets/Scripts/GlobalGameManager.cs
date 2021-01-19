using UnityEngine;
using System.Collections;
using System.Linq;

public class GlobalGameManager : MonoBehaviour {

	public static int Points;
	//private GameObject currentGameManager;
	private GameObject fpsController;
	private int hiddenObjectsCount = 0;
	private int hiddenObjectsFound = 0;
	private bool alreadyPlayed = false;

	void Start() {
		fpsController = GameObject.Find ("CustomFPSController");
		IEnumerable hiddenObjectsArray = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "HiddenObjects");
		IEnumerator ie = hiddenObjectsArray.GetEnumerator ();
		while (ie.MoveNext ()) {
			hiddenObjectsCount += ((GameObject)ie.Current).transform.childCount;
		}
	}

    public static void increasePointsOnly(int pointsToAdd) {
        Points += pointsToAdd;
    }

    void increasePoints(int pointsToAdd) {
		Points += pointsToAdd;
		//if (currentGameManager != null) {
		//	currentGameManager.GetComponent<GameManager> ().hiddenObjectsFound++;
		//}
		hiddenObjectsFound++;
	}

	void setCurrentGameManager(GameObject gameManager) {
		//currentGameManager = gameManager;
	}

	void OnGUI ()
	{
        TextManager.GetInstance(); //BB, 20171208

		GUI.color = Color.white;
        GUI.Box(new Rect(Screen.width - 105, 10, 100, 25), TextManager.Get("points_________") + " " + Points.ToString());

		if (hiddenObjectsCount > 0) {
            GUI.Box(new Rect(Screen.width - 255, 40, 250, 25), TextManager.Get("found__________") + " " + hiddenObjectsFound +
                 TextManager.Get("from___________") + " " + hiddenObjectsCount + TextManager.Get("hiddenObjects__"));
		}

		if (hiddenObjectsCount > 0 && hiddenObjectsCount == hiddenObjectsFound) {
			fpsController.GetComponent<CustomFirstPersonController> ().isSelected = false;
			fpsController.GetComponent<CustomFirstPersonController> ().isStopped = true;

			gameObject.transform.position = fpsController.transform.position;
			if (!alreadyPlayed) {
				alreadyPlayed = true;
				if (gameObject.GetComponent<AudioSource> () != null &&
				   !gameObject.GetComponent<AudioSource> ().isPlaying &&
				   gameObject.GetComponent<AudioSource> ().clip != null) {

					gameObject.GetComponent<AudioSource> ().Play ();
				}
			}

			if (GUI.Button (new Rect (Screen.width / 2 - 120, Screen.height / 2, 100, 30), Resources.Load("ReplayButton") as Texture) ) {
				UnityEngine.SceneManagement.SceneManager.LoadScene (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name);
				DynamicGI.UpdateEnvironment ();
			}
				
			if (GUI.Button (new Rect (Screen.width / 2 + 20, Screen.height / 2, 100, 30), Resources.Load("ExitButton") as Texture) ) {
				if (Application.isEditor) {
					#if UNITY_EDITOR
					UnityEditor.EditorApplication.isPlaying = false;
					#endif
				} else {
					Application.Quit ();
				}
			}

		}
	}
}
