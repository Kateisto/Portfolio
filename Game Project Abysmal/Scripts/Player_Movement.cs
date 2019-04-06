using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour {

	[SerializeField]
	private float _speed = 10.0f;
	private Vector3 _moveDirection;
	private Vector3 _mouseDirection;
	[SerializeField]
	private float _jumpForce = 4.0f;
	[SerializeField]
	private int _rotationSens = 100;
	private float _angle = 0;
	Rigidbody rb;
	Object_Follow _followObject;
	private int _groundLayer;
	private Vector3 _yVelFix;


	void Start ()
	{
		rb = GetComponent<Rigidbody>();
		_groundLayer = LayerMask.GetMask ("Ground");
		Cursor.visible = false;
	}
		
	void OnCollisionExit(Collision col)
	{
		//Jos _followObjecti:lla on arvo(OnCollisionEnteristä) niin verrataan siihen kuuluvaa gameobjektia, että onko se sama gameobjekti, jonka collisionista poistutaan
		if (_followObject != null && _followObject.gameObject == col.gameObject) 
		{
			_followObject = null;
		}
	}

	void OnCollisionEnter(Collision col)
	{	
		//obj = kontaktissa oltavan objektin Object_Follow scripti
		var obj = col.gameObject.GetComponent<Object_Follow> ();
		//Jos obj tulos ei ole null, sijoitetaan _followObject:iin kontaktissa oltavan objektin Object_Follow scripti
		if (obj != null) 
		{
			_followObject = obj;
		}
	}

	void Update()
	{
		float _moveHorizontal = Input.GetAxis ("Horizontal");
		float _moveVertical = Input.GetAxis ("Vertical");
		float _mouseRotate = Input.GetAxisRaw ("Mouse X");

		_angle += _mouseRotate * Time.deltaTime * _rotationSens;
		_moveDirection = (_moveHorizontal * Vector3.right + _moveVertical * Vector3.forward);
		_moveDirection = GetAngleRotation() * _moveDirection;

		if (Input.GetKeyDown (KeyCode.Space))
		{
			Jump ();
		}
	}
		
	void FixedUpdate ()
	{
		_yVelFix = new Vector3 (0, rb.velocity.y, 0);
		rb.velocity = _moveDirection * _speed;
		rb.velocity += _yVelFix;

		//Lisätään hahmon velocityyn _followObject:in sisältämä velocity arvo
		if (_followObject != null)
		{
			
			rb.velocity += _followObject.Velocity;
		}
	}

	void Jump()
	{
		if (Physics.Raycast (transform.position, Vector3.down, 0.6f, _groundLayer))
		{
			rb.AddForce (Vector3.up * _jumpForce, ForceMode.Impulse);
		}
	
		else if (Physics.Raycast (transform.position + Vector3.back * 0.49f, Vector3.down, 0.6f, _groundLayer))
		{
			rb.AddForce (Vector3.up * _jumpForce, ForceMode.Impulse);
		}
	
		else if (Physics.Raycast (transform.position + Vector3.forward * 0.49f, Vector3.down, 0.6f, _groundLayer))
		{
			rb.AddForce (Vector3.up * _jumpForce, ForceMode.Impulse);
		}
	
		else if (Physics.Raycast (transform.position + Vector3.right * 0.49f, Vector3.down, 0.6f, _groundLayer))
		{
			rb.AddForce (Vector3.up * _jumpForce, ForceMode.Impulse);
		}
	
		else if (Physics.Raycast (transform.position + Vector3.left * 0.49f, Vector3.down, 0.6f, _groundLayer))
		{
			rb.AddForce (Vector3.up * _jumpForce, ForceMode.Impulse);
		}
	}

	internal Quaternion GetAngleRotation()
	{
		return Quaternion.AngleAxis (_angle, Vector3.up);
	}
}