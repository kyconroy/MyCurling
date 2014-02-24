using UnityEngine;
using System.Collections;

public class FollowStoneScript : MonoBehaviour {
	public Transform target;
	public float distance;

	// Use this for initialization
	void Start () {
	
	}
	

	void Update()
	{
		transform.position = new Vector3 (target.position.x, target.position.y + distance, target.position.z - distance);		
	}
}
