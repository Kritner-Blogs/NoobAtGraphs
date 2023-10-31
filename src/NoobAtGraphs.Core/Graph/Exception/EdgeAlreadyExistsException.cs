namespace NoobAtGraphs.Core.Graph.Exception;

public class EdgeAlreadyExistsException<TNodeKey> : System.Exception
{
    public EdgeAlreadyExistsException(TNodeKey tail, TNodeKey head) : base(
        $"The specified directed edge between {tail} and {head} already exists")
    {
    }
}