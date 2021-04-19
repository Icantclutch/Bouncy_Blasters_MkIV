using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerAnimationController : NetworkBehaviour
{
    public float animChangeSpeed = 0.4f;
    private float _targetXVel = 0;
    private float _targetYVel = 0;

    [SyncVar(hook = nameof(XVelocityUpdated))]
    float _xVelocity;
    private void XVelocityUpdated(float oldVel, float newVel)
    {
        _xVelocity = newVel;
        GetComponentInChildren<Animator>().SetFloat("xVel", _xVelocity);
    }

    [SyncVar(hook = nameof(YVelocityUpdated))]
    float _yVelocity;
    private void YVelocityUpdated(float oldVel, float newVel)
    {
        _yVelocity = newVel;
        GetComponentInChildren<Animator>().SetFloat("yVel", _yVelocity);
    }

    //Sets the target velocity that the animation blend tree will use
    public void SetVelocity(float x, float y)
    {
        _targetXVel = x;
        _targetYVel = y;
    }

    [SyncVar(hook = nameof(UsingPistolUpdated))]
    private bool _usingPistol = false;

    private void UsingPistolUpdated(bool oldBool, bool newBool)
    {
        _usingPistol = newBool;
        GetComponentInChildren<Animator>().SetBool("pistol", _usingPistol);
    }

    public void SetUsingPistol(bool newBool)
    {
        _usingPistol = newBool;
    }
    [SyncVar(hook = nameof(FallingUpdated))]
    private bool _falling = false;

    private void FallingUpdated(bool oldBool, bool newBool)
    {
        _falling = newBool;
        GetComponentInChildren<Animator>().SetBool("falling", _falling);
    }

    public void SetFalling(bool newBool)
    {
        _falling = newBool;
    }

    [ClientRpc]
    public void SetJump()
    {
        GetComponentInChildren<Animator>().SetTrigger("jump");
    }

    [SyncVar(hook = nameof(IsRunningUpdated))]
    private bool isRunning = false;

    private void IsRunningUpdated(bool oldBool, bool newBool)
    {
        isRunning = newBool;
        GetComponentInChildren<Animator>().SetBool("running", isRunning);
    }

    public void SetIsRunning(bool isRun)
    {
        isRunning = isRun;
    }

    //slowly change velocity used by the animation until it is close the the target velocity
    private void FixedUpdate()
    {
        
        if(Mathf.Abs(_xVelocity - _targetXVel) > animChangeSpeed)
        {
            _xVelocity += Mathf.Sign(_targetXVel - _xVelocity) * animChangeSpeed;
        }

        if (Mathf.Abs(_yVelocity - _targetYVel) > animChangeSpeed)
        {
            _yVelocity += Mathf.Sign(_targetYVel - _yVelocity) * animChangeSpeed;
        }
    }
}
