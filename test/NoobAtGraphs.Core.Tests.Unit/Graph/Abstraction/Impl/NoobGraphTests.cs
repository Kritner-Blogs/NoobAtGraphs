using FluentAssertions;
using NoobAtGraphs.Core.Graph.Abstraction;
using NoobAtGraphs.Core.Graph.Exception;
using NoobAtGraphs.Core.Graph.Impl;
using Xunit;

namespace NoobAtGraphs.Core.Tests.Unit.Graph.Abstraction.Impl;

/// <summary>
/// Unit tests against <see cref="NoobGraph{TNodeKey,TNode}"/>.
/// </summary>
public class NoobGraphTests
{
    private readonly IGraph<Guid, FakeGraphNode> _sut = new NoobGraph<Guid, FakeGraphNode>();

    private record FakeGraphNode : IGraphNode<Guid, FakeGraphNode>
    {
        public Guid NodeKey { get; init; }
        public FakeGraphNode Node => this;
        public ISet<Guid> Successors { get; } = new HashSet<Guid>();


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

    [Fact]
    public void WhenAddingDirectedEdge_ShouldRepresentRelationshipInTail()
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
        
        
        _sut.AddDirectedEdge(node1.NodeKey, node2.NodeKey);


        var nodes = _sut.GetAllNodes();
        nodes[node1.NodeKey].Successors.Should().NotBeEmpty("node 1 should be a tail node");
        nodes[node1.NodeKey].Successors.Count.Should().Be(1, "a single directed edge was added with node 1 as the tail");
        nodes[node1.NodeKey].Successors.Single().Should().Be(node2.NodeKey, "a directed edge was added with node 1 as the tail, node 2 as the head");
        
        nodes[node2.NodeKey].Successors.Should().BeEmpty("node 2 not be a tail");
        nodes[node2.NodeKey].Successors.Count.Should().Be(0, "no directed edges were added with node 2 as the tail");
    }
    
    [Fact]
    public void WhenAddingDirectedEdgeTwice_ShouldThrow()
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
        
        
        _sut.AddDirectedEdge(node1.NodeKey, node2.NodeKey);
        var func = () => _sut.AddDirectedEdge(node1.NodeKey, node2.NodeKey);


        func.Should().ThrowExactly<EdgeAlreadyExistsException<Guid>>();
    }
    
    [Fact]
    public void WhenProvidedValidGraph_ShouldReturnInTraversalOrder()
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
        var node3 = new FakeGraphNode()
        {
            NodeKey = Guid.NewGuid(),
            SomeProperty = 1,
        };
        _sut.AddNode(node1);
        _sut.AddNode(node2);
        _sut.AddNode(node3);
        _sut.AddDirectedEdge(node1.NodeKey, node2.NodeKey);
        _sut.AddDirectedEdge(node2.NodeKey, node3.NodeKey);


        var traversalOrderKeys = _sut.GetNodeKeysInDependencyOrder().ToList();
        var nodes = _sut.GetAllNodes();
        
        traversalOrderKeys.Count.Should().Be(3, "number of nodes on the graph");
        nodes[traversalOrderKeys[0]].SomeProperty.Should().Be(42);
        nodes[traversalOrderKeys[1]].SomeProperty.Should().Be(7);
        nodes[traversalOrderKeys[2]].SomeProperty.Should().Be(1);
    }
    
    [Fact]
    public void WhenProvidedInvalidGraph_ShouldThrow()
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
        var node3 = new FakeGraphNode()
        {
            NodeKey = Guid.NewGuid(),
            SomeProperty = 1,
        };
        _sut.AddNode(node1);
        _sut.AddNode(node2);
        _sut.AddNode(node3);
        _sut.AddDirectedEdge(node1.NodeKey, node2.NodeKey);
        

        var func = () => _sut.GetNodeKeysInDependencyOrder().ToList();


        func.Should().ThrowExactly<InvalidGraphException>("not all nodes in the graph are connected");
    }
    
    [Fact]
    public void WhenProvidingGraphWithCycle_ShouldThrow()
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
        _sut.AddDirectedEdge(node1.NodeKey, node2.NodeKey);
        _sut.AddDirectedEdge(node2.NodeKey, node1.NodeKey);


        true.Should().BeFalse("Not yet implemented cycle detection logic");
    }
}