using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightControl : MonoBehaviour
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

    private float _rushSpeed;
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

    private void Awake()
    {
        _rushSlashData_Skill = DataManager.Instance.GetGameData<SkillData>("S103");

        col = GetComponent<BoxCollider>();
        col.enabled = false;

        _playerPos = player.gameObject.transform.position;

        _rushSpeed = 10.0f;
        _rushDistance = _rushSlashData_Skill.value2;
        _rushCoolDown = _rushSlashData_Skill.skillCooltime;
        _rushCoolDownHit = _rushSlashData_Skill.value1;
        _damage = _rushSlashData_Skill.skillPowerRate;
    }

    private void Update()
    {
        //Debug.Log(Vector2.Distance(transform.position, _targetPos));

        if (isOnCoolDown)
        {
            _currentRushCoolDown -= Time.unscaledDeltaTime;
            if (_currentRushCoolDown <= 0f)
            {
                isOnCoolDown = false;
            }
        }

        if (Input.GetMouseButtonDown(0) && !isOnCoolDown && !isRush)
        {
            SettingTargetPos();
        }
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
            RushSlash();
        }
    }

    private void SettingTargetPos()
    {
        _mousePos = Input.mousePosition;
        _changedWorldPos = Camera.main.ScreenToWorldPoint(_mousePos - new Vector2(0f, player.transform.lossyScale.y / 2));

        _direction = (_changedWorldPos - (Vector2)player.transform.position).normalized;
        _targetPos = (Vector2)player.transform.position + _direction * _rushDistance;
        player.SetMovementDirection(_direction);
        _currentRushCoolDown = _rushCoolDown;
        isRush = true;
        isOnCoolDown = true;
    }

    private void RushSlash()
    {
        rb.velocity = _direction * _rushSpeed;

        if (Vector2.Distance(player.transform.position, _targetPos) < 0.1f)
        {
            isRush = false;
            rb.velocity = Vector3.zero; // 속도를 0으로 설정하여 이동 멈춤
            col.enabled = false;
            timer = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && isRush)
        {
            _currentRushCoolDown = _rushCoolDownHit;
        }
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Platform") || other.gameObject.CompareTag("Wall"))
        {
            //if (other.gameObject.layer == LayerMask.NameToLayer("Terrain_Impassable"))
            isRush = false;
            rb.velocity = Vector3.zero;
            col.enabled = false;
            timer = 0f;
        }
    }
}
