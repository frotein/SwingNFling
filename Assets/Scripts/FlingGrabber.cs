using UnityEngine;
using System.Collections;

public class FlingGrabber : MonoBehaviour {

	public float maxDist;
	public Vector3 flingPosition;
	public Flinger otherFling;
	public Rope_Shooter shooter;
	public Transform bottomPoint;
	bool grabbed;
	Rigidbody2D rb;
	
	// Use this for initialization
	void Start () 
	{
		grabbed = false;
		rb = transform.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Controls.Clicked()) // if you click on the model while its in the flinger, grab it
		{
			if(transform.GetComponent<BoxCollider2D>().OverlapPoint(Controls.ClickedPosition()))
			{
				grabbed = true;
				rb.isKinematic = true;	
			}
			
		}
		if(grabbed)
		{
			if(Controls.Released()) // if you release the model after grabbing it, fling it
			{
				rb.isKinematic = false;
				grabbed = false;
				Vector2 dirVec = (Vector2)(transform.position - flingPosition)  * -150;
				if(dirVec.y < 100)
				dirVec.y = 100;

				rb.AddForce(dirVec);
				otherFling.enabled = false;
				shooter.enabled = true;
				this.enabled = false;
			}
			else // otherwise, if you have grabbed it, move the model to the controls point
			{
				Vector3 mousePos = Controls.ClickedPosition();
				Vector2 direction = (Vector2)(mousePos - flingPosition);
				BoxCollider2D box = transform.GetComponent<BoxCollider2D>();
				RaycastHit2D[] hits = Physics2D.BoxCastAll((Vector2) mousePos, box.size, rb.rotation, Vector2.zero);
				 
				if(hits.Length < 2)
				{
					if(direction.magnitude < maxDist)
					{
						transform.position = new Vector3(mousePos.x,mousePos.y,transform.position.z);
					}
					else
					{
						transform.position = flingPosition + new Vector3(direction.x,direction.y,transform.position.z).normalized * maxDist;	
					}
					if(transform.position.y < bottomPoint.position.y)
					transform.position = new Vector3(transform.position.x,bottomPoint.position.y,transform.position.z);					
				}
			}
			
		}
	}

	
}
