using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "StartPatternHp", story: "[BossStat] Compare Hp to PatternStartHp", category: "Conditions", id: "c095e7ef0b9952b3fb28ed9ab58f3690")]
public partial class StartPatternHpCondition : Condition
{
    [SerializeReference] public BlackboardVariable<BossStat> BossStat;
    public bool isOnce  = false;

    public override bool IsTrue()
    {
        if (BossStat.Value.BossHealth <= BossStat.Value.PatternHp && isOnce == false)
        {
            isOnce = true;
            return true;
        }
        return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
