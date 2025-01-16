using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PatternStart", story: "Start [Pattern]", category: "Action", id: "143562d365e3fbd455b7c952a21a3f14")]
public partial class PatternStartAction : Action
{
    [SerializeReference] public BlackboardVariable<Pattern> Pattern;
    //한번만 했는가를 확인하는 파라미터
    
    

    protected override Status OnStart()
    {
        Pattern.Value.StartPattern();
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Pattern.Value.UpdatePattern();
        
        
        
        if (Pattern.Value.remainPatternTime <= 0)
        {
            Debug.Log("Pattern falied");
            
            return Status.Failure;
        }

        if (Pattern.Value.CheckPatternClearCondition())
        {
            Pattern.Value.currenttime -= Time.deltaTime;
        }

        if (Pattern.Value.currenttime < 2)
        {
            var a = Pattern.Value.BarrierPrefab.GetComponent<Renderer>().material;
            float destorySphere = Mathf.Lerp(0,1,Pattern.Value.currenttime/2);
            a.SetFloat("_Dissolve",destorySphere);
            
            if (destorySphere <= 0)
            {
                Pattern.Value.EndPattern();
                return Status.Success;
            }
        }
    
        
        return Status.Running;
      
    }

    protected override void OnEnd()
    {
    }
}

