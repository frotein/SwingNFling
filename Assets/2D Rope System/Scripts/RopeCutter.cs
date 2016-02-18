using UnityEngine;
using System.Collections;

public class RopeCutter : MonoBehaviour {
	public bool useTouch = false;			//determines which input to use (mouse or touch)
	public AudioClip cutSound;				//audio which is played when rope is cut
	public GameObject cutFX;				//object which will be instantiated at cut position
	public bool limitCutCount = false;		//limit cuts for whole scene
	public int maxCutCount = 5;				//max count of cuts for whole scene
	public bool limitCutPerObject = false;	//limit cut for per object
	public int maxCutPerObject = 2;			//max count of cuts per object
	public bool newMatPerChunk = true;		//do we create new material for each cut part? used for ropes using linerenderer to make texture scale correspond to rope size
	public bool perspectiveCam = false;		//determines what type of camera is used
	public float distanceFromCam = 9.0f;	//cutter object will be placed this distance away from camera if perspectiveCam is true. This is the distance from camera to visuals

	private bool cutting = false;			//determines if cutting is in process
	private bool cut = true;				//used to limit cut per object
	private int cutCount = 0;				//used to limit cut for whole scene
	private float lastCutTime;				//used to make little bit delay between detecting cut

	//used to set trailrenderer's time
	private float trailRendTime;
	private float dragEndTime;

	//used to save cutter objects components
	private Transform tr;
	private Camera cam;
	private AudioSource audioSrc;
	private TrailRenderer trailRend;

	//used to save chain's IDs
	private Hashtable ropeHash = new Hashtable();


	// Use this for initialization
	void Start ()
	{
		//get cutter object's components
		tr = transform;
		cam = Camera.main;
		audioSrc = GetComponent<AudioSource>();
		trailRend = GetComponent<TrailRenderer>();

		trailRendTime = trailRend.time;

		if(!limitCutCount)
			maxCutCount = cutCount + 1;
	}


	// Update is called once per frame
	void Update ()
	{
		//if touchcount > 0 or mouse button is pressed
		if (useTouch ? Input.touchCount > 0 : Input.GetMouseButton(0))
		{
			Vector3 pos;

			//if useTouch variable is true (set from inspector window) use touch, else use mouse for dragging
			if(useTouch)
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

			//position cutter object to touch/click position
			tr.position = pos;
			cutting = true;

			//wait little bit before showing trail renderer. used to not draw trail at first click/touch, 
			//because this object jumps from last click/touch position to current click/touch position and it'll look weird
			if(Time.time > dragEndTime + 0.05f)
				trailRend.time = trailRendTime;
		}
		else
		{
			cutting = false;
			trailRend.time = 0;
			dragEndTime = Time.time;
		}
	}

	//called when cutter object exit's from another object's collider
	void OnTriggerExit2D(Collider2D col)
	{
		if(Time.time > lastCutTime + 0.2f && !col.isTrigger) //wait 0.2 before second cut, used to avoid double cut when this trigger exits 2 collider simultaneously
		{
			if(cutCount < maxCutCount && cutting && col.transform.parent && col.tag == "rope2D" && col.GetComponent<HingeJoint2D>())
			{
				//if limitCutPerObject is set to true, check if we can cut it again
				if(limitCutPerObject)
				{
					var id = col.transform.parent.GetInstanceID(); //get object ID

					if(ropeHash.ContainsKey (id))
					{
						if((int)ropeHash[id] < maxCutPerObject - 1) //rope can be cut again
						{
							cut = true;
							ropeHash[id] = (int)ropeHash[id] + 1;
						}
						else cut = false;
					}
					else
					{
						ropeHash.Add (id,0);
						cut = true;
					}
				}

				//if we can cut rope
				if(!limitCutPerObject || (limitCutPerObject && cut))
				{
					//if audio isn't playing, play cut sound
					if(!audioSrc.isPlaying)
					{
						audioSrc.clip = cutSound;
						audioSrc.Play ();
					}
					
					col.GetComponent<HingeJoint2D>().enabled = false; //disable cut object's HingeJoint2D component, so it won't be connected to another object
					col.isTrigger = true;
					col.GetComponent<Renderer>().enabled = false;

					//if cutFX object is set (from inspector window), instantiate that object at cut position
					if(cutFX)
					{
						var obj = Instantiate (cutFX, col.transform.position, Quaternion.identity) as GameObject;
						Destroy (obj, 1);
					}

					//if rope has attached "UseLineRenderer" script call its Split function
					if(col.transform.parent && col.transform.parent.GetComponent<UseLineRenderer>())
					{
						col.transform.parent.GetComponent<UseLineRenderer>().Split(col.name, newMatPerChunk);
					}

					if(limitCutCount)
						cutCount ++;
				}
				lastCutTime = Time.time;
			}
		}
	}
}
