using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Follow : MonoBehaviour {

	Vector3 velocity;
	Vector3 prevPos;

	public Vector3 Velocity { get { return velocity; } }


	void Update ()
	{
		transform.position = new Vector3 (transform.position.x, 0, transform.position.z);
		velocity = (transform.position - prevPos) / Time.deltaTime;
		prevPos = transform.position;
	}
}
