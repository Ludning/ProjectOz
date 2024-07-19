using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ScaleController : OzMagic
{
    private OzmagicData _timeStopData_OzMagic;

    PlayerModelController pmc;
    Rigidbody[] allRigidbodies;
    Animator[] allAnimators;
    NavMeshAgent[] allNavMeshAgents;
    BehaviorDesigner.Runtime.BehaviorTree[] allBTs;

    [SerializeField] float ozTimestopDuration; //current
    [SerializeField] float ozTimestopRate;
    [SerializeField] float ozTimestopChainDuration;
    float _times = 0f;

    Material originalSkybox;
    Material timeStopSkybox;

    private void Awake()
    {
        originalSkybox = RenderSettings.skybox;
        timeStopSkybox = ResourceManager.Instance.LoadResource<Material>("TimeStopSkybox");
        _timeStopData_OzMagic = DataManager.Instance.GetGameData<OzmagicData>("O201");

        _ozMagicPercentage = _timeStopData_OzMagic.ozSkillPercentage;
        _lifeTime = _timeStopData_OzMagic.value1;
        ozTimestopRate = _timeStopData_OzMagic.value2;
        ozTimestopChainDuration = _timeStopData_OzMagic.value3;

        SearchAllMovement();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        pmc = FindFirstObjectByType<PlayerModelController>();
    }

    public override void Excute()
    {
        if (gameObject.activeInHierarchy)
        {
            ChangeSkybox(timeStopSkybox);
            StartCoroutine(CycleOfTimeStop());
        }
    }

    IEnumerator CycleOfTimeStop()
    {
        bool addTime = false;
        _times += ozTimestopDuration;


        Time.timeScale = ozTimestopRate;
        while (_times > 0f)
        {
            yield return null;

            _times -= Time.unscaledDeltaTime;
            if (pmc.CurrentModelState == PlayerModelState.Knight && !addTime)
            {
                OnStop(true);
                Time.timeScale = 1f;
                _times += ozTimestopChainDuration;
                addTime = true;
            }
        }

        OnStop(false);
        Time.timeScale = 1f;
        ChangeSkybox(originalSkybox);
        DestroyOzMagic();
        yield break;
    }

    public void SearchAllMovement()
    {
        allRigidbodies = FindObjectsOfType<Rigidbody>();
        allAnimators = FindObjectsOfType<Animator>();
        allNavMeshAgents = FindObjectsOfType<NavMeshAgent>();
        allBTs = FindObjectsOfType<BehaviorDesigner.Runtime.BehaviorTree>();
    }

    public void OnStop(bool isStop)
    {
        if (isStop)
        {
            PauseAllExceptPlayer(pmc.gameObject);
        }
        else
        {
            ResumeAll();
        }
    }

    private void PauseAllExceptPlayer(GameObject gameObject)
    {
        // Rigidbody 
        foreach (Rigidbody rb in allRigidbodies)
        {
            if (rb.gameObject != gameObject)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }

        // Animator 
        foreach (Animator animator in allAnimators)
        {
            bool isPlayer = animator.gameObject == gameObject ||
            animator.gameObject == gameObject.transform.GetChild(0) ||
            animator.gameObject == gameObject.transform.GetChild(1);

            if (!isPlayer)
            {
                animator.enabled = false;
            }
        }

        foreach (BehaviorDesigner.Runtime.BehaviorTree bts in allBTs)
        {
            if (bts.gameObject != gameObject)
            {
                bts.enabled = false;
            }
        }

        // NavMeshAgent 
        foreach (NavMeshAgent agent in allNavMeshAgents)
        {
            if (agent.gameObject != gameObject)
            {
                agent.isStopped = true;
            }
        }
    }

    private void ResumeAll()
    {
        // Rigidbody
        foreach (Rigidbody rb in allRigidbodies)
        {
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }

        foreach (Animator animator in allAnimators)
        {
            animator.enabled = true;
        }

        foreach (NavMeshAgent agent in allNavMeshAgents)
        {
            agent.isStopped = false;
        }

        foreach (BehaviorDesigner.Runtime.BehaviorTree bts in allBTs)
        {
            bts.enabled = true;
        }
    }

    void ChangeSkybox(Material skyboxMaterial)
    {
        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
            DynamicGI.UpdateEnvironment();
        }
    }
}
