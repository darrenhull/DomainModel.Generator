
public class TypeGraphBuilder
{
    private readonly Graph graph;
    private readonly INodeConnectionStrategy[] conn;

    public TypeGraphBuilder(INodeConnectionStrategy[] conn)
    {
        this.graph = new Graph();
        this.conn = conn;
    }

    public Node AddNode(Node node)
    {
        var newNode = graph.AddNode(node);
        ConnectNodes(newNode);
        return newNode;
    }

    public Graph Build() => this.graph;

    private void ConnectNodes(Node newNode)
    {
        foreach (var node in graph.Nodes)
        {
            if (newNode == node)
                continue;

            foreach(var connection in conn)
            {
                if(connection.AreConnected(node, newNode))
                    graph.AddEdge(connection.GetEdge(node, newNode));

                if (connection.AreConnected(newNode,node))
                    graph.AddEdge(connection.GetEdge(newNode, node));
            }

        }
    }
}
