using UnityEngine;
using System.Collections;

public class DoorOpenScript : MonoBehaviour {

	private GameObject globalGameManager; //BB, 20170828

	void Update(){

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, Range)) {

			if (hit.transform == transform && (Question == null || Question != null && Question.GetComponent<ShowQuestion>().isAnswered)) {

				inReach = true;

				if (Input.GetMouseButtonUp (0) && Running == false) {

					StartCoroutine(Open ());
					AudioSource[] audioSources = globalGameManager.GetComponents<AudioSource> ();
					//print("audioSources[0]: "+audioSources[0]);
					//print("audioSources[1]: "+audioSources[1]); //for opening/closing a door
					//print("audioSources[2]: "+audioSources[2]); //for hitting a hidden object
					if (audioSources[1] != null && 
						!audioSources[1].isPlaying &&
						audioSources[1].clip != null) {
						audioSources[1].Play ();
					}
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
			if (Input.mousePosition.x + 20 + 200 > Screen.width) {
				guiBox = new Rect (Screen.width - 200 , Input.mousePosition.y + 20, 200, 55);
			} else {
				guiBox = new Rect (Input.mousePosition.x + 20, Input.mousePosition.y + 20, 200, 55);
			}
            GUI.Box(guiBox, TextManager.Get("clickToOpen1___") + "\n" + TextManager.Get("clickToOpen2___") + "\n" + TextManager.Get("clickToOpen3___")); //"Click to open/close the door" 
		}
	}

	private bool inReach;

	// INSPECTOR SETTINGS
	[Header("Door Settings")]
	[Tooltip("The initial angle of the door/window.")]
	public float InitialAngle = 0.0F;
	[Tooltip("The amount of degrees the door/window rotates.")]
	public float RotationAngle = 90.0F;
	[Tooltip("The range from which the door can be opened")]
	public float Range = 4.0F;

	public enum PositionOfHinge
	{
		Left,
		Right,
	}
	public PositionOfHinge HingePosition;

	public enum SideOfRotation
	{
		Left,
		Right,
	}
	public SideOfRotation RotationSide;

	[Tooltip("Rotating speed of the door/window.")]
	public float Speed = 3F;
	[Tooltip("0 = infinite times")]
	public int TimesMoveable = 0;

	public GameObject Question;

	// PRIVATE SETTINGS
	private int n = 0; //For 'TimesMoveable' loop.
	[HideInInspector] public bool Running = false;

	private bool opened = false;

	// DEBUGGING
	[Header("Debug Settings")]
	[Tooltip("Visualizes the position of the hinge in-game by a colored cube.")]
	public bool VisualizeHinge = false;
	[Tooltip("The color of the visualization of the hinge.")]
	public Color HingeColor = Color.cyan;

	//Define an initial and final rotation.
	private Quaternion FinalRot, InitialRot;
	private int State;

	//Create a hinge.
	GameObject hinge;
			
	void Start ()
	{
		try {
			globalGameManager = GameObject.Find("/GlobalGameManager"); //BB, 20170828

		}
		catch (System.Exception ) {
		}

		//Create a hinge.
		hinge = new GameObject();
		hinge.name = "hinge";

		//Calculate sine/cosine of initial angle (needed for hinge positioning).
		float CosDeg = Mathf.Cos ((transform.eulerAngles.y * Mathf.PI) / 180);
		float SinDeg = Mathf.Sin ((transform.eulerAngles.y * Mathf.PI) / 180);

		//Read transform (position/rotation/scale) of the door.
		float PosDoorX = transform.position.x;
		float PosDoorY = transform.position.y;
		float PosDoorZ = transform.position.z;

		float RotDoorX = transform.localEulerAngles.x;
		float RotDoorZ = transform.localEulerAngles.z;

		float ScaleDoorX = transform.localScale.x;
		float ScaleDoorZ = transform.localScale.z;

		//Create a placeholder of the hinge's position/rotation.
		Vector3 HingePosCopy = hinge.transform.position;
		Vector3 HingeRotCopy = hinge.transform.localEulerAngles;

		// HINGE LEFT
		if (HingePosition == PositionOfHinge.Left)
		{
			// CALCULATE
			if (transform.localScale.x > transform.localScale.z)
			{
				HingePosCopy.x = (PosDoorX - (ScaleDoorX / 2 * CosDeg));
				HingePosCopy.z = (PosDoorZ + (ScaleDoorX / 2 * SinDeg));
				HingePosCopy.y = PosDoorY;

				HingeRotCopy.x = RotDoorX;
				HingeRotCopy.y = -InitialAngle;
				HingeRotCopy.z = RotDoorZ;
			}

			else
			{
				HingePosCopy.x = (PosDoorX + (ScaleDoorZ / 2 * SinDeg));
				HingePosCopy.z = (PosDoorZ + (ScaleDoorZ / 2 * CosDeg));
				HingePosCopy.y = PosDoorY;

				HingeRotCopy.x = RotDoorX;
				HingeRotCopy.y = -InitialAngle;
				HingeRotCopy.z = RotDoorZ;
			}
		}

		// RIGHT
		if (HingePosition == PositionOfHinge.Right)
		{
			// CALCULATE
			if(transform.localScale.x > transform.localScale.z)
			{
				HingePosCopy.x = (PosDoorX + (ScaleDoorX / 2 * CosDeg));
				HingePosCopy.z = (PosDoorZ - (ScaleDoorX / 2 * SinDeg));
				HingePosCopy.y = PosDoorY;

				HingeRotCopy.x = RotDoorX;
				HingeRotCopy.y = -InitialAngle;
				HingeRotCopy.z = RotDoorZ;
			}

			else
			{
				HingePosCopy.x = (PosDoorX - (ScaleDoorZ / 2 * SinDeg));
				HingePosCopy.z = (PosDoorZ - (ScaleDoorZ / 2 * CosDeg));
				HingePosCopy.y = PosDoorY;

				HingeRotCopy.x = RotDoorX;
				HingeRotCopy.y = -InitialAngle;
				HingeRotCopy.z = RotDoorZ;
			}
		}

		// HINGE POSITIONING
		hinge.transform.position = HingePosCopy;
		transform.parent = hinge.transform;
		hinge.transform.localEulerAngles = HingeRotCopy;

		// DEBUGGING
		if (VisualizeHinge == true)
		{
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = HingePosCopy;
			cube.transform.localScale = new Vector3(0.25f,0.5f,0.25f);
			cube.GetComponent<Renderer>().material.color = HingeColor;
		}

		// ERROR CODES
		if (Mathf.Abs(InitialAngle) + Mathf.Abs(RotationAngle) == 180 || Mathf.Abs(InitialAngle) + Mathf.Abs(RotationAngle) > 180)
		{
			//UnityEditor.EditorUtility.DisplayDialog ("Error 001","The difference between 'Initial Angle' and 'Rotation Angle' can't exceed or be equal to 180 degrees.", "Ok", "");
			//UnityEditor.EditorApplication.isPlaying = false;
		}

		// ANGLES
		if (RotationSide == SideOfRotation.Left)
		{
			InitialRot = Quaternion.Euler (0, -InitialAngle, 0);
			FinalRot = Quaternion.Euler(0, -InitialAngle - RotationAngle, 0);
		}

		if (RotationSide == SideOfRotation.Right)
		{
			InitialRot = Quaternion.Euler (0, -InitialAngle, 0);
			FinalRot = Quaternion.Euler(0, -InitialAngle + RotationAngle, 0);
		}
	}

	public IEnumerator Open ()
	{
		if (!opened) {
			opened = true;
			/* change Color */
			Material tempMaterial = new Material (gameObject.transform.Find("porta").GetComponent<MeshRenderer> ().sharedMaterial);
			tempMaterial.color = Color.white;
			gameObject.transform.Find("porta").GetComponent<MeshRenderer> ().sharedMaterial = tempMaterial;
			gameObject.transform.Find("cornice").Find("Cubo").GetComponent<MeshRenderer> ().sharedMaterial = tempMaterial;
			gameObject.transform.Find("cornice").Find("Cubo_1").GetComponent<MeshRenderer> ().sharedMaterial = tempMaterial;
			gameObject.transform.Find("cornice").Find("Cubo_2").GetComponent<MeshRenderer> ().sharedMaterial = tempMaterial;
		}

		if (n < TimesMoveable || TimesMoveable == 0)
		{
			//Change state from 1 to 0 and back (= alternate between FinalRot and InitialRot).
			if (hinge.transform.rotation == (State == 0 ? FinalRot : InitialRot)) State ^= 1;

			//Set 'FinalRotation' to 'FinalRot' when moving and to 'InitialRot' when moving back.
			Quaternion FinalRotation = ((State == 0) ? FinalRot : InitialRot);

			//Make the door/window rotate until it is fully opened/closed.
			while (Mathf.Abs(Quaternion.Angle(FinalRotation, hinge.transform.rotation)) > 0.01f)
			{
				Running = true;
				hinge.transform.rotation = Quaternion.Lerp (hinge.transform.rotation, FinalRotation, Time.deltaTime * Speed);

				yield return new WaitForEndOfFrame();
			}

			Running = false;

			if (TimesMoveable == 0) n = 0;

			else n++;
		}
	}

}