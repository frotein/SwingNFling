using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
	
	void OnGUI() {
		if (GUI.Button(new Rect(Screen.width - 100, 10, 70, 30), "Scene 1"))
			Application.LoadLevel (0);

		if (GUI.Button(new Rect(Screen.width - 100, 50, 70, 30), "Scene 2"))
			Application.LoadLevel (1);

		if (GUI.Button(new Rect(Screen.width - 100, 90, 70, 30), "Scene 3"))
			Application.LoadLevel (2);
	}
}
