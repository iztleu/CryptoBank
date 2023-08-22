namespace GenerivHosting.Kestrel.Endpoints.Controllers.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class PathAttribute : Attribute
{
    public PathAttribute(string path)
    {
        Path = path;
    }

    public string Path { get; }
}