using UnityEngine;
using System.Collections;

public class Breakable : MonoBehaviour {

	public static AudioSource audioSrc; 	//used to play sounds when rope is broken

	public float breakAngle = 30;			//when angle between this and connected chain is more than this variable's value, chain is broken (detach from rope and play sound)
	public float breakDistance = 1.3f;		//when distance between this and connected chain is more than this variable's value, chain is broken (detach from rope and play sound)
	public bool newMatPerChunk = true;		//if this variable is set to true, when using linerenderer, for each broken part will be created new material, so the texture scale will be correspond to size of rope
	public bool limitBreakCount = false;	//limit break
	public int maxBreakCount = 3;			//number of maximum break
	public AudioClip chainBreakSound; 		//this sound will be played when chain is broken
	public GameObject breakFX;  			//this game object will be instantiated when chain is broken

	//these variables are used to save components for late access, good for optimization
	private HingeJoint2D joint;
	private HingeJoint2D connectedJoint;
	private Transform connectedObject;
	private Transform tr;
	
	private bool broken = false;
	private bool jointGot = false;
	private float time = 0.0f;

	[System.NonSerialized]
	public int breakCount = 0;


	void Awake()
	{
		//if audio source isn't created yet, create it, position to camera, add AudioSource component
		if(audioSrc == null)
		{
			GameObject obj = new GameObject("Break Sound Player");
			obj.transform.position = Camera.main.transform.position;
			audioSrc = obj.AddComponent<AudioSource>();
			audioSrc.playOnAwake = false;
		}
	}


	//called when script is enabled
	void OnEnable ()
	{
		//reset variables
		broken = false;
		jointGot = false;
		breakCount = 0;

		//save components in variables to use them in update function, good for optimization
		if(tr == null)
			tr = transform;

		//save this and connected chain's HingeJoint2D component and connected chains transform too, to use in angle calculation
		if(joint == null)
			joint = GetComponent<HingeJoint2D>();

		if(!limitBreakCount)
			maxBreakCount =	breakCount + 1;

		time = Time.time;
	}

	//get transform and HingeJoint2D information from this joint's connected object
	void GetConnectedObject()
	{
		connectedObject = joint.connectedBody.transform;
		connectedJoint = connectedObject.GetComponent<HingeJoint2D>();

		jointGot = true;
	}
	

	// Update is called once per frame
	void FixedUpdate ()
	{
		//if chain isn't broken yet and chain's angle is more than breakAngle variable(set from inspector window) call "Break" method
		if(Time.time > time + 1 && breakCount < maxBreakCount && !broken)
		{
			if(joint && joint.connectedBody) //if this object has joint and it is connected to another object
			{
				if(!jointGot)//if we still haven't got information from connected object, get it
					GetConnectedObject ();

				if(connectedJoint)
				{
					var angle = Vector3.Angle (tr.up, connectedObject.up); //check angle between this chain and connected chain

					if(angle > breakAngle)//if angle is more than breakAngle variable, break chain
					{
						Break ();
					}
				}

				if(connectedObject)
				{
					if(Vector3.Distance (tr.position, connectedObject.position) > breakDistance)//if distance is more than breakDistance variable, break chain
					{
						Break ();
					}
				}
			}

			//if this object doesn't have HingeJoint2D  component or it has but isn't enabled, 
			//make broken variable false to not play audio when this object rotates more than brokeAngle variable
			if(!joint ||(jointGot && joint && !joint.enabled))
			{
				broken = true;
			}
		}
	}


	void Break()
	{
		//if limitBreakCount is set to true (from inspector window) iterate through all chains and increase breakCount
		if(limitBreakCount)
		{
			foreach(Transform child in tr.parent.transform)
				child.GetComponent<Breakable>().breakCount ++;
		}

		//disable hingejoint component, collider and renderer
		if(joint)
		{
			joint.enabled = false;
			tr.GetComponent<Collider2D>().isTrigger = true; 
			tr.GetComponent<Renderer>().enabled = false;
		}

		//if breakFX is set, instantiate it where break occurs
		if(breakFX)
		{
			var obj = Instantiate (breakFX, tr.position, Quaternion.identity) as GameObject;
			Destroy (obj, 1);
		}

		if(!audioSrc.isPlaying) //if audio isn't playing, play chain break sound
		{
			audioSrc.enabled = true;
			audioSrc.clip = chainBreakSound;
			audioSrc.Play ();
		}
		
		broken = true;

		//if rope uses linerenderer, call UseLineRenderer's Split function
		if(joint && joint.transform.parent)
		{
			var linerend = joint.transform.parent.GetComponent<UseLineRenderer>();
			if(linerend)
			{
				linerend.Split(joint.name, newMatPerChunk);
			}
		}
	}
}
