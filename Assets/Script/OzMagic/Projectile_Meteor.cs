using UnityEngine;

public class Projectile_Meteor : OzMagic
{
    private OzmagicData _meteorData_OzMagic;
    private ProjectileData _meteorData_Projectile;

    private Rigidbody _rb;

    private Camera _camera;

    private Vector3 _spawnPos;

    private float _damageTimer;
    private float _hitCycle;
    private float _meteorDamage;
    private float _gravity;
    private float _rotSpeed;

    private void Awake()
    {
        _meteorData_OzMagic = DataManager.Instance.GetGameData<OzmagicData>("O101");
        _meteorData_Projectile = DataManager.Instance.GetGameData<ProjectileData>("P201");

        _rb = GetComponent<Rigidbody>();

        _ozMagicPercentage = _meteorData_OzMagic.ozSkillPercentage;

        _lifeTime = _meteorData_Projectile.projectileLifeTime;
        _gravity = _meteorData_Projectile.projectileSpeedRate;
        _rotSpeed = 50.0f;
    }


    protected override void OnEnable()
    {
        base.OnEnable();

        _camera = Camera.main;
        _spawnPos = _camera.gameObject.transform.position + new Vector3(0.0f, _camera.orthographicSize, -_camera.gameObject.transform.position.z);
        transform.position = _spawnPos;
        _damageTimer = 0f;
        CancelInvoke(nameof(DestroyOzMagic));
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, _rotSpeed * Time.deltaTime));
    }

    public override void Excute()
    {
        _rb.velocity = new Vector3(0, -_gravity, 0);
        Invoke(nameof(DestroyOzMagic), _lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            DestroyOzMagic();
        }           
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _damageTimer += Time.deltaTime;
            if (_damageTimer >= _hitCycle)
            {
                // todo
                // 플레이어 데미지
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _damageTimer = 0f;
        }
    }
}
