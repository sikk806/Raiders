using UnityEngine;

public class BehaviourTreeRunner
{
    INode rootNode;

    public BehaviourTreeRunner(INode rootNode)
    {
        this.rootNode = rootNode;
    }

    public void Operate()
    {
        rootNode.Evaluate();
    }
}
