using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System;

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
	public float gravity = 20.0f;
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
	private bool _isAiming = false;

	[SerializeField]
	private bool _isLaunched = false;

	private float launchTime = .25f;
	private float timer;

	[SerializeField]
	private float _maxSprintTime = 3f;
	[SerializeField]
	private float _sprintTime;
	[SerializeField]
	private GameObject _sprintSlider = null;


	[SerializeField]
	private Vector3 vel;

	public Camera MainCamera;
	private float FovSpeed;
	private float StartingFov;

    private Vector3 movementDirection;





	//Distance variable to see if the player is on the ground
	[SerializeField]
	private float _distToGround;


	[SyncVar(hook = nameof(HandleisRunningUpdated))]
	private bool isRunning = false;

    private void HandleisRunningUpdated(bool oldBool, bool newBool)
    {
		isRunning = newBool;
		GetComponentInChildren<Animator>().SetBool("running", isRunning);
	}

    void Awake()
	{
		
		
	}
	private void Start()
	{
		//GetComponent<PlayerMovement>().inRespawnRoom = true

		_distToGround = coll.bounds.extents.y;

		if (hasAuthority)
		{
			//PlayerSpawnSystem.SpawnPlayer(gameObject);
		}
		_sprintTime = _maxSprintTime;
		_sprintSlider.GetComponent<Slider>().maxValue = _maxSprintTime;

		StartingFov = MainCamera.fieldOfView;
		FovSpeed = 3;
	}

	[Client]
    private void Update()
    {
		if (!hasAuthority)
			return;
        if (active)
        {

			//Displaying Vel in inspector for debugging purposes
			vel = rbody.velocity;

			//Resetting the launch state when on ground for long enough 
            if (_isLaunched && grounded)
            {
				timer -= Time.deltaTime;
				if(timer < 0)
                {
					_isLaunched = false;
                }
            }

			//setting the grounded state of the player off of a raycast
			grounded = IsGrounded();
			
			//Handling the amount of sprint the player has
			SprintTimer();

            if (grounded)
            {
                //Single Event Physics can be done in update
                PlayerJumps();

				//Code for sprint modifier
				if (Input.GetKeyDown(Keybinds.Sprint) && _sprintTime > 0)
				{
					EnableSprint();
				}
				else if (Input.GetKeyUp(Keybinds.Sprint) || _sprintTime <= 0)
				{
					DisableSprint();
				}
			}
			if (Input.GetKeyUp(Keybinds.Sprint) || _sprintTime <= 0)
			{
				DisableSprint();
			}


		}

		//Debug control to respawn the player
		if (Input.GetKeyUp(KeyCode.Return))
		{
			PlayerSpawnSystem.SpawnPlayer(gameObject);
		}
	}

    [Client]
	void FixedUpdate()
	{
		if (!hasAuthority)
        {
			return;
		}

		if (active)
		{
			//GetKey or GetAxis physics are done in FixedUpdate
			Movement();
		
		}
		// We apply gravity manually for more tuning control
		rbody.AddForce(-transform.up *  gravity, ForceMode.Acceleration);

        if (rbody.velocity.y == 0)
        {
            hasJumped = false;
			
        }

		Slider tempSlider = _sprintSlider.GetComponent<Slider>();
		if (_sprintTime >= _maxSprintTime)
        {
			_sprintSlider.SetActive(false);
        } else
        {
			_sprintSlider.SetActive(true);
		}
		tempSlider.value = _sprintTime;
	}

    //A Function that takes the player's input and calculates movement
    private void Movement()
    {
		float speedDebuff;
		if (_isAiming)
        {
			speedDebuff = 0.05f;
        }
        else
        {
			speedDebuff = 1.0f;
        }

		//Calculate how fast we should be moving
		movementDirection = new Vector3(Input.GetAxis(Keybinds.Horizontal), 0, Input.GetAxis(Keybinds.Vertical)).normalized;

		//setting local to world coordinates and then adding player speed
		movementDirection = transform.TransformDirection(movementDirection);
		movementDirection *= speed;

		if(movementDirection.magnitude > 0.1)
        {
			isRunning = true;
        }
        else
        {
			isRunning = false;
		}

		/*
		 * Allow movement if the player is grounded or if they are in the air, but not off a launch
		 * Thi allows the player to air strafe when falling normally but since movement is slower than
		 * the launch pad's launch, it can mess up with the launch speed mid air. This prevents air straffing
		 * until the player lands on the ground.
		 */
		if (!_isLaunched || grounded) 
        {
			// Apply a force that attempts to reach our target velocity
			Vector3 velocity = rbody.velocity;
			Vector3 velocityChange = (movementDirection - velocity);
			velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
			velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
			velocityChange.y = 0;
			rbody.AddForce(velocityChange * speedDebuff, ForceMode.VelocityChange);
		}
        else 
		{
			//Allow for velocity to be uncapped

        }
		
	}

	//A Function that draws a raycast below the player to see if it hits the ground beneath the player
	private bool IsGrounded()
    {
		
		Debug.DrawRay(transform.position, -Vector3.up * _distToGround, Color.red);
		return Physics.Raycast(transform.position, -Vector3.up, _distToGround + 0.5f);
	
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
			
			rbody.AddForce(0, CalculateJumpVerticalSpeed(), 0, ForceMode.VelocityChange);
			//rbody.velocity += new Vector3(0, CalculateJumpVerticalSpeed(), 0);
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
				_sprintTime += Time.deltaTime;
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
			SprintFov();
			_sprintTime -= Time.deltaTime;
		}
	}

	//Disables the ability to sprint
	public void DisableSprint()
    {
		if (_isSprinting)
		{
			_isSprinting = !_isSprinting;
			ResetFov();
			speed /= sprintModifier;
			if(!inRespawnRoom)
				GetComponent<Shooting>().active = true;
		}
	}
	

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


	public void Aiming(bool state)
    {
		_isAiming = state;
    }

	public void Launch()
	{
		_isLaunched = true;
		timer = launchTime;
	}

	//call to reset the FOV
	public void ResetFov()
    {
		MainCamera.fieldOfView = StartingFov;
    }

	//increase Fov slighly
	public void SprintFov()
    {
		MainCamera.fieldOfView += FovSpeed;
    }

}
