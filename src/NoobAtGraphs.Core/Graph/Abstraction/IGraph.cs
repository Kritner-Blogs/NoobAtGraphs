namespace NoobAtGraphs.Core.Graph.Abstraction;

/// <summary>
/// Abstraction representing a graph and its operations.
/// </summary>
/// <typeparam name="TNodeKey">The type of key being represented in each node of the graph.</typeparam>
/// <typeparam name="TNode">The type of node being represented by the graph.</typeparam>
public interface IGraph<TNodeKey, TNode>
    where TNode : IGraphNode<TNodeKey, TNode>
    where TNodeKey : notnull
{
    /// <summary>
    /// Adds a node to the graph.
    /// </summary>
    /// <param name="node">The node to add to the graph.</param>
    void AddNode(TNode node);

    IDictionary<TNodeKey, TNode> GetAllNodes();
}