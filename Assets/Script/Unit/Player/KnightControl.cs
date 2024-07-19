using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KnightControl : MonoBehaviour, IControl
{
    private SkillData _rushSlashData_Skill;

    [SerializeField] private CharacterMediator player;
    [SerializeField] private Rigidbody rb;
    private BoxCollider col;

    private Vector2 _playerPos;

    private Vector2 _targetPos;
    private Vector2 _direction;
    private Vector2 _changedWorldPos;
    private Vector2 _mousePos;

    [SerializeField] private float _rushForce;
    [SerializeField] private float _rushDistance;
    [SerializeField] private float _rushCoolDown;
    [SerializeField] private float _rushCoolDownHit;
    [SerializeField] private float _currentRushCoolDown;
    private float _damage;
    private float timer;
	private int _jumpCount;
    [SerializeField] private bool isRush = false;
    [SerializeField] private bool isOnCoolDown = false;
    [SerializeField] private Animator knightAnimator;
    [SerializeField] private Transform rushSlashVfxRigger;
    [SerializeField] private VfxControl rushSlashVfxControl;
    private static readonly int HashAttack = Animator.StringToHash("IsAttack");

    private void Awake()
    {
        _rushSlashData_Skill = DataManager.Instance.GetGameData<SkillData>("S103");

        col = GetComponent<BoxCollider>();
        col.enabled = false;

        _playerPos = player.gameObject.transform.position;
        _rushDistance = _rushSlashData_Skill.value2;
        _rushCoolDown = _rushSlashData_Skill.skillCooltime;
        _rushCoolDownHit = _rushSlashData_Skill.value1;
        _damage = _rushSlashData_Skill.skillPowerRate;
    }

    private void Update()
    {
        if (_jumpCount > 0)
        {
            if (player.IsGround == true)
                _jumpCount = 0;
        }
    }

    private void FixedUpdate()
    {  
        if(isRush)
        {
            timer += Time.unscaledDeltaTime;
            if (timer >= 0.1f)
            {
                col.enabled = true;
            }
            //OnUpdateRushSlash();
        }
    }
    
    public void StartRushSlash()
    {
        _mousePos = Mouse.current.position.ReadValue();
        _changedWorldPos = Camera.main.ScreenToWorldPoint(_mousePos - (Vector2)player.transform.position);
        Debug.Log(_changedWorldPos);
        _direction = (_changedWorldPos - (Vector2)player.transform.position).normalized;
        _targetPos = (Vector2)player.transform.position + _direction * _rushDistance;
        
        player.playerModelController.OnInputSetDirection(_direction);
        _currentRushCoolDown = _rushCoolDown;
        isRush = true;
        isOnCoolDown = true;
        
        rushSlashVfxControl.transform.SetParent(rushSlashVfxRigger.transform);
        rushSlashVfxControl.transform.localPosition = Vector3.zero;
        rushSlashVfxControl.StartParticleNonLIfeTime();

        player.playerCombat.IsInvincibility = true;
        
        player.PlayerMovement.StartRushSlash(_direction, _rushForce, _rushDistance, EndRushSlash);
    }

    public void EndRushSlash()
    {
        isRush = false;
        col.enabled = false;
        timer = 0f;
        
        rushSlashVfxControl.transform.parent = null;
        rushSlashVfxControl.StopParticle();
        
        player.playerCombat.IsInvincibility = false;
        
        player.PlayerMovement.EndRushSlash();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && isRush)
        {
            _currentRushCoolDown = _rushCoolDownHit;
            if (other.TryGetComponent(out Combat combat))
                combat.Damaged(_damage);
        }
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Platform") || other.gameObject.CompareTag("Wall"))
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Terrain_Impassable"))
            {
                isOnCoolDown = true;
                EndRushSlash();
            }
        }
        if (other.gameObject.CompareTag("Meteor"))
        {
            if (player.gameObject.layer == player.PlayerMovement._rushSlashLayerNum)
            {
                _currentRushCoolDown = _rushCoolDownHit;
                EndRushSlash();
            }
        }
    }

    public void OnInputAttack(KeyType type)
    {
        if (type == KeyType.KeyDown && !isOnCoolDown && !isRush)
        {
            StartRushSlash();
        }
    }

    public void OnInputJump(KeyType type)
    {
        if(type == KeyType.KeyDown)
            StartJump();
    }

    #region Jump
    private void StartJump()
    {
        if (player.IsGround == true && _jumpCount < 2)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector2.up * player.PlayerMovement.JumpForce, ForceMode.Impulse);
            _jumpCount = 1;
        }
        else if (player.IsGround == false && _jumpCount < 2)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector2.up * player.PlayerMovement.JumpForce, ForceMode.Impulse);
            _jumpCount = 2;
        }
    }
    #endregion
    #region Update Action
    private void RefreshCoolDown()
    {
        if (isOnCoolDown)
        {
            _currentRushCoolDown -= Time.unscaledDeltaTime;
            if (_currentRushCoolDown <= 0f)
            {
                isOnCoolDown = false;
            }
        }
    }
    #endregion
    private void OnEnable()
    {
        TimeManager.Instance.RegistCooldownAction(RefreshCoolDown);
    }
    private void OnDisable()
    {
        TimeManager.Instance.DeregistCooldownAction(RefreshCoolDown);
    }
}
