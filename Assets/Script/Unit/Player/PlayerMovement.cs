using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody Rigidbody;
    [SerializeField] private GroundChecker PlayerGroundChecker;
    private Vector2 _direction;
    [SerializeField] private float _speed = 100f;
    [SerializeField] private float _jumpForce = 10f;

    [SerializeField] private CharacterMediator CharacterMediator;
    [SerializeField] private ScaleController ScaleController;
    
    private bool _isStop = false;


    private void Update()
    {
        Vector3 velocity = new Vector3(_direction.x * _speed * Time.unscaledDeltaTime, Rigidbody.velocity.y, 0);
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
        _direction = context.ReadValue<Vector2>().normalized;
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        
    }
    public void OnStateChange(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            CharacterMediator.Notify(this, new CharacterMediatorMessage<PlayerStatControlType>() { value = PlayerStatControlType.ChangeTransformation});
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
            if (!_isStop)
            {
                ScaleController.PauseAllExceptPlayer(gameObject);
                _isStop = !_isStop;
            }
            else
            {
                ScaleController.ResumeAll();
                _isStop = !_isStop;
            }
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
        //Debug.Log($"mousePosition {mousePosition}");
    }
    #endregion

    
}
