//In this script we have dragger object which has spring joint component attached.
//When we move mouse or finger (for touch device) on screen and "forTouchScreen" variable is set to true(from inspector window)
//We are positioning dragger object to that position. If we click/tap on 2D object tagged as tack, connect to it. 
//So now when you move cursor/finger, dragger object follows it and unity's physics does everything else to make object follow

using UnityEngine;
using System.Collections;

public class Drag : MonoBehaviour {
	public bool forTouchScreen = false;		//determines what input to use (mouse or finger)
	public GameObject dragger;				//dragger object
	public LayerMask layerMask;				//determines which layer objects will be able to drag
	public bool perspectiveCam = false;		//determines what type of camera is used
	public float distanceFromCam = 9.0f;	//dragger object will be placed this distance away from camera if perspectiveCam is true. This is the distance from camera to visuals

	private SpringJoint2D springJoint;		//saves dragger objects SpringJoint2D component for late access
	private Rigidbody2D connectedRB;		//saves object on which dragger object is connected
	private bool dragging = false;			//is true when we are moving dragger object
	private Transform hitObject;			//object which was last hit by raycast
	private bool movingTransform;			//is true when we are moving object which doesn't have rigidbody2D component
	
	private Camera cam;

	void Start()
	{
		//create dragger object
		if(!dragger)
		{
			dragger = new GameObject("dragger");
			dragger.AddComponent<SpringJoint2D>();
		}

		//get springJoint2D component from dragger object
		springJoint = dragger.GetComponent<SpringJoint2D>();

		cam = Camera.main;
	}
	
	void Update ()
	{
		//detect input (touch or mouse button 1)
		if (forTouchScreen ? Input.touchCount > 0 : Input.GetMouseButton(1))
		{
			Vector3 pos;
			
			//if forTouchScreen variable is true (set from inspector window) use touches, else use mouse for dragging
			if(forTouchScreen)
			{
				if(perspectiveCam)
					pos = cam.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, distanceFromCam));
				else
					pos = cam.ScreenToWorldPoint(Input.GetTouch(0).position);//get position, where touch is detected
			}
			else
			{
				if(perspectiveCam)
					pos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceFromCam));
				else
					pos = cam.ScreenToWorldPoint(Input.mousePosition); //get position, where mouse cursor is
			}

			if(!perspectiveCam)
				pos.z = -1;

			//make dragger object's position same as input position
			dragger.transform.position = pos;
			
			//cast ray
			RaycastHit2D  hit;
			hit = Physics2D.Raycast(pos, Vector2.zero, Mathf.Infinity, layerMask);

			if(!hitObject)
				hitObject = hit.transform;

			//check if hit object has collider and we aren't still dragging object
			if(!dragging && !movingTransform && hit.collider && (hit.collider.tag == "tack" || hit.collider.tag == "candy") && hit.collider.GetComponent<Rigidbody2D>())
			{
				//change springjoint anchor & connectedAnchor positions and connect to hit object
				springJoint.anchor = springJoint.transform.InverseTransformPoint (hit.point);
				springJoint.connectedAnchor = hit.transform.InverseTransformPoint (hit.point);
				springJoint.connectedBody = hit.collider.GetComponent<Rigidbody2D>();

				//save hit object's rigidbody2D component and set it's isKinematic false (physics won't affect on it)
				connectedRB = hit.collider.GetComponent<Rigidbody2D>();	
				connectedRB.isKinematic = false;

				dragging = true;
			}
			else if(!dragging && hitObject && hitObject.tag == "tack")	//if object doesn't have rigidbody2D component, position it to input position directly (without physics involvement)
			{
				hitObject.position = pos;
				movingTransform = true;
			}
		}
		else
		{
			//if mouse 0 button or touch isn't detected make springJoint's connected body null. So we aren't dragging object, it'll be free to move
			if(springJoint)
			{
				springJoint.connectedBody = null;
				if(connectedRB && connectedRB.transform.tag != "candy")
					connectedRB.isKinematic = true;
			}
			
			dragging = false;
			movingTransform = false;
			hitObject = null;
		}
	}
}
