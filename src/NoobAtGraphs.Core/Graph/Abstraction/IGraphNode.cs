namespace NoobAtGraphs.Core.Graph.Abstraction;

/// <summary>
/// Abstraction representing a graph node.
/// </summary>
/// <typeparam name="TNodeKey">The type used to represent the key of a node.</typeparam>
/// <typeparam name="TNode">The type of object being represented as nodes.</typeparam>
public interface IGraphNode<out TNodeKey, out TNode>
    where TNodeKey : notnull
{
    /// <summary>
    /// The graph node's key.
    /// </summary>
    TNodeKey NodeKey { get; }
    
    /// <summary>
    /// The graph node.
    /// </summary>
    TNode Node { get; }
}