//Unity 4.3.1 version's 2D physics doesn't work as its 3D physics. 2D Rigidbody attached object doesn't follow its parent when isKinematic is set to true,
//So we are arranging its transform with this script, this may won't be needed in future versions


using UnityEngine;
using System.Collections;

public class FollowParent : MonoBehaviour {

	private Transform tr;
	private Transform trParent;
	private Vector3 posDiference;

	// Use this for initialization
	void Start ()
	{
		if(!transform.parent)
		{
			//if this object doesn't have parent give warning
			Debug.LogWarning ("Object " + gameObject.name +" has attached 'FollowParent' script, but it hasn't parent");
			return;
		}

		tr = transform; 								//retrieve this objects transform
		trParent = tr.parent; 							//retrieve parent's transform
		posDiference = trParent.position - tr.position; //calculate position diference between this and parent object
	}
	
	// Update is called once per frame
	void Update ()
	{
		//adjust this object's position
		if(trParent)
			tr.position = trParent.position - posDiference;
	}
}
