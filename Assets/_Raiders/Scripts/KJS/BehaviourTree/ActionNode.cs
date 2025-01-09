using System;
using UnityEngine;

public class ActionNode : INode
{
    Func<NodeState> onUpdate = null;

    public ActionNode(Func<NodeState> onUpdate)
    {
        this.onUpdate = onUpdate;
    }

    public NodeState Evaluate()
    {
        throw new System.NotImplementedException();
    }
}
