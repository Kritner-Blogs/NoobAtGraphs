using FluentAssertions;
using NoobAtGraphs.Core.Graph.Abstraction;
using NoobAtGraphs.Core.Graph.Impl;
using Xunit;

namespace NoobAtGraphs.Tests.Unit.Graph.Abstraction.Impl;

/// <summary>
/// Unit tests against <see cref="NoobGraph{TNodeKey,TNode}"/>.
/// </summary>
public class NoobGraphTests
{
    private readonly NoobGraph<Guid, FakeGraphNode> _sut = new();

    private record FakeGraphNode : IGraphNode<Guid, FakeGraphNode>
    {
        public Guid NodeKey { get; init; }
        public FakeGraphNode Node => this;

        public int SomeProperty { get; init; }
    }

    [Fact]
    public void WhenInvokingAddNode_ShouldAddNodeToGraph()
    {
        var node1 = new FakeGraphNode()
        {
            NodeKey = Guid.NewGuid(),
            SomeProperty = 42,
        };
        var node2 = new FakeGraphNode()
        {
            NodeKey = Guid.NewGuid(),
            SomeProperty = 7,
        };

        
        _sut.AddNode(node1);
        _sut.AddNode(node2);

        
        var nodes = _sut.GetAllNodes();
        nodes.Count.Should().Be(2, "the number of nodes that were added");
        nodes.Keys.Should().Contain(node1.NodeKey, "this node was added to the graph");
        nodes.Keys.Should().Contain(node2.NodeKey, "this node was added to the graph");
        nodes[node1.NodeKey].SomeProperty.Should().Be(node1.SomeProperty, "when looking up by node ID, should get the expected node");
        nodes[node2.NodeKey].SomeProperty.Should().Be(node2.SomeProperty, "when looking up by node ID, should get the expected node");
    }

    [Fact]
    public void WhenAttemptingToAddDuplicateKey_ShouldThrow()
    {
        var guid = Guid.NewGuid();
        var node1 = new FakeGraphNode()
        {
            NodeKey = guid,
            SomeProperty = 42,
        };
        var node2 = new FakeGraphNode()
        {
            NodeKey = guid,
            SomeProperty = 7,
        };

        
        _sut.AddNode(node1);
        var action = () => _sut.AddNode(node2);


        action.Should().ThrowExactly<ArgumentException>("the key was already added");
    }
}