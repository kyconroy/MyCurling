using UnityEngine;
using System.Collections;

public class WallScript : MonoBehaviour {
	
	void OnCollisionEnter(Collision	collision)
	{
		string hitObject = transform.gameObject.tag;
		if (hitObject == "Player") 
		{
			Debug.Log("Hit player");
			transform.gameObject.rigidbody.velocity = new Vector3(0,0,0);
			Wait(3f);

			StoneBehavior stone = transform.gameObject.GetComponent<StoneBehavior> ();
			stone.Reset();
		}

		Debug.Log("Hit " + transform.gameObject.name);
	}

	IEnumerator Wait(float wait) {
		yield return new WaitForSeconds(wait);
	}
}
