using UnityEngine;
using System.Collections;

public class StoneBehavior : MonoBehaviour {
	
	private Material materialColored;

	public float  FORCE_MULTIPLIER = 0.1f;
	public float  FRICTION = .1f;
	public float SLIDE_FORCE = 0.1f;

	private float X_DISPLACE = 3.7f;

	private float force;
	private float inputStart;

	private bool  xLocked;

	private bool  inMotion;
	private bool  finished;

	Transform clickedObject = null;
	Vector3 objectOffset;

	// Use this for initialization
	void Start () 
	{			
		Reset ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Application.platform == RuntimePlatform.WindowsEditor || 
			Application.platform == RuntimePlatform.OSXEditor ||
		    Application.platform == RuntimePlatform.Android) 
		{
			if (!xLocked)
			{
				DragStoneMouse();
			}

			if (xLocked && !inMotion)
			{
				CalculateClickPoints();
			}
		}
		else
		{
			if (!xLocked)
			{
				DragStoneTouch();
			}

			if (xLocked && !inMotion)
			{
				CalculateTouchPoints();
			}
		}

		if (Mathf.Abs (this.transform.position.x) >= X_DISPLACE ||
		    this.transform.position.z >= 20.5) 
		{
			transform.gameObject.rigidbody.velocity = new Vector3(0,0,0);
			finished = true;
		}

		if (!inMotion && force != 0) 
		{
			// Send the puck
			rigidbody.AddForce(new Vector3(0, 0, force));
			inMotion = true;
		}

		if (inMotion && !finished)
		{
			ApplyFriction ();
			ApplyMotion();
		}

		if (finished) 
		{
			StartCoroutine(WaitAndReset());
		}
	}

	public void Reset()
	{		
		force = 0;
		inputStart = -1f;
		inMotion = false;
		finished = false;
		xLocked = false;
		clickedObject = null;

		this.rigidbody.velocity = new Vector3 (0, 0, 0);
		this.rigidbody.rotation = new Quaternion(0,0,0,0);
		this.transform.position = new Vector3 (0, 0.1f, -8);
	}

	public void ApplyMotion()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		
		if (Input.GetMouseButton (0))
		{
			float mouseX = Input.mousePosition.x;
			if (mouseX <= (Screen.width / 2))
			{
				TurnLeft();
			}
			else
			{
				TurnRight();
			}
		}
	}

	public void TurnLeft()
	{
		rigidbody.AddForce (new Vector3(SLIDE_FORCE * -1, 0, 0));
	}

	public void TurnRight()
	{
		rigidbody.AddForce (new Vector3(SLIDE_FORCE, 0, 0));
	}

	void DragStoneMouse()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		if (Input.GetMouseButtonDown (0) && null == clickedObject) 
		{
			if (Physics.Raycast (ray, out hit, 10)) 
			{
				if (this.transform == hit.transform) 
				{
					// we hit the button
					Debug.Log ("Clicked button");
					clickedObject = hit.transform;
					objectOffset = clickedObject.position - ray.origin;
				}
			}
		}
		else if (Input.GetMouseButtonUp (0) && clickedObject) 
		{
			Debug.Log("Released button");
			clickedObject = null;
			xLocked = true;
		}
		
		if (clickedObject)
		{
			// Only move the object on the X axis.
			float newXPos = ray.origin.x + objectOffset.x;
			if (newXPos < (X_DISPLACE * -1))
			{
				newXPos = X_DISPLACE * -1;
			}
			else if (newXPos > X_DISPLACE)
			{
				newXPos = X_DISPLACE;
			}

			clickedObject.position = new Vector3(newXPos, clickedObject.position.y, clickedObject.position.z); 
		}
	}

	void DragStoneTouch()
	{
		Ray ray; 
		RaycastHit hit;
		
		if (Input.touchCount > 0 &&  !clickedObject &&
		    Input.GetTouch(0).phase == TouchPhase.Began) 
		{
			ray = Camera.main.ScreenPointToRay (Input.GetTouch(0).position);
			if (Physics.Raycast (ray, out hit, Mathf.Infinity)) 
			{
				if (this.transform == hit.transform) 
				{
					// we hit the button
					Debug.Log ("touched button");
					clickedObject = hit.transform;
					objectOffset = clickedObject.position - ray.origin;
				}
			}
		}

		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended
		         && clickedObject) 
		{
			Debug.Log("untouched button");
			clickedObject = null;
			xLocked = true;
		}
		
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved && clickedObject)
		{
			Debug.Log("Dragging object");
			// Only move the object on the X axis.
			float newXPos = ray.origin.x + objectOffset.x;
			if (newXPos < (X_DISPLACE * -1))
			{
				newXPos = X_DISPLACE * -1;
			}
			else if (newXPos > X_DISPLACE)
			{
				newXPos = X_DISPLACE;
			}
			
			clickedObject.position = new Vector3(newXPos, clickedObject.position.y, clickedObject.position.z); 
		}
	}

	void ApplyFriction()
	{		
		if (inMotion && rigidbody.velocity.z > 0) 
		{
			rigidbody.AddForce (new Vector3 (0, 0, FRICTION * -1));
			if (rigidbody.velocity.z < 0.1)
			{
				rigidbody.velocity = new Vector3(0,0,0);
				rigidbody.rotation = new Quaternion(0,0,0,0);
				finished = true;
			}
		}
	}

	void CalculateClickPoints()
	{
		if (Input.GetMouseButtonDown(0))
		{
			// calculate force first
			if (force == 0)
			{
				inputStart = Input.mousePosition.y;
			}
		}
		
		if (Input.GetMouseButtonUp(0))
		{
			// calculate force first
			if (force == 0 && inputStart != -1)
			{
				force = Mathf.Abs((Input.mousePosition.y - inputStart) * FORCE_MULTIPLIER);
				Debug.Log("Setting force = " + force);
			}
		}
	}

	void CalculateTouchPoints ()
	{
		// Get touch
		if (Input.touchCount > 0)
		{
			Touch touchPoint = Input.GetTouch(0);
			if (touchPoint.phase == TouchPhase.Began)
			{
				// calculate force first
				if (force == 0)
				{
					inputStart = touchPoint.position.y;
				}
			}
			
			if (touchPoint.phase == TouchPhase.Ended)
			{
				// calculate force first
				if (force == 0)
				{
					force = touchPoint.position.y - inputStart;
					Debug.Log("Setting force = " + force);
				}
			}
		}
	}

	IEnumerator WaitAndReset() {
		yield return new WaitForSeconds(3);
		Reset ();
	}
}
