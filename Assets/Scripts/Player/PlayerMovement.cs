using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : NetworkBehaviour
{
	private Rigidbody rbody;
	private CapsuleCollider coll;
	public float speed = 10.0f;
	public float gravity = 10.0f;
	public float maxVelocityChange = 10.0f;
	public bool canJump = true;
	public float jumpHeight = 2.0f;
	private bool grounded = false;



	void Awake()
	{
		rbody = GetComponent<Rigidbody>();
		coll = GetComponent<CapsuleCollider>();
		rbody.freezeRotation = true;
		rbody.useGravity = false;
		
	}
    private void Start()
    {
		if (hasAuthority)
		{
			PlayerSpawnSystem.SpawnPlayer(gameObject);
		}
	}
    [Client]
	void FixedUpdate()
	{
		if (!hasAuthority)
            return;
		if (grounded)
		{
			// Calculate how fast we should be moving
			Vector3 targetVelocity = new Vector3(Input.GetAxis(Keybinds.Horizontal), 0, Input.GetAxis(Keybinds.Vertical));
			targetVelocity = transform.TransformDirection(targetVelocity);
			targetVelocity *= speed;

			// Apply a force that attempts to reach our target velocity
			Vector3 velocity = rbody.velocity;
			Vector3 velocityChange = (targetVelocity - velocity);
			velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
			velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
			velocityChange.y = 0;
			rbody.AddForce(velocityChange, ForceMode.VelocityChange);

			// Jump
			if (canJump && Input.GetKeyDown(Keybinds.Jump))
			{
				rbody.velocity += new Vector3(0, CalculateJumpVerticalSpeed(), 0);
			}
		}
        if (Input.GetKeyUp(KeyCode.Return))
        {
			PlayerSpawnSystem.SpawnPlayer(gameObject);
		}
		// We apply gravity manually for more tuning control
		rbody.AddForce(new Vector3(0, -gravity * rbody.mass, 0));

		grounded = false;
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
