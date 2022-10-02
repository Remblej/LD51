using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour {

    [SerializeField] private TimerController timerController;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private Animator animator;
    [SerializeField] public float speed = 3f;
    [SerializeField] public float acceleration = .1f;
    
    private Vector2 _targetVelocity = Vector2.zero;
    private static readonly int Velocity = Animator.StringToHash("velocity");
    private bool _reversedControls = false;
    private bool _disabledControls = false;

    public void ReverseControls() {
        _reversedControls = true;
    }

    public void DisableControls() {
        _disabledControls = true;
    }

    private void Update() {
        if (_disabledControls) return;
        
        var input = ReadInput();
        var flipX = spriteRenderer.flipX;
        flipX = input.x switch {
            < 0 when !flipX => true,
            > 0 when flipX => false,
            _ => flipX
        };

        if (spriteRenderer.flipX != flipX) {
            spriteRenderer.flipX = flipX;
        }
 
        animator.SetFloat(Velocity, playerRb.velocity.magnitude);
    }

    private void FixedUpdate() {
        if (_disabledControls) return;
        
        _targetVelocity = ReadInput() * speed; 
        
        var velocity = playerRb.velocity;
        if (Vector3.Distance(_targetVelocity, velocity) <= acceleration) {
            velocity = _targetVelocity;
        }
        else {
            var moveDir = (_targetVelocity - velocity).normalized;
            var clampedChange = Vector2.ClampMagnitude(moveDir * acceleration, speed);
            velocity += clampedChange;
        }
        
        playerRb.velocity = velocity;
    }
    
    private Vector2 ReadInput() {
        return _reversedControls ? 
            new Vector2(-Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical")) : 
            new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

}