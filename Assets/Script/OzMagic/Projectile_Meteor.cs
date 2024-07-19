using UnityEngine;

public class Projectile_Meteor : OzMagic
{
    private OzmagicData _meteorData_OzMagic;
    private ProjectileData _meteorData_Projectile;

    private Rigidbody _rb;

    private Camera _camera;

    private Vector3 _spawnPos;


    private float _hitCycle;
    private float _gravity;
    private float _rotSpeed;
    private float _damageTimerPlayer;
    private float _damageTimerEnemy;
    private float _damage;
    private float _dotDamage;

    private void Awake()
    {
        _meteorData_OzMagic = DataManager.Instance.GetGameData<OzmagicData>("O101");
        _meteorData_Projectile = DataManager.Instance.GetGameData<ProjectileData>("P201");

        _rb = GetComponent<Rigidbody>();

        _ozMagicPercentage = _meteorData_OzMagic.ozSkillPercentage;

        _damage = _meteorData_OzMagic.ozPowerRate;
        _dotDamage = _meteorData_OzMagic.value1;
        _hitCycle = _meteorData_OzMagic.value2;
        _lifeTime = _meteorData_Projectile.projectileLifeTime;
        _gravity = _meteorData_Projectile.projectileSpeedRate;
        _rotSpeed = 50.0f;
    }


    protected override void OnEnable()
    {
        base.OnEnable();

        _camera = Camera.main;
        _spawnPos = _camera.gameObject.transform.position + new Vector3(0.0f, _camera.orthographicSize + transform.lossyScale.y / 2, -_camera.gameObject.transform.position.z);
        transform.position = _spawnPos;
        _damageTimerPlayer = 0f;
        _damageTimerEnemy = 0f;
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
            OzMagicManager.Instance.MeteorExplosion.isPlayerHit = false;
            OzMagicManager.Instance.OnExplosion(transform.position);
            DestroyOzMagic();
            //OzMagicManager.Instance.MeteorExplosion.is
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Player_RushSlash"))
        {
            OzMagicManager.Instance.MeteorExplosion.isPlayerHit = true;
            OzMagicManager.Instance.OnExplosion(transform.position);
            DestroyOzMagic();
        }
        if (other.CompareTag("Enemy")|| other.gameObject.CompareTag("Player"))
        {
            if (other.TryGetComponent(out Combat combat))
            {
                combat.Damaged(_damage);
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _damageTimerPlayer += Time.deltaTime;
            if (_damageTimerPlayer >= _hitCycle)
            {

                if (other.TryGetComponent(out Combat combat))
                {
                    combat.Damaged(_dotDamage);
                    _damageTimerPlayer = 0;
                }
            }
        }
        if (other.gameObject.CompareTag("Enemy"))
        {
            _damageTimerEnemy += Time.deltaTime;
            if (_damageTimerEnemy >= _hitCycle)
            {
                if (other.TryGetComponent(out Combat combat))
                {
                    combat.Damaged(_dotDamage);
                    _damageTimerEnemy = 0;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _damageTimerPlayer = 0f;
        }
        if (other.gameObject.CompareTag("Enemy"))
        {
            _damageTimerEnemy = 0f;
        }
    }
}
