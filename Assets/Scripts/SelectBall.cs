using UnityEngine;
using System.Collections;

public class SelectBall : MonoBehaviour {

	[Tooltip("The range from which the ball can be selected")]
	public float Range = 10.0F;
	private bool inReach;
	public int minPoints = 0;
	private bool minPointsReached = false;

	private GameObject gameManager;
	private GameObject globalGameManager;

	void Start() {
		try {
			gameManager = this.gameObject.transform.parent.parent.parent.Find ("GameManager").gameObject;
			globalGameManager = GameObject.Find("/GlobalGameManager");
		}
		catch (System.Exception ) {
		}
	}

	public void setMinPoints (int minPointsToSet)
	{
		minPoints = minPointsToSet;
	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.GetComponent<CustomBallUserControl> ().isActive && !gameObject.GetComponent<CustomBallUserControl> ().isSelected) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Range)) {

				if (hit.transform == transform) {
					inReach = true;

                    //if (minPoints > 0 && globalGameManager.GetComponent<GlobalGameManager>().Points < minPoints)
                    if (minPoints > 0 && GlobalGameManager.Points < minPoints) {
                        minPointsReached = false;
					} else {
						minPointsReached = true;
					}

					if (Input.GetMouseButtonUp (0))
						gameManager.SendMessage ("changePlayer", gameObject);
				}

				return;
				

			}
		}
		inReach = false;
	}

	void OnGUI ()
	{
        TextManager.GetInstance(); //BB, 20171208

		if (inReach == true)
		{
			GUI.color = Color.white;
			Rect guiBox;
			if (Input.mousePosition.x + 20 + 400 > Screen.width) {
				guiBox = new Rect (Screen.width - 400 , Input.mousePosition.y + 20, 400, 55);
			} else {
				guiBox = new Rect (Input.mousePosition.x + 20, Input.mousePosition.y + 20, 400, 55);
			}
			if (minPointsReached) {
                GUI.Box(guiBox, TextManager.Get("clickBall1_____") + "\n" + TextManager.Get("clickBall2_____") + "\n" + TextManager.Get("clickBall3_____")); //"Click to select the ball.\nUse the arrow keys to move it around and find the matching target.\nPress Esc to regain control."
			} else {
                GUI.Box(guiBox, TextManager.Get("youNeed2Collect") + " " + minPoints.ToString() + TextManager.Get("points2Unlock__"));
			}
		}
	}
}
