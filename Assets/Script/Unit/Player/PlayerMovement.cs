using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 _direction;
    [SerializeField] private float _speed = 100f;
    [SerializeField] private float _jumpForce = 10f;
    
    private PcData _mageData;
    private PcData _knightData;

    [SerializeField] private CharacterMediator CharacterMediator;
    [SerializeField] private Rigidbody Rigidbody;
    
    private void Update()
    {
        Vector3 velocity = new Vector3(_direction.x * _speed * Time.unscaledDeltaTime, Rigidbody.velocity.y, 0);
        Rigidbody.velocity = velocity;
        

        //CharacterMediator;
    }

    public void SetDirection(Vector2 direction)
    {
        _direction = direction;
        RotationCharacter(direction);
    }

    private void RotationCharacter(Vector2 direction)
    {
        switch (direction.x)
        {
            case > 0:
                transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            case < 0:
                transform.rotation = Quaternion.Euler(0, -90, 0);
                break;
        }
    }

    public void OnJump()
    {
        Rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode.Impulse);
    }
}
