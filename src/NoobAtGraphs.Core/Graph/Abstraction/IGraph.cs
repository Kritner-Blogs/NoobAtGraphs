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

    /// <summary>
    /// Adds a directed edge to the graph.
    /// </summary>
    /// <param name="tail">The predecessor (dependency).</param>
    /// /// <param name="head">The successor.</param>
    void AddDirectedEdge(TNodeKey tail, TNodeKey head);
    
    /// <summary>
    /// Get all nodes from the graph, organized into a dictionary where the <see cref="TNodeKey"/> is the key,
    /// and <see cref="TNode"/>s aer the values.
    /// </summary>
    /// <remarks>Does not take into account directed edges/dependencies.</remarks>
    /// <returns>A dictionary of <see cref="TNodeKey"/>s to <see cref="TNode"/>s.</returns>
    IDictionary<TNodeKey, TNode> GetAllNodes();

    /// <summary>
    /// Gets nodes from the graph in the order in which they should be operated on.
    /// </summary>
    /// <returns>The node keys in their traversal order.</returns>
    IEnumerable<TNodeKey> GetNodeKeysInDependencyOrder();
}