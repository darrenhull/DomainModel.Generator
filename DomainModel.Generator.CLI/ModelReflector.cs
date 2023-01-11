using System.Reflection;

namespace DomainModel.Generator.CLI;

public class ModelReflector
{
    private readonly Options options;

    public ModelReflector(Options options)
    {
        this.options = options;
    }
    public Graph ReflectTypes(Type[] types)
    {
        var graphBuilder = new TypeGraphBuilder(new INodeConnectionStrategy[] {
            new ByIdNodeConnectionStrategy(),
            new ByTypeNodeConnectionStrategy(),
            new ByInheritanceConnectionStrategy()
            });
        foreach (var type in types)
        {
            if (!options.ShouldReflect(type))
                continue;

            var node = new Node(type);
            try
            {
                if (type.IsEnum)
                {
                    var enumName = type.Name;
                    foreach (var fieldInfo in type.GetFields())
                    {
                        var name = fieldInfo.Name;

                        if (fieldInfo.FieldType.IsEnum)
                        {
                            node.AddPublicAttribute(name, typeof(void));
                        }
                    }
                }else
                {
                    var baseProperties = new List<PropertyInfo>();

                    if (type.BaseType != null && type.BaseType != typeof(object))
                        baseProperties.AddRange(type.BaseType.GetProperties(BindingFlags.Public |BindingFlags.Instance));


                    foreach (var property in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                    {
                        try
                        {
                            if(baseProperties.All(x=>!x.Name.Equals(property.Name)))
                                node.AddPublicAttribute(property.Name, property.PropertyType);
                        }
                        catch (FileNotFoundException ex)
                        {
                            Console.Error.WriteLine($"Error when reflecting {type.Name}.{property.Name}.");
                            PrintException(ex);
                            node.AddPublicAttribute(property.Name, typeof(object));
                        }
                    }
                }
                graphBuilder.AddNode(node);
            }
            catch (FileNotFoundException ex)
            {
                Console.Error.WriteLine($"Error when reflecting {type.Name}: " + ex.Message);
                PrintException(ex);
            }

        }
        return graphBuilder.Build();
    }

    private static void PrintException(FileNotFoundException ex)
    {
        Console.Error.Write("Please look on below exception and provide missing dependency into directory with your model. ");
        Console.Error.WriteLine("Try using 'dotnet publish' to be sure that external dependencies will be copied to output directory.");
        Console.Error.WriteLine(ex.Message);
    }
}
