namespace BehaviorDesigner.Runtime.Tasks
{
    public class BTA_Attack : Action
    {
        public Enemy owner;

        public override void OnAwake()
        {
            owner = GetComponent<Enemy>();
        }

        public override TaskStatus OnUpdate()
        {
            if (owner.Attack())
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }


    }
}