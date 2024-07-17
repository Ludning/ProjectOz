namespace BehaviorDesigner.Runtime.Tasks
{
    public class BTA_SetState : Action
    {
        public Enemy enemy;
        public AIState state;

        public override void OnAwake()
        {
            base.OnAwake();
            enemy = GetComponent<Enemy>();
        }

        public override TaskStatus OnUpdate()
        {
            enemy.SetState(state);
            return TaskStatus.Success;
        }
    }

}