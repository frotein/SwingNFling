using UnityEngine;
//using UnityEditor;
using System.Collections.Generic;

public class RuntimeRope : MonoBehaviour{
	public GameObject chainObject;			//object which will be used for creating rope
	public Transform pointsHolder;			//parent of objects which are used to create rope between those points
	public bool connectEndPoints = true;	//used to create or not rope between first and last points

//first and second points, between thes points will be created rope
	public Transform pointA;
	public Transform pointB;

	//used to lock or not lock first and last chain of rope
	public bool lockFirstChain = false;
	public bool lockLastChain = false;

	//used to connect or not rope end points to pointA or pointB
	public bool connectToA = true;
	public bool connectToB = true;

	public bool hideEndChains = true;			//used to determine hide or not first and last chain
	public bool useLineRenderer = false;		//use or not linerenderer for rope
	public Material ropeMat;					//material of linerenderer

	public float delay = 0.01f;					//delay between chain creation
	public float ropeWidth = 0.0f;				//width of linerenderer

	private GameObject chainsHolder;			//object which will hold all chains
	private List<Transform> pointsHolderArray;	//list for holding points
	private Rope2D rope = new Rope2D();


	void Start()
	{
		if(!chainObject.GetComponent<Collider2D>())
			Debug.LogWarning("Chain Object Doesn't Have Collider2D Attached");

		if(chainObject)
		{
			var chainHingeJoint = chainObject.GetComponent<HingeJoint2D>();	//get HingeJoint2D component from chainObject

			//if chain object doesn't have 'HingeJoint2D' component attached, give warning
			if(!chainHingeJoint)
				Debug.LogWarning ("Chain Object Doesn't Have 'HingeJoint2D' Component Attached");
			else
				chainHingeJoint.enabled = false;

			rope.Initialize (chainObject,50);	//create rope pool
		}
		else
		{
			Debug.LogWarning("Chain Object Isn't Assigned");
		}
	}
	
	void Update ()
	{
		//if key 'C' is pressed and everything is set correct, create rope
		if(Input.GetKeyDown(KeyCode.C))
		{
			//if something isn't set correct don't do anything
			if(!pointA && !pointsHolder)
			{
				Debug.LogWarning ("PointA Isn't Assigned");
				return;
			}
			
			if(!pointB && !pointsHolder)
			{
				Debug.LogWarning ("PointB Isn't Assigned");
				return;
			}
			
			if(!chainObject)
			{
				Debug.LogWarning ("Chain Object Isn't Assigned");
				return;
			}
			
			if(pointA && pointB && pointA.GetInstanceID() == pointB.GetInstanceID())
			{
				Debug.LogWarning ("Same Object Is Assigned For Both PointA and PointB");
				return;
			}

			//if in pointsHolder is assigned, create rope between its children's positions
			if(pointsHolder)
			{
				pointsHolderArray = new List<Transform>();

				//get all children
				foreach(Transform child in pointsHolder)
					pointsHolderArray.Add(child);

				//set pointA and pointB and create rope
				for(int i = 0; i < pointsHolderArray.Count - 1; i++)
				{
					pointA = pointsHolderArray[i];
					pointB = pointsHolderArray[i + 1];
					
					Create ();
				}

				//if connectEndPoints is set to true, create rope between first and last points
				if(connectEndPoints)
				{
					pointA = pointsHolderArray[pointsHolderArray.Count - 1];
					pointB = pointsHolderArray[0];
					
					Create ();
				}
			}
			else
			{
				Create ();
			}
		}

		//if 'R' key is pressed, remove rope
		if(Input.GetKeyDown(KeyCode.R))
		{
			rope.Remove();
		}
	}

	//create rope
	void Create()
	{
		if(!chainObject.GetComponent<Collider2D>())
		{
			Debug.LogWarning("Chain Object Doesn't Have Collider2D Attached");
			return;
		}
		
		if(!chainObject.GetComponent<HingeJoint2D>())
		{
			Debug.LogWarning("Chain Object Doesn't Have HingeJoint2D Attached");
			return;
		}

		//if rope width is 0, that means that user hadn't set rope width, in that case we make width same as chainObject's renderer size
		if(ropeWidth <= 0.0f)
			ropeWidth = chainObject.GetComponent<Renderer>().bounds.size.x;

		//if pointA has 3D collider attached, remove it
		var colA = pointA.GetComponent<Collider>();
		if(colA)
			DestroyImmediate(colA);

		//if connectToA is set to true and pointA doesn't have DistanceJoint2D component yet, add it
		if(connectToA)
		{
			var jointA = pointA.GetComponent<DistanceJoint2D>();
			if(!jointA || (jointA && jointA.connectedBody))
			{
				pointA.gameObject.AddComponent<DistanceJoint2D>();
				pointA.GetComponent<Rigidbody2D>().isKinematic = true;
			}
		}

		//if pointB has 3D collider attached, remove it
		var colB = pointB.GetComponent<Collider>();
		if(colB)
			DestroyImmediate(colB);

		//if connectToB is set to true and pointB doesn't have DistanceJoint2D component yet, add it
		if(connectToB)
		{
			var jointB = pointB.GetComponent<DistanceJoint2D>();
			if(!jointB || (jointB && jointB.connectedBody))
			{
				pointB.gameObject.AddComponent<DistanceJoint2D>();
				pointB.GetComponent<Rigidbody2D>().isKinematic = true;
			}
		}

		//calculate how many chains is needed from pointA to pointB
		var chainCount = (int)(Vector3.Distance(pointA.position, pointB.position) / (chainObject.GetComponent<Renderer>().bounds.extents.x * 1.9f));
		if(chainCount < 2)
		{
			Debug.LogWarning("Distance from "+ pointA.name +" (PointA) to "+ pointB.name +" (PointB) is very small, increase distance");
			return;
		}

		//create "Chains Holder" object, used to make chains children of that object
		chainsHolder = new GameObject("Chains Holder");

		//uncomment this if you want to create rope instantly
//		rope.CreateRope (chainsHolder, chainObject, pointA, pointB, lockFirstChain, lockLastChain, connectToA, connectToB, hideEndChains, useLineRenderer, ropeMat, ropeWidth);

		//this function uses delay between chain creation
		StartCoroutine(rope.CreateRopeWithDelay(chainsHolder, chainObject, pointA, pointB, lockFirstChain, lockLastChain, connectToA, connectToB, hideEndChains, useLineRenderer, ropeMat, ropeWidth, delay));
	}
}