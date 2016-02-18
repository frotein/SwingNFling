using UnityEngine;
using System.Collections;

public class Restart : MonoBehaviour {

	//when mouse is clicked on this object reload current scene
	void OnMouseDown()
	{
		Application.LoadLevel (Application.loadedLevel);
	}
	
	// Update is called once per frame
	void Update ()
	{
		//if touchcount is more than 2 reload current scene
		if(Input.touchCount > 2)
			Application.LoadLevel (Application.loadedLevel);
	}
}
