using UnityEngine;
using System.Collections;

public class Rope_Shooter : MonoBehaviour {

	public GameObject objectHolder, chainObject;
	public Transform otherObject;
	public Material ropeMat;
	public Camera cam;
	public GameObject lastChain, lastChain2;
	bool connectedToRope;
	GameObject connectedObject;
	int delay = 0;
	Rope2D rope;
	bool connectedThisFrame;
	// Use this for initialization
	void Start () 
	{
		rope = new Rope2D();

		// initialize the rope in the scene
		rope.Initialize(chainObject, 20);
		connectedToRope = false;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		

		if(Controls.Clicked() && !connectedToRope) // if you click, raycast in 2d from the character towards the clicked point
		{
			Vector3 pos = Controls.ClickedPosition();
			RaycastHit2D[] hits = Physics2D.RaycastAll((Vector2)transform.position, new Vector2(pos.x - transform.position.x,pos.y - transform.position.y).normalized);
			if(hits != null)
			{
				foreach(RaycastHit2D hit in hits)
				{
				
					if(hit.transform.name != "box" && !hit.collider.isTrigger) // if you hit something that isnt the character, connect the rope to the first object 
					{
						
						ConnectionToPosition(hit.point);
						connectedThisFrame = true;
						break;		
					}
				}
			}		
		}

		if(Controls.Clicked() && connectedToRope && !connectedThisFrame) // if you release, disconnect the rope.
		{
			rope.Remove();	
			objectHolder = new GameObject("rope holder");
			connectedToRope = false;
		}
		
		connectedThisFrame = false;
	}

	// check whether point a is to the left or right of the 2d line defined by 2 points in it b and c
	public bool isLeft(Vector2 a, Vector2 b, Vector2 c)
	{
    	return ((b.x - a.x)*(c.y - a.y) - (b.y - a.y)*(c.x - a.x)) > 0;
	}

	void FixedUpdate()
	{
		// if we are currently connected to a rope, apply force to the player perpendicular to the connection point
		// apply the force in a direction depending on what direction the player is currenly moving in
		if(connectedToRope) 
		{
			
			Vector3 vec = lastChain2.transform.position -  lastChain.transform.position;
        	float Angle = Mathf.Atan2(vec.y,vec.x);
        	float ang = Angle * Mathf.Rad2Deg;

			
        	// set the last chain to the angle from the previouse rope , fixes some small beg with the 2d rope script
			if(Mathf.Abs(lastChain.transform.eulerAngles.z - ang) > 1)
			lastChain.transform.eulerAngles = new Vector3(0,0,ang);

			Rigidbody2D rb = transform.GetComponent<Rigidbody2D>();
			
			if(delay <= 0)
			{ 
				Vector2 pos2d = ((Vector2)transform.position);
				bool left = isLeft(((Vector2)connectedObject.transform.position),pos2d ,pos2d + ((Vector2) rb.velocity));
				int way = 1;
				if(!left)	
				way = -1;
				
				Vector2 dir = (((Vector2)connectedObject.transform.position) - ((Vector2)transform.position)).normalized; 
				
				rb.AddForce(new Vector2(dir.y , -dir.x) * way * 5);
			}
			delay--;
		}
		else delay = 0;
	}

	void LateUpdate()
	{
		Controls.SetTouchCount();
	}

	// Connects the rope to the player and a given position pos
	void ConnectionToPosition(Vector2 pos)
	{		
		objectHolder.transform.position = new Vector3(objectHolder.transform.position.x,objectHolder.transform.position.y, 0);
		transform.GetComponent<DistanceJoint2D>().enabled = true;
		if(connectedObject != null)
		GameObject.Destroy(connectedObject);

		connectedObject = GameObject.Instantiate(otherObject.gameObject);
		Vector2 vel = transform.GetComponent<Rigidbody2D>().velocity;
		connectedObject.transform.position = new Vector3(pos.x,pos.y,0);
		rope.CreateRope(objectHolder, chainObject, transform, connectedObject.transform, false,false,true,true, false, true, ropeMat,.1f);
		lastChain = objectHolder.transform.GetChild(objectHolder.transform.childCount - 1).gameObject;
		lastChain2 = objectHolder.transform.GetChild(objectHolder.transform.childCount - 2).gameObject; 
		lastChain.GetComponent<Rigidbody2D>().isKinematic = true;
		transform.GetComponent<Rigidbody2D>().isKinematic = false;	
		transform.GetComponent<Rigidbody2D>().velocity = vel;
		objectHolder.transform.position = new Vector3(objectHolder.transform.position.x,objectHolder.transform.position.y, 1);
		connectedToRope = true;
	}


	// set a slight delay to the velocity pusher when the player collides so you reverse direction correctly on collisions
	void OnCollisionEnter2D(Collision2D coll)
	{
		delay = 5;
	}
}
