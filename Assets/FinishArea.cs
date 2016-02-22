using UnityEngine;
using System.Collections;

public class FinishArea : MonoBehaviour {

	public GameObject player;
	Rigidbody2D playerRB;
	bool inside;
	// Use this for initialization
	void Start () 
	{
		playerRB = player.GetComponent<Rigidbody2D>();
		inside  = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		HasFinished();
	}

	public bool HasFinished() // returns true if player is stopped inside finish area
	{
		if(playerRB.velocity.magnitude == 0 && inside)
		{
			Debug.Log("Finished");
			return true;
		}

		return false;
	}

	public void OnTriggerEnter2D(Collider2D col)
	{
		if(col.attachedRigidbody.tag == "Player")
		{
			inside = true;
			Debug.Log("entered");
		}
	}

	public void OnTriggerExit2D(Collider2D col)
	{
		if(col.attachedRigidbody.tag == "Player")
		{
			inside = false;
		}
	}
}
