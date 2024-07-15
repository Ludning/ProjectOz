using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Check Is Enemy Just Loss Target")]
    [TaskCategory("Character")]
    [TaskIcon("{SkinColor}ReflectionIcon.png")]
    public class BTC_IsJustLossTarget : Conditional
    {
        public Enemy owner;
        public SharedTransform target;
        public SharedFloat detectRange;

        public override void OnAwake()
        {
            owner = GetComponent<Enemy>();
        }

        public override TaskStatus OnUpdate()
        {
            if (owner == null)
            {
                return TaskStatus.Failure;
            }
            if(target.Value == null)
            {
                return TaskStatus.Failure;
            }
            bool isLostTarget = !owner.IsTargetNear(detectRange.Value);
            if(target.Value.CompareTag("Player") && isLostTarget)
            {
                target.Value = null;
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}
