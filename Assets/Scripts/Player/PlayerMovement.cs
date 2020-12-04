using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : NetworkBehaviour
{
	[SerializeField]
	private Rigidbody rbody = null;
	[SerializeField]
	private CapsuleCollider coll = null;
	public float speed = 10.0f;
	public float sprintModifier = 3.0f;
	public float gravity = 10.0f;
	public float maxVelocityChange = 10.0f;
	public bool canJump = true;
	public float jumpHeight = 2.0f;
	public bool active = true;
	private bool grounded = false;
	private bool hasJumped = false;
	private bool _isSprinting = false;


	void Awake()
	{
		
		
	}
    private void Start()
    {
		//rbody = GetComponentInChildren<Rigidbody>();
		//coll = GetComponentInChildren<CapsuleCollider>();
		//rbody.freezeRotation = true;
		//rbody.useGravity = false;
		if (hasAuthority)
		{
			//PlayerSpawnSystem.SpawnPlayer(gameObject);
		}
	}
    [Client]
	void FixedUpdate()
	{
		//Debug.Log("Movement");
		if (!hasAuthority)
            return;
		if (active)
		{
			if (grounded)
			{
				// Calculate how fast we should be moving
				Vector3 targetVelocity = new Vector3(Input.GetAxis(Keybinds.Horizontal), 0, Input.GetAxis(Keybinds.Vertical));
				targetVelocity = transform.TransformDirection(targetVelocity);
				targetVelocity *= speed;
				//Debug.Log("Velocity");
				// Apply a force that attempts to reach our target velocity
				Vector3 velocity = rbody.velocity;
				Vector3 velocityChange = (targetVelocity - velocity);
				velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
				velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
				velocityChange.y = 0;
				rbody.AddForce(velocityChange, ForceMode.VelocityChange);

				// Jump
				if (canJump && Input.GetKeyDown(Keybinds.Jump) && !hasJumped)
				{

					rbody.velocity += new Vector3(0, CalculateJumpVerticalSpeed(), 0);


					//rbody.velocity += new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
					hasJumped = true;


				}
			}
			if (Input.GetKeyUp(KeyCode.Return))
			{
				PlayerSpawnSystem.SpawnPlayer(gameObject);
			}
			if (Input.GetKeyDown(Keybinds.Sprint))
			{
				EnableSprint();
			}
			else if (Input.GetKeyUp(Keybinds.Sprint))
			{
				DisableSprint();
			}
		}
		// We apply gravity manually for more tuning control
		rbody.AddForce(new Vector3(0, -gravity * rbody.mass, 0));

		if (rbody.velocity.y == 0)
        {
			hasJumped = false;
        }

		grounded = false;
	}

    public void EnableSprint()
    {
		if (!_isSprinting)
		{
			_isSprinting = !_isSprinting;
			speed *= sprintModifier;
			GetComponent<Shooting>().active = false;
		}
	}

	public void DisableSprint()
    {
		if (_isSprinting)
		{
			_isSprinting = !_isSprinting;
			speed /= sprintModifier;
			GetComponent<Shooting>().active = true;
		}
	}

    void OnCollisionStay()
	{
		grounded = true;
	}

	float CalculateJumpVerticalSpeed()
	{
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}

}
