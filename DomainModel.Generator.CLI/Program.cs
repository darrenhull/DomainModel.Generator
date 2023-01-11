using DomainModel.Generator.CLI;
using DomainModel.Generator.Mermaid;

static string GenerateDiagram(Graph graph)
{
    var diagramGenerator = new DomainModel.Generator.Mermaid.ClassDiagramGenerator();
    var map = new Dictionary<Node, IClass>();
    foreach (var node in graph.Nodes)
    {
        var @class = diagramGenerator.AddClass(node.Name);
        map.Add(node, @class);
        foreach (var attribute in node.Attributes)
        {
            var attributeType = attribute.type == "void" ? "" : attribute.type;
            @class.AddPublicAttribute(attribute.name, attributeType);
        }
    }
    foreach (var edge in graph.Edges)
    {
        if(edge is ChildEdge)
            diagramGenerator.LinkWithInheritance(map[edge.From], map[edge.To]);
        else
            diagramGenerator.LinkWithAssociation(map[edge.From], map[edge.To]);
    }
    return diagramGenerator.Generate();
}

return ArgsParser.Run(args, (options) =>
{
    //new OptionsValidator().AssertOptions(options);

    var modelLoader = new ModelLoader(
        new ModelReflector(options));
    var graph = modelLoader.LoadModule(options);
    var diagram = GenerateDiagram(graph);
    File.WriteAllText(options.GenerateOptions.OutputPath, diagram);
    var exists = File.Exists(options.GenerateOptions.OutputPath);
    return 0;

},
onError: result => { Console.Error.WriteLine(result); return 1; },
showHelp: result => { Console.WriteLine(result); return 0; },
showVersion: result => { Console.WriteLine(result); return 0; });
