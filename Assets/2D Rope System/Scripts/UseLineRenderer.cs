using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class UseLineRenderer : MonoBehaviour {

	public static bool brokenChainsHolderCreated = false;	//used to save chains which aren't connected to another chain

	//used to set linerenderer's material and width
	private LineRenderer lineRend;
	public Material ropeMaterial;
	public float width = 0.0f;
	
	private List<Transform> chains;			//used to save chains
	private GameObject brokenChainsHolder;	//used to save "Broken Chains Holder" object
	private Transform tr;					//used to save rope's transform for future use to iterate through its children

	//used to initialize rope
	private bool started = false;
	private float startChainCount;


	// Use this for initialization
	void Start ()
	{
		tr = transform; //get transform

		//if "Broken Chains Holder" isn't created yet, create it
		if(!brokenChainsHolderCreated)
		{
			brokenChainsHolder = GameObject.Find ("Broken Chains Holder");

			if(brokenChainsHolder == null)
			{
				brokenChainsHolder = new GameObject("Broken Chains Holder");
				brokenChainsHolderCreated = true;
			}
		}

		//if chains list is empty fill it
		if(chains == null)
		{
			chains = new List<Transform>();

			foreach(Transform child in tr)
				chains.Add (child);
		}

		//if LineRenderer component isn't added, add it
		if(chains.Count > 0 && !GetComponent<LineRenderer>())
		{
			lineRend = gameObject.AddComponent<LineRenderer>();
			lineRend.SetVertexCount (chains.Count);
			lineRend.material = ropeMaterial;
		}
		else lineRend = GetComponent<LineRenderer>();

		//set linerenderer's width
		if(lineRend)
		{
			if(width <= 0.0f)
			{
				width = chains[0].GetComponent<Renderer>().bounds.size.x;
			}

			lineRend.SetWidth (width, width);
		}

		startChainCount = chains.Count;
		
		started = true;
	}


	//add chain into chains list
	public void AddChain(Transform chain)
	{
		//if chains list isn't created, create it
		if(chains == null)
			chains = new List<Transform>();

		//if linerenderer isn't added, add it
		if(lineRend == null)
		{
			lineRend = GetComponent<LineRenderer>();

			if(!lineRend)
			{
				lineRend = gameObject.AddComponent<LineRenderer>();
				lineRend.material = ropeMaterial;
			}
		}

		chains.Add (chain);	//add chain into chains list

		//fill LineRenderer component's positions
		lineRend.SetVertexCount (chains.Count);
		lineRend.SetPosition (chains.Count - 1, chains[chains.Count - 1].position);

		startChainCount = chains.Count;
	}


	// Update is called once per frame
	void Update ()
	{
		//if rope hasn't any child object, destroy it
		if(tr.childCount < 1)
			Destroy (gameObject);
		else if(chains != null && chains.Count > 0)
		{
			//update linerenderer component's positions
			for(int i = 0; i < chains.Count; i++)
				lineRend.SetPosition (i, chains[i].position);
		}
	}


	//split the rope
	public void Split(string name, bool newMatPerChunk)
	{
		//if rope initialization isn't done, do it now
		if(!started)
			Start ();

		if(chains != null && chains.Count > 0)
		{
			GameObject newPart = new GameObject("new part");	//create new object to hold another part of rope
			bool splitAfter = false;

			//iterate through all chain
			foreach(Transform child in chains)
			{
				//check object's name to detect from which chain is needed to start splitting
				if(!splitAfter && name == child.name)
					splitAfter = true;

				if(splitAfter)
				{
					var joint = child.GetComponent<HingeJoint2D>();	//get HingeJoint2D component from chain

					if((joint && joint.enabled && joint.connectedBody) || (!joint && (child == chains[0] || child == chains[chains.Count - 1] )))
					{
						child.parent = newPart.transform;	//make this chain's parent the newly created object
					}
					else
					{
						child.parent = (brokenChainsHolder != null) ? brokenChainsHolder.transform : null;
						child.GetComponent<Collider2D>().isTrigger = true;

//						uncomment if you want to destroy chain instead adding/parrenting it in "BrokenChainsHolder" object
//						child.parent = null;
//						Destroy (child.gameObject);
					}
				}
			}

			//clear linerenderer's positions
				lineRend.SetVertexCount(0);

			//fill chains array
			chains.Clear ();
			foreach(Transform child in tr)
				chains.Add (child);

			//set linerenderer's positions count
				lineRend.SetVertexCount(chains.Count);

			//add UseLineRenderer script to new part and set rope material for that
			var useLineRend = newPart.AddComponent<UseLineRenderer>();

			//if newMatPerChunk is true, create new material for each part of rope
			if(newMatPerChunk)
			{
				//create new material
				Material newMat = new Material(ropeMaterial);

				//calculate texture scale
				float difference = startChainCount / (startChainCount - chains.Count - 1);	
				float xScale = ropeMaterial.mainTextureScale.x / difference;

				//set texture scale, new material and width
				newMat.mainTextureScale = new Vector2(xScale, ropeMaterial.mainTextureScale.y);
				useLineRend.ropeMaterial = newMat;
				useLineRend.width = width;

				//change material for left rope part
				if(chains.Count > 0)
				{
					//create new material
					Material newMat2 = new Material(ropeMaterial);

					//calculate texture scale
					float difference2 = startChainCount / chains.Count;
					float xScale2 = ropeMaterial.mainTextureScale.x / difference2;

					//set texture scale, new material and width
					newMat2.mainTextureScale = new Vector2(xScale2, ropeMaterial.mainTextureScale.y);
					lineRend.material = newMat2;
					ropeMaterial = newMat2;
				}

				startChainCount = chains.Count;
			}
			else useLineRend.ropeMaterial = ropeMaterial;
		}
	}
}
