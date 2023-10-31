using NoobAtGraphs.Core.Graph.Abstraction;
using NoobAtGraphs.Core.Graph.Exception;
using System.Collections.Immutable;

namespace NoobAtGraphs.Core.Graph.Impl;

public class NoobGraph<TNodeKey, TNode> : IGraph<TNodeKey, TNode>
    where TNode : IGraphNode<TNodeKey, TNode>
    where TNodeKey : notnull
{
    /// <summary>
    /// Dictionary mapping node keys to their respective node.
    /// </summary>
    private readonly IDictionary<TNodeKey, TNode> _keysToNodes = new Dictionary<TNodeKey, TNode>();
    
    /// <summary>
    /// Dictionary mapping individual node keys to 1..n heads.
    /// </summary>
    private readonly IDictionary<TNodeKey, ISet<TNodeKey>> _tailsToHeads = new Dictionary<TNodeKey, ISet<TNodeKey>>();

    private enum NodeStatus
    {
        Unvisited,
        Visited,
        TerminationPoint
    }
    
    public void AddNode(TNode node)
    {
        _keysToNodes.Add(node.NodeKey, node);
    }

    public void AddDirectedEdge(TNodeKey tail, TNodeKey head)
    {
        if (!_keysToNodes.ContainsKey(tail))
            throw new NodeKeyNotFoundException<TNodeKey>(tail);
        if (!_keysToNodes.ContainsKey(head))
            throw new NodeKeyNotFoundException<TNodeKey>(head);
        var existsInMap = _tailsToHeads.TryGetValue(tail, out var heads);
        if (heads != null && heads.Contains(head))
            throw new EdgeAlreadyExistsException<TNodeKey>(tail, head);

        if (existsInMap)
        {
            heads.Add(head);
        }
        else
        {
            _tailsToHeads.Add(tail, new HashSet<TNodeKey>()
            {
                head
            });
        }

        _keysToNodes[tail].Successors.Add(head);
    }

    public IDictionary<TNodeKey, TNode> GetAllNodes()
    {
        return _keysToNodes.ToImmutableDictionary();
    }

    public IEnumerable<TNodeKey> GetNodeKeysInDependencyOrder()
    {
        // Randomly ordered keys, and their "visited" status
        var randomlyOrderedKeys = _keysToNodes.Keys
            .OrderBy(_ => Guid.NewGuid())
            .ToDictionary(key => key, _ => NodeStatus.Unvisited);

        foreach (var startingNode in randomlyOrderedKeys)
        {
            var visitedKeys = new HashSet<TNodeKey>();

            var reverseOrderedNodeKeys = new List<TNodeKey>();
            if (DepthFirstSearch(startingNode.Key, visitedKeys, randomlyOrderedKeys, reverseOrderedNodeKeys))
            {
                // need to reverse the order of the list, since it is currently in the opposite order of traversal
                reverseOrderedNodeKeys.Reverse();
                return reverseOrderedNodeKeys;
            }
                

            SetAllNodesToUnvisited(randomlyOrderedKeys);
        }

        throw new InvalidGraphException();
    }

    private bool DepthFirstSearch(TNodeKey startingNode,
        ISet<TNodeKey> visitedKeys,
        IDictionary<TNodeKey, NodeStatus> randomlyOrderedKeys, 
        List<TNodeKey> orderedNodeKeys)
    {
        visitedKeys.Add(startingNode);
        randomlyOrderedKeys[startingNode] = NodeStatus.Visited;

        if (_tailsToHeads.TryGetValue(startingNode, out var toHead))
        {
            foreach (var head in toHead)
            {
                DepthFirstSearch(head, visitedKeys, randomlyOrderedKeys, orderedNodeKeys);
            }
        }
        
        randomlyOrderedKeys[startingNode] = NodeStatus.TerminationPoint;
        orderedNodeKeys.Add(startingNode);
        return visitedKeys.Count == randomlyOrderedKeys.Count;
    }
    
    private static void SetAllNodesToUnvisited(Dictionary<TNodeKey, NodeStatus> randomlyOrderedKeys)
    {
        foreach (var kvp in randomlyOrderedKeys)
            randomlyOrderedKeys[kvp.Key] = NodeStatus.Unvisited;
    }
}