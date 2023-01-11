public interface INodeConnectionStrategy
{
    bool AreConnected(Node from, Node to);

    Edge GetEdge(Node from, Node to);
}

public abstract class NodeConnectionStrategy: INodeConnectionStrategy
{
    public abstract bool AreConnected(Node from, Node to);

    public virtual Edge GetEdge(Node from, Node to)
    {
        return new Edge(from, to);
    }
}

/**
<summary>Match by direct reference of type</summary>
<example>
class A {}
class B {
    public A RefToA {get; set;}
}
</example>
*/
public class ByInheritanceConnectionStrategy : NodeConnectionStrategy
{
    public override bool AreConnected(Node from, Node to)
    {
        //TODO: Solve potential problem with formatted attribute name vs type name
        var isConnected = (Node a, Node b) => a.ParentType?.Equals(b.Name)??false;
        return isConnected(from, to);
            
    }
    public override Edge GetEdge(Node from, Node to)
    {
        return new ChildEdge(from, to);
    }
}

public class ByEnumerableConnectionStrategy : NodeConnectionStrategy
{
    public override bool AreConnected(Node from, Node to)
    {
        //TODO: Solve potential problem with formatted attribute name vs type name
        var isConnected = (Node a, Node b) => a.ParentType?.Equals(b.Name) ?? false;
        return isConnected(from, to);

    }
    public override Edge GetEdge(Node from, Node to)
    {
        return new ChildEdge(from, to);
    }
}


//                else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
//{
//    var fieldType = type.GetType().GetGenericArguments()[0];
//    node.AddPublicAttribute(name, fieldType);
//}

/**
<summary>Match by direct reference of type</summary>
<example>
class A {}
class B {
    public A RefToA {get; set;}
}
</example>
*/
public class ByTypeNodeConnectionStrategy : NodeConnectionStrategy
{
    public override bool AreConnected(Node from, Node to)
    {
        //TODO: Solve potential problem with formatted attribute name vs type name
        var isConnected = (Node a, Node b) => a.Attributes.Any(n => n.type == b.Name);
        return isConnected(from, to);
    }
}

/**
<summary>Match by indirect reference by id</summary>
<example>
class A {}
class B {
    public Guid AId {get; set;}
    OR
    public Guid[] AIds {get; set;}
}
</example>
*/
public class ByIdNodeConnectionStrategy : NodeConnectionStrategy
{
    public override bool AreConnected(Node from, Node to)
    {
        //TODO: Solve potential problem with formatted attribute name vs type name
        var isConnected = (Node a, Node b) => a.Attributes.Any(n => (n.name == $"{b.Name}Id") || (n.name == $"{b.Name}Ids"));
        return isConnected(from, to);
    }


}