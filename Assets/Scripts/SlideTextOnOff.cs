using UnityEngine;
using System.Collections;

public class SlideTextOnOff : MonoBehaviour {

	private bool inReach;
	[Tooltip("The range from which the door can be opened")]
	public float Range = 7.0F;
	//public string defaultBackground = "marble-black";

 	void Update(){

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, Range)) {
			//print ("hit.transform.name="+hit.transform.name);
			//print ("transform.name="+transform.name);
			if (hit.transform == transform) {

				inReach = true;

				return;

			}

		}
		inReach = false;
	}

	void OnGUI ()
	{
		//print("gameObject: " + gameObject);
		//print("gameObject.transform: " + gameObject.transform);
		//print("gameObject.transform.parent: " + gameObject.transform.parent);
		//print("gameObject.transform.parent.gameObject: " + gameObject.transform.parent.gameObject);
		//print("gameObject.transform.parent.gameObject.GetComponent<MeshRenderer> (): " + gameObject.transform.parent.gameObject.GetComponent<MeshRenderer> ());
		//print("gameObject.transform.parent.gameObject.GetComponent<MeshRenderer> ().sharedMaterials [1]: " + gameObject.transform.parent.gameObject.GetComponent<MeshRenderer> ().sharedMaterials [1]);
		//print("gameObject.transform.parent.gameObject.GetComponent<MeshRenderer> ().sharedMaterials [1].mainTexture.name: " + gameObject.transform.parent.gameObject.GetComponent<MeshRenderer> ().sharedMaterials [1].mainTexture.name);
		//if (gameObject.transform.parent.gameObject.GetComponent<MeshRenderer> ().sharedMaterials [1].mainTexture != null) {
			//if (!gameObject.transform.parent.gameObject.GetComponent<MeshRenderer> ().sharedMaterials [1].mainTexture.name.Equals(defaultBackground)) {
				//in this case there is a backround picture set for this slide
				//print("mainTexture="+gameObject.transform.parent.gameObject.GetComponent<MeshRenderer> ().sharedMaterials [1].mainTexture.name);
		if (inReach == true) {
			//if (gameObject.GetComponent<TextMesh> ().text != "") { //this means there is valid text set to the slide
			//print(gameObject.GetComponent<MeshRenderer> ().sharedMaterials);
			gameObject.GetComponent<Renderer> ().enabled = true;
		}
		else {
			gameObject.GetComponent<Renderer> ().enabled = false;
		}
	}

}