using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolingSystem 
{
	//pooled objects list.
	private List<GameObject> pooledObjects = new List<GameObject>();

	//original object reference.
	private GameObject original = null;

	//name for object
	private string objectName = "ObjectName";

	//objects parent in hierarchy
	private Transform objectParent = null;


	//initializes a new instance of the PoolingSystem class.
	public PoolingSystem (GameObject obj, int initialSize, string name, Transform parent)
	{
		original = obj;
		objectName = name;
		objectParent = parent;

		GameObject tempObj = null;
		
		// Populate the initial pool
		for (int i = 0; i < initialSize; i++)
		{
			// Instantiate the object
			tempObj = GameObject.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;

			//name the object
			tempObj.name = name;

			//parent the object
			tempObj.transform.parent = objectParent;

			// Set the object inactive
			tempObj.SetActive(false);

			// Add it to the list
			pooledObjects.Add (tempObj);
		}
	}


	//hide object. used instead of Destroy.
	public void Remove(GameObject obj)
	{
		//parent the object
		obj.transform.parent = objectParent;

		//name the object
		obj.name = objectName;

		// Set the object inactive
		obj.SetActive(false);
	}


	public void RemoveFromList(GameObject obj)
	{
		pooledObjects.Remove(obj); //delete object from pooled objects list
	}


	//create object. used instead of Instantiate.
	public GameObject Create()
	{
		for(int i = 0; i < pooledObjects.Count; i++)
		{
			//if object isn't active in hierarchy, return that one
			if(pooledObjects[i])
			{
				if(!pooledObjects[i].activeInHierarchy)
				{
					pooledObjects[i].SetActive (true);
					return pooledObjects[i];
				}
			}else pooledObjects.RemoveAt(i);
		}

		// No free elements, create new one.
		var tempObj = GameObject.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;

		// add the new object to the list
		pooledObjects.Add (tempObj);

		//name the object
		tempObj.name = objectName;

		//parent the object
		tempObj.transform.parent = objectParent;

		return tempObj;
	}
}
