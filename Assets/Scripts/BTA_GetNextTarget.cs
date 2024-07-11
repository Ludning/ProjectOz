using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class BTA_GetNextTarget : Action
    {
        public SharedTransformList targetList;
        public SharedInt currentTargetIndex;
        public SharedTransform currentTarget;

        public override void OnStart()
        {

            if (currentTargetIndex.Value + 1 >= targetList.Value.Count)
            {
                currentTargetIndex.Value = 0;
            }
            else
            {
                currentTargetIndex.Value = currentTargetIndex.Value + 1;
            }

            currentTarget.Value = targetList.Value[currentTargetIndex.Value];
        }
    }

}