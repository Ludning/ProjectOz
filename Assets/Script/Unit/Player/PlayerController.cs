using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody Rigidbody;
    [SerializeField] private GroundChecker PlayerGroundChecker;
    private Vector2 _direction;
    [SerializeField] private float _speed = 100f;
    [SerializeField] private float _jumpForce = 10f;
    
    
    [SerializeField] private CharacterMediator CharacterMediator;

    
    
    private void Update()
    {
        Debug.Log(Rigidbody.velocity);
        Vector3 velocity = new Vector3(_direction.x * _speed * Time.deltaTime, Rigidbody.velocity.y, 0);
        Rigidbody.velocity = velocity;
        switch (_direction.x)
        {
            case > 0:
                transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            case < 0:
                transform.rotation = Quaternion.Euler(0, -90, 0);
                break;
        }

        

        CharacterMediatorMessage<int> msg = new CharacterMediatorMessage<int>()
        {
            value = 10
        };
        CharacterMediator.Notify(this, msg);
    }

    #region Input Function
    public void OnMove(InputAction.CallbackContext context)
    {
        _direction = context.ReadValue<Vector2>();
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        
    }
    public void OnStateChange(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            CharacterMediator.Notify(this, new CharacterMediatorMessage<PlayerModelState>());
        }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (PlayerGroundChecker.IsGrounded())
            {
                Rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode.Impulse);
            }
        }
        else if (context.performed)
        {
            
        }
        else if(context.canceled)
        {
            
        }
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("OnAttack");
        }
        else if (context.performed)
        {
            
        }
        else if(context.canceled)
        {
            
        }
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = context.ReadValue<Vector2>();
        Debug.Log($"mousePosition {mousePosition}");
    }
    #endregion
}
