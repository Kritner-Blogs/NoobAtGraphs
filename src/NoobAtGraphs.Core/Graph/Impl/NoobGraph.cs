using NoobAtGraphs.Core.Graph.Abstraction;
using System.Collections.Immutable;

namespace NoobAtGraphs.Core.Graph.Impl;

public class NoobGraph<TNodeKey, TNode> : IGraph<TNodeKey, TNode>
    where TNode : IGraphNode<TNodeKey, TNode>
    where TNodeKey : notnull
{
    private readonly IDictionary<TNodeKey, TNode> _keysToNodes = new Dictionary<TNodeKey, TNode>();

    public void AddNode(TNode node)
    {
        _keysToNodes.Add(node.NodeKey, node);
    }

    public IDictionary<TNodeKey, TNode> GetAllNodes()
    {
        return _keysToNodes.ToImmutableDictionary();
    }
}