using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RopeSystem2D : EditorWindow{
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

	private float ropeWidth = 0.0f;				//width of linerenderer
	private string helpText = "";				//used to give hints
	private GameObject chainsHolder;			//object which will hold all chains
	private List<Transform> pointsHolderArray;	//list for holding points
	
	// Add menu named "My Window" to the Window menu
	[MenuItem ("Window/2D Rope System")]
	static void Init () {
		// Get existing open window or if none, make a new one:
		EditorWindow.GetWindow (typeof (RopeSystem2D));
	}
	
	void OnGUI ()
	{
		//create input form for pointsHolder
		EditorGUILayout.BeginHorizontal();
		pointsHolder = (Transform)EditorGUILayout.ObjectField ("Points Holder", pointsHolder, typeof(Transform), true);
		if(GUILayout.Button ("X", GUILayout.MaxWidth (20), GUILayout.MaxHeight (15)))
			pointsHolder = null;
		EditorGUILayout.EndHorizontal ();

		connectEndPoints = EditorGUILayout.Toggle ("Connect End Points", connectEndPoints);

		GUILayout.Space (5);

		//if nothing is assigned in pointsHolder input form make pointA and pointB input forms visible
		if(!pointsHolder)
		{
			pointA = (Transform)EditorGUILayout.ObjectField ("PointA", pointA, typeof(Transform), true);
			GUILayout.Space (5);

			pointB = (Transform)EditorGUILayout.ObjectField ("PointB", pointB, typeof(Transform), true);
			GUILayout.Space (5);
		}

		//create input form for chainObject
		chainObject = (GameObject)EditorGUILayout.ObjectField ("Chain Object", chainObject, typeof(GameObject), true);

		GUILayout.Space (5);

		//create checkboxes for decision to lock first or last chains
		GUILayout.BeginHorizontal ();
		lockFirstChain = EditorGUILayout.Toggle ("Lock First Chain", lockFirstChain);
		lockLastChain = EditorGUILayout.Toggle ("Lock Last Chain", lockLastChain);
		GUILayout.EndHorizontal ();

		if(lockFirstChain)
			connectToA = false;
		if(lockLastChain)
			connectToB = false;

		//create checkboxes for decision to connect to pointA or pointB
		GUILayout.BeginHorizontal ();
		connectToA = EditorGUILayout.Toggle ("Connect To Point A", connectToA);
		connectToB = EditorGUILayout.Toggle ("Connect To Point B", connectToB);
		GUILayout.EndHorizontal ();
		
		if(connectToA)
			lockFirstChain = false;
		if(connectToB)
			lockLastChain = false;

		//create checkbox for decision to hide start and end chains
		hideEndChains = EditorGUILayout.Toggle ("Hide End Chains", hideEndChains);
		if(hideEndChains)
			useLineRenderer = false;

		GUILayout.Space (5);

		//create checkbox for decision to use linerenderer
		useLineRenderer = EditorGUILayout.BeginToggleGroup ("Use Line Renderer", useLineRenderer);
		if(useLineRenderer)
			hideEndChains = false;

		//create input form for linerenderer's material
		ropeMat = (Material)EditorGUILayout.ObjectField ("Material", ropeMat, typeof(Material), true);

		//create input form for linerenderer's width
		EditorGUILayout.BeginHorizontal ();
		ropeWidth = EditorGUILayout.FloatField ("Width", ropeWidth);
		if(ropeWidth < 0)
			ropeWidth = 0.0f;

		GUILayout.Space (17);
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.EndToggleGroup ();

		GUILayout.Space (10);

		//create font style for helpText
		GUIStyle fontStyle = new GUIStyle();
//		fontStyle.normal.textColor = Color.green;
		fontStyle.fontStyle = FontStyle.Bold;
		fontStyle.fontSize = 12;
		fontStyle.alignment = TextAnchor.MiddleCenter;

		//create label for helpText
		GUILayout.Label (helpText, fontStyle);

		GUILayout.Space (10); 
		
		if(GUILayout.Button ("Create", GUILayout.MinHeight (30)))
		{
			//give hints if something isn't correct
			if(!pointA && !pointsHolder)
			{
				helpText = "PointA Isn't Assigned";
				return;
			}

			if(!pointB && !pointsHolder)
			{
				helpText = "PointB Isn't Assigned";
				return;
			}

			if(!chainObject)
			{
				helpText = "Chain Object Isn't Assigned";
				return;
			}

			if(pointA && pointB && pointA.GetInstanceID() == pointB.GetInstanceID())
			{
				helpText = "Same Object Is Assigned For Both PointA and PointB";
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
				helpText = "Created";
				Create ();
			}
		}
	}

	//create rope
	void Create()
	{
		if(!chainObject.GetComponent<Collider2D>())
		{
			helpText = "Chain Object Doesn't Have Collider2D Attached";
			return;
		}

		if(!chainObject.GetComponent<HingeJoint2D>())
		{
			helpText = "Chain Object Doesn't Have HingeJoint2D Attached";
			return;
		}

		//if rope width is 0, that means that user hadn't set rope width, in that case we make width same as chainObject's renderer size
		if(ropeWidth <= 0.0f)
			ropeWidth = chainObject.GetComponent<Renderer>().bounds.size.x;

		//if pointA has 3D collider attached, remove it
		var colA = pointA.GetComponent<Collider>();
		if(colA)
			Undo.DestroyObjectImmediate(colA);

		//if connectToA is set to true and pointA doesn't have DistanceJoint2D component yet, add it
		if(connectToA)
		{
			var jointA = pointA.GetComponent<DistanceJoint2D>();
			if(!jointA || (jointA && jointA.connectedBody))
				Undo.AddComponent<DistanceJoint2D>(pointA.gameObject);	//register undo
		}

		//if pointB has 3D collider attached, remove it
		var colB = pointB.GetComponent<Collider>();
		if(colB)
			Undo.DestroyObjectImmediate (colB);

		//if connectToB is set to true and pointB doesn't have DistanceJoint2D component yet, add it
		if(connectToB)
		{
			var jointB = pointB.GetComponent<DistanceJoint2D>();
			if(!jointB || (jointB && jointB.connectedBody))
				Undo.AddComponent<DistanceJoint2D>(pointB.gameObject);	//register undo
		}

		//calculate how many chains is needed from pointA to pointB
		var chainCount = (int)(Vector3.Distance(pointA.position, pointB.position) / (chainObject.GetComponent<Renderer>().bounds.extents.x * 1.9f));
		if(chainCount < 2)
		{
			helpText = "Distance between PointA & PointB is very small";

			if(pointsHolder)
				Debug.LogWarning("Distance from "+ pointA.name +" (PointA) to "+ pointB.name +" (PointB) is very small, increase distance");

			return;
		}

		//create "Chains Holder" object, used to make chains children of that object
		chainsHolder = new GameObject("Chains Holder");
		Undo.RegisterCreatedObjectUndo (chainsHolder, "Create Rope"); //register undo

		//create rope
		Rope2D rope = new Rope2D();
		rope.CreateRope (chainsHolder, chainObject, pointA, pointB, lockFirstChain, lockLastChain, connectToA, connectToB, hideEndChains, useLineRenderer, ropeMat, ropeWidth);
	}
}