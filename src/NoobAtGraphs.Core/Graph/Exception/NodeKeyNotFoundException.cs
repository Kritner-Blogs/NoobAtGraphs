namespace NoobAtGraphs.Core.Graph.Exception;

public class NodeKeyNotFoundException<TNodeKey> : System.Exception
{
    public NodeKeyNotFoundException(TNodeKey nodeKey) : base(
        $"The specified node key of {nodeKey} was not found within the graph.")
    {
    }
}