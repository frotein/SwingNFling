using UnityEngine;
using System.Collections;


// SLingshot flinging class used to start player movement
public class Flinger : MonoBehaviour {

	public GameObject player;
	public Transform leftBand, rightBand;
	Vector3 leftPoint, rightPoint, flingTowardsPoint;
	// Use this for initialization
	void Start () 
	{
		// get the two ends of the slingshot
		leftPoint = transform.GetChild(0).position;
		rightPoint = transform.GetChild(1).position;
		
		// set the point the slingshot will fling towards when let go
		flingTowardsPoint = transform.GetChild(3).position;

		// set position in the grabber
		player.GetComponent<FlingGrabber>().flingPosition = flingTowardsPoint;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		// get the two sides of the player model the slingshot bands are going to
		Vector3 leftPlayerPoint = player.transform.GetChild(0).position;
		Vector3 rightPlayerPoint = player.transform.GetChild(1).position;
		
		
		// set the band positions
		Vector3 leftVec = leftPlayerPoint - leftPoint;
        float leftAngle = Mathf.Atan2(leftVec.y,leftVec.x) * Mathf.Rad2Deg;
		leftBand.eulerAngles = new Vector3(0,0,leftAngle);
		leftBand.localScale = new Vector3(leftVec.magnitude * 1.45f,1,1);
		leftBand.position = leftVec * .5f + leftPoint;

		Vector3 rightVec = rightPlayerPoint - rightPoint;
        float rightAngle = Mathf.Atan2(rightVec.y,rightVec.x) * Mathf.Rad2Deg;
		rightBand.eulerAngles = new Vector3(0,0,rightAngle);
		rightBand.localScale = new Vector3(rightVec.magnitude * 1.45f,1,1);
		rightBand.position = (rightVec * .5f + rightPoint) + new Vector3(0,0,1);
	}
	
	
}
