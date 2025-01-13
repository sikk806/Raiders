using Unity.Behavior;
using UnityEngine;

public class BehaviourSC : MonoBehaviour
{
    public BehaviorGraph behaviour;
    public BlackboardVariable blackboard;
    void Start()
    {
        // behaviour = GetComponent<BehaviorGraph>();
        var a=  behaviour.BlackboardReference;
        var b = a.Blackboard.Variables;
        for (int i = 0; i < b.Count; i++)
        {
            Debug.Log(b[i].Name);
        }
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
