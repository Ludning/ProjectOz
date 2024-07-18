using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Check Is Enemy can see target")]
    [TaskCategory("Character")]
    [TaskIcon("{SkinColor}ReflectionIcon.png")]
    public class BTC_IsVisible : Conditional
    {
        public Enemy owner;

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
            if(owner.IsTargetVisible())
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}
