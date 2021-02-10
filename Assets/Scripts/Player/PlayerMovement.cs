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
	public float sprintModifier = 2.0f;
	public float gravity = 10.0f;
	public float maxVelocityChange = 10.0f;
	public bool canJump = true;
	public float jumpHeight = 2.0f;
	public bool active = true;
	public bool inRespawnRoom = false;
	[SerializeField]
	public bool grounded = false;
	private bool hasJumped = false;
	[SerializeField]
	private bool _isSprinting = false;

	[SerializeField]
	private float _maxSprintTime = 3f;
	[SerializeField]
	private float _sprintTime;


	//Distance variable to see if the player is on the ground
	[SerializeField]
	private float _distToGround;


	void Awake()
	{
		
		
	}
    private void Start()
    {
		_distToGround = coll.bounds.extents.y;
		
		if (hasAuthority)
		{
			//PlayerSpawnSystem.SpawnPlayer(gameObject);
		}
		_sprintTime = _maxSprintTime;
	}

    [Client]
	void FixedUpdate()
	{
		//Debug.Log("Movement");
		if (!hasAuthority)
            return;
		if (active)
		{
			grounded = IsGrounded();
			if (grounded)
			{ 
				Movement();
				SprintTimer();
				PlayerJumps();

				if (Input.GetKey(Keybinds.Sprint) && _sprintTime > 0)
				{
					EnableSprint();
				}
                else
                {
					DisableSprint();
                }


			}
			if (!Input.GetKey(Keybinds.Sprint))
			{
				DisableSprint();
			}

			
			if (Input.GetKeyUp(KeyCode.Return))
			{
				PlayerSpawnSystem.SpawnPlayer(gameObject);
			}
			


		}
		// We apply gravity manually for more tuning control
		rbody.AddForce(new Vector3(0, -gravity * rbody.mass, 0));

		if (rbody.velocity.y == 0)
        {
			hasJumped = false;
        }

		
		
		
	}

	//A Function that takes the player's input and calculates movement
	private void Movement()
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
	}
	//A Function that draws a raycast below the player to see if it hits the ground beneath the player
	private bool IsGrounded()
    {
		Debug.DrawRay(transform.position, -Vector3.up * _distToGround, Color.red);
		return Physics.Raycast(transform.position, -Vector3.up, _distToGround + 0.4f);
    }
	
	//Function for allowing the player to jump
	private void PlayerJumps()
    {
		/*
		 * Allow player to jump when they are near the ground
		 * However, this applies to when the player jumps up
		 * This check only allows the player to jump when they are falling near the ground
		 */
		if (rbody.velocity.y < 0)
		{
			canJump = true;
		}

		//Jump check and jump functionality
		if (canJump && Input.GetKeyDown(Keybinds.Jump))
		{
			canJump = false;
			hasJumped = true;
			//rbody.AddForce(0, gravity * 0.5f * jumpHeight, 0, ForceMode.Impulse);
			Debug.Log("Jumped " + jumpHeight);
			rbody.velocity += new Vector3(0, CalculateJumpVerticalSpeed(), 0);
		}
	}

	//Function for handling the amount of time the player can sprint
	private void SprintTimer()
    {
        if (_isSprinting)
        {
			_sprintTime -= Time.deltaTime;
        }
        else
        {
			if(_sprintTime < _maxSprintTime)
            {
				_sprintTime += Time.deltaTime / 2;
            }
        }
    }
	
	//Enables the player to sprint
	public void EnableSprint()
    {
		if (!_isSprinting)
		{
			_isSprinting = !_isSprinting;
			speed *= sprintModifier;
			GetComponent<Shooting>().active = false;
			_sprintTime -= Time.deltaTime;
		}
	}

	//Disables the ability to sprint
	public void DisableSprint()
    {
		if (_isSprinting)
		{
			_isSprinting = !_isSprinting;
			speed /= sprintModifier;
			if(!inRespawnRoom)
				GetComponent<Shooting>().active = true;
		}
	}

	/*
    void OnCollisionStay()
	{
		grounded = true;
	}*/
	

	float CalculateJumpVerticalSpeed()
	{
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}

	public void SetMoveSpeed(float moveSpeed)
    {
		speed = moveSpeed;
    }

	public void SetJumpHeight(float jump)
    {
		jumpHeight = jump;
    }
}
