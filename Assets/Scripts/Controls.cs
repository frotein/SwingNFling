using UnityEngine;
using System.Collections;


// a static class that unifies controls for both PC and mobile
public static class Controls 
{
	static int touchCount;
	public static bool Clicked()
	{
		#if UNITY_EDITOR
		return Input.GetMouseButtonDown(0);
		#endif

		#if UNITY_ANDROID
		return (touchCount == 0 && Input.touchCount == 1);
		#endif
	
		touchCount = Input.touchCount;
	}

	public static Vector2 ClickedPosition() // must make sure clicked is true
	{
		Vector2 screenPosition;
		#if UNITY_EDITOR
		screenPosition = Input.mousePosition;
		#endif

		#if UNITY_ANDROID
		screenPosition = Input.touches[0].position;
		#endif

		return (Vector2)Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x,screenPosition.y,Camera.main.nearClipPlane)); 
	}

	public static bool Released()
	{
		#if UNITY_EDITOR
		return Input.GetMouseButtonUp(0);
		#endif

		#if UNITY_ANDROID
		return (Input.touchCount == 0); //touchCount == 1 && 
		#endif		

		touchCount = Input.touchCount;
	}

	public static void SetTouchCount() // must be called at the end of the frame
	{
		touchCount = Input.touchCount;
	}
	
}
