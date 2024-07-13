using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Check TargetIsNearOrEnable And Enable")]
    [TaskCategory("Character")]
    [TaskIcon("{SkinColor}ReflectionIcon.png")]
    public class BTC_IsTargetNear : Conditional
    {
        public Enemy owner;
        public float range = 3f;
        public SharedTransform currentTarget;

        public override TaskStatus OnUpdate()
        {
            if (owner == null)
            {
                Debug.LogWarning("Unable to compare field - compare value is null");
                return TaskStatus.Failure;
            }

            if (owner.IsTargetNear(range))
            {
                currentTarget.Value = owner.GetTarget();
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}
