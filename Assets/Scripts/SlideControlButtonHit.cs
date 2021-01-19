using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlideSeparator;

public class SlideControlButtonHit : MonoBehaviour {

	private bool hasEnterInDown;
	// Use this for initialization
	void Start () {
		hasEnterInDown = false;
	}

	// Update is called once per frame
	void Update () {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit)) {
			if (hit.transform == transform)  {
				
				string nextSlideText;
				float moveDelta = 0.17f;

				if (Input.GetMouseButtonUp(0)) {
					hasEnterInDown = true;
					Vector3 start = hit.transform.localPosition;
					Vector3 destination = start + new Vector3(0f, moveDelta, 0f);
					hit.transform.localPosition = Vector3.MoveTowards(start, destination, moveDelta);

					Component[] slideMesh = hit.transform.parent.parent.GetComponentsInChildren(typeof(TextMesh));
					Component[] textPortionsComp = hit.transform.parent.parent.GetComponentsInChildren(typeof(SlideTextPortions));
					//string[] textPortions = ((SlideTextPortions)textPortionsComp [0]).textSlidePortions;
					/*for (short k = 0; k < textPortions.GetLength(0); k++) {
						Debug.Log ("Slide No "+k.ToString()+": "+ textPortions[k]);
					}*/

					nextSlideText = GetNextSlideText(hit.transform.name, textPortionsComp, slideMesh);

					Debug.Log ("nextSlideText shown.");
					if (nextSlideText != ((TextMesh)slideMesh[0]).text) {
						((TextMesh)slideMesh [0]).text = nextSlideText + "                                --- стр. " + 
							(((SlideTextPortions)textPortionsComp[0]).currentSlideShown+1).ToString() + " от " + 
							(((SlideTextPortions)textPortionsComp[0]).slidePortionsNo).ToString() + " ---";
					}
				}

				if (Input.GetMouseButtonDown(0) && hasEnterInDown) {
					Vector3 startDown = hit.transform.localPosition;
					Vector3 destination = startDown + new Vector3 (0f, -moveDelta, 0f);
					hit.transform.localPosition = Vector3.MoveTowards(startDown, destination, moveDelta);
					hasEnterInDown = false;
				}
			}
		}
	}

	string GetNextSlideText(string transformObjName, Component[] textPortionsComp, Component[] slideMesh)
	{
		switch (transformObjName) 
		{
		case "First":
			return ((SlideTextPortions)textPortionsComp[0]).SetFirst();
		case "Next":
			return ((SlideTextPortions)textPortionsComp[0]).SetNext();
		case "Prev":
			return ((SlideTextPortions)textPortionsComp[0]).SetPrev();
		case "Last":
			return ((SlideTextPortions)textPortionsComp[0]).SetLast();
		default:
			return ((TextMesh)slideMesh[0]).text;
		}
	}
}
