using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Check TargetIsNearOrEnable And Enable")]
    [TaskCategory("Character")]
    [TaskIcon("{SkinColor}ReflectionIcon.png")]
    public class BTC_IsTargetNear : Conditional
    {
        public Enemy owner;
        public SharedFloat range = 3f;

        public override void OnAwake()
        {
            owner = GetComponent<Enemy>();
        }
        public override TaskStatus OnUpdate()
        {
            if (owner == null)
            {
                Debug.LogWarning("Unable to compare field - compare value is null");
                return TaskStatus.Failure;
            }

            if (owner.IsTargetNear(range.Value))
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}
