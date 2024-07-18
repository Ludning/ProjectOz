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

    private int _defaultLayer;
    private int _rushSlashLayer;

    [SerializeField] private bool isRush = false;
    [SerializeField] private bool isOnCoolDown = false;
    [SerializeField] private Animator knightAnimator;
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
    private void FixedUpdate()
    {  
        if(isRush)
        {
            timer += Time.deltaTime;
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
        //player.PlayerMovement.OnInputSetDirection(_direction);
        _currentRushCoolDown = _rushCoolDown;
        isRush = true;
        isOnCoolDown = true;
        //knightAnimator.SetBool(HashAttack, true);
        //rb.useGravity = false;
        //rb.velocity = Vector3.zero;
        //rb.AddForce(_direction * _rushForce, ForceMode.Impulse);

        player.PlayerMovement.StartRushSlash(_direction, _rushForce, _rushDistance, EndRushSlash);
    }
    /*private void OnUpdateRushSlash()
    {
        float distance = Vector2.Distance(player.transform.position, _targetPos);
        if (distance < 0.2f)
            EndRushSlash();
    }*/

    public void EndRushSlash()
    {
        isRush = false;
        //rb.velocity = Vector3.zero; // 속도를 0으로 설정하여 이동 멈춤
        //rb.useGravity = true;
        col.enabled = false;
        timer = 0f;
        //knightAnimator.SetBool(HashAttack, false);
        
        player.PlayerMovement.EndRushSlash();
    }

    /*private void SettingTargetPos()
    {
        _mousePos = Input.mousePosition;
        _changedWorldPos = Camera.main.ScreenToWorldPoint(_mousePos - new Vector2(0f, player.transform.lossyScale.y / 2));

        _direction = (_changedWorldPos - (Vector2)player.transform.position).normalized;
        _targetPos = (Vector2)player.transform.position + _direction * _rushDistance;
        player.playerModelController.OnInputSetDirection(_direction);
        player.PlayerMovement.OnInputSetDirection(_direction);
        _currentRushCoolDown = _rushCoolDown;
        isRush = true;
        knightAnimator.SetBool(HashAttack, true);
        //rb.AddForce();
        rb.AddForce(_direction * _rushSpeed, ForceMode.Impulse);
    }*/
    /*private void RushSlash()
    {
        rb.velocity = _direction * _rushSpeed;

        if (Vector2.Distance(player.transform.position, _targetPos) < 0.1f)
        {
            isRush = false;
            isOnCoolDown = true;
            rb.velocity = Vector3.zero; // 속도를 0으로 설정하여 이동 멈춤
            col.enabled = false;
            timer = 0f;
            knightAnimator.SetBool(HashAttack, false);
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && isRush)
        {
            _currentRushCoolDown = _rushCoolDownHit;
        }
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Platform") || other.gameObject.CompareTag("Wall"))
        {
            //if (other.gameObject.layer == LayerMask.NameToLayer("Terrain_Impassable"))
            isOnCoolDown = true;
            EndRushSlash();
        }
    }

    public void OnInput(KeyType type)
    {
        if (type == KeyType.KeyDown && !isOnCoolDown && !isRush)
        {
            StartRushSlash();
        }
    }
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
