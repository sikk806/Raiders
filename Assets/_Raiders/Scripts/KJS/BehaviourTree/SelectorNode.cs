using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// If Success return Success. But If Failure, check other Nodes.
public class SelectorNode : INode
{
    List<INode> childs;

    public SelectorNode(List<INode> childs)
    {
        this.childs = childs;
    }

    public NodeState Evaluate()
    {
        if(childs == null)
        {
            return NodeState.Failure;
        }

        foreach(INode child in childs)
        {
            switch(child.Evaluate())
            {
                case NodeState.Success:
                    return NodeState.Success;
                case NodeState.Running:
                    return NodeState.Running;
            }
        }
        
        return NodeState.Failure;
    }
}
