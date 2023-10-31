namespace NoobAtGraphs.Core.Graph.Impl;

public class InvalidGraphException : System.Exception
{
    public InvalidGraphException() : base(
        $"Unable to traverse graph")
    {
    }
}