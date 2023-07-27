using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMotor : MonoBehaviour //make abstract?
{
    public Transform root;
    public Transform head;
    public Transform leftLeg;
    public Transform rightLeg;
    public Transform leftFoot;
    public Transform rightFoot;

    public AudioSource walkingSource; //todo:move into player reference script
    public AudioSource collisionSource;

    public bool isJumping = false;
    public bool isFlopping = false;
    public bool isSneaking = false;

    [SerializeField] protected float moveSpeed;

    public float inputSpeed;

    public bool[] otherInputs = { false, false, false };

    public bool isAcceptingInput = true;

    [HideInInspector] public Quaternion rotation = Quaternion.identity;

    public Player owner;

    [SerializeField][Range(0, 1)] float predictionDampner = 1;

    private Transform walkingEffect;

    public virtual void Init(Player owner)
    {
        this.owner = owner;
        owner.walkingAudioSource = walkingSource;
        owner.collidingAudioSource = collisionSource;
        walkingEffect = owner.walkingDustParticles.transform;
        predictionDampner = owner.IsLocal ? predictionDampner : 1;
    }


    protected virtual void FixedUpdate()
    {
        Vector3 direction = rightFoot.position - leftFoot.position;

        walkingEffect.position = leftFoot.position + (direction * 0.5f); //move down
    }

    public virtual void OnMove(InputAction.CallbackContext value)
    {
    }

    public virtual void OnJump(InputAction.CallbackContext value)
    {
    }

    public virtual void OnFlop(InputAction.CallbackContext value)
    {
    }

    public void OnUse(InputAction.CallbackContext value)
    {
    }

    public void OnReady(InputAction.CallbackContext value)
    {
    }

    public void ToggleCustomisation(InputAction.CallbackContext value)
    {
    }

    public void ToggleGameRules(InputAction.CallbackContext value)
    {
    }

    public void TogglePause(InputAction.CallbackContext value)
    {
    }

    public void SetInputs(float _inputSpeed, bool[] _otherInputs, Quaternion _rotation)
    {
        inputSpeed = _inputSpeed;
        rotation = _rotation;
        otherInputs = _otherInputs;
    }
    public void SetPlayerPositions(Vector3 _headPos, Vector3 _rightFootPos, Vector3 _leftFootPos, Vector3 _rightLegPos, Vector3 _leftLegPos)
    {
        root.position = _headPos;
        rightFoot.position = _rightFootPos;
        leftFoot.position = _leftFootPos;
        rightLeg.position = _rightLegPos;
        leftLeg.position = _leftLegPos;
    }
    public void SetPlayerRotations(Quaternion _rightFootRot, Quaternion _leftFootRot, Quaternion _rightLegRot, Quaternion _leftLegRot)
    {
        rightFoot.rotation = _rightFootRot;
        leftFoot.rotation = _leftFootRot;
        rightLeg.rotation = _rightLegRot;
        leftLeg.rotation = _leftLegRot;
    }

    public void SetRootRotation(Quaternion _rootRotation)
    {
        root.rotation = _rootRotation;
    }

    public virtual bool GetCanKnockOutOthers() { return false; }
    public virtual void SetCanKnockOutOthers(bool canKnockOutOthers) { }

    public virtual void SetForwardForceMultiplier(float multiplier) { }

    public virtual void OnCollisionWithOther(float duration) { }

    public virtual SimplePlayerController GetMovementController() { return null; }
}
