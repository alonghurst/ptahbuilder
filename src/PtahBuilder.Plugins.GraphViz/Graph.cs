namespace PtahBuilder.Plugins.GraphViz;

public class Graph
{
    private readonly Dictionary<string, GraphAttribute> _attributes = new(StringComparer.Ordinal);
    private readonly Dictionary<string, Graph> _subgraphs = new(StringComparer.Ordinal);
    private readonly List<Node> _nodes = [];
    private readonly HashSet<string> _nodeIds = new(StringComparer.Ordinal);

    protected Graph(string name)
    {
        Require.NotNullOrWhiteSpace(name, nameof(name));
        Name = name;
        Root = (RootGraph)this;
    }

    internal Graph(RootGraph root, string name)
    {
        ArgumentNullException.ThrowIfNull(root);
        Require.NotNullOrWhiteSpace(name, nameof(name));
        Root = root;
        Name = name;
    }

    public RootGraph Root { get; }

    public string Name { get; }

    internal IReadOnlyDictionary<string, GraphAttribute> Attributes => _attributes;

    internal IReadOnlyList<Graph> Subgraphs => _subgraphs.Values.ToArray();

    internal IReadOnlyList<Node> Nodes => _nodes;

    public void SetAttribute(string name, string value)
    {
        Require.NotNullOrWhiteSpace(name, nameof(name));
        _attributes[name] = GraphAttribute.Plain(value);
    }

    public Graph GetOrAddSubgraph(string name)
    {
        Require.NotNullOrWhiteSpace(name, nameof(name));
        if (_subgraphs.TryGetValue(name, out var subgraph))
        {
            return subgraph;
        }

        subgraph = new Graph(Root, name);
        _subgraphs.Add(name, subgraph);
        return subgraph;
    }

    public Node GetOrAddNode(string id)
    {
        var node = Root.GetOrCreateNode(id);
        if (_nodeIds.Add(id))
        {
            _nodes.Add(node);
        }

        return node;
    }
}

public sealed class RootGraph : Graph
{
    private readonly Dictionary<string, Node> _nodesById = new(StringComparer.Ordinal);
    private readonly Dictionary<string, Edge> _edgesByName = new(StringComparer.Ordinal);
    private readonly List<Edge> _edges = [];

    public RootGraph(string name, bool directed = true)
        : base(name)
    {
        Directed = directed;
    }

    public bool Directed { get; }

    internal IReadOnlyList<Edge> Edges => _edges;

    internal IReadOnlyDictionary<string, Node> NodesById => _nodesById;

    internal Node GetOrCreateNode(string id)
    {
        Require.NotNullOrWhiteSpace(id, nameof(id));
        if (_nodesById.TryGetValue(id, out var node))
        {
            return node;
        }

        node = new Node(id);
        _nodesById.Add(id, node);
        return node;
    }

    public Edge GetOrAddEdge(Node from, Node to, string name)
    {
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);
        Require.NotNullOrWhiteSpace(name, nameof(name));

        if (_edgesByName.TryGetValue(name, out var edge))
        {
            return edge;
        }

        edge = new Edge(name, from, to);
        _edgesByName.Add(name, edge);
        _edges.Add(edge);
        return edge;
    }

    public string ToDot() => DotWriter.Write(this);

    public void WriteDotFile(string path)
    {
        Require.NotNullOrWhiteSpace(path, nameof(path));
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(path, ToDot());
    }
}

public sealed class Node
{
    private readonly Dictionary<string, GraphAttribute> _attributes = new(StringComparer.Ordinal);

    internal Node(string id)
    {
        Id = id;
    }

    public string Id { get; }

    internal IReadOnlyDictionary<string, GraphAttribute> Attributes => _attributes;

    public void SetAttribute(string name, string value)
    {
        Require.NotNullOrWhiteSpace(name, nameof(name));
        _attributes[name] = GraphAttribute.Plain(value);
    }

    public void SetHtmlAttribute(string name, string value)
    {
        Require.NotNullOrWhiteSpace(name, nameof(name));
        _attributes[name] = GraphAttribute.Html(value);
    }
}

public sealed class Edge
{
    private readonly Dictionary<string, GraphAttribute> _attributes = new(StringComparer.Ordinal);

    internal Edge(string name, Node from, Node to)
    {
        Name = name;
        From = from;
        To = to;
    }

    public string Name { get; }

    public Node From { get; }

    public Node To { get; }

    internal IReadOnlyDictionary<string, GraphAttribute> Attributes => _attributes;

    public void SetAttribute(string name, string value)
    {
        Require.NotNullOrWhiteSpace(name, nameof(name));
        _attributes[name] = GraphAttribute.Plain(value);
    }
}

internal readonly record struct GraphAttribute(string Value, bool IsHtml)
{
    public static GraphAttribute Plain(string value) => new(value ?? string.Empty, false);

    public static GraphAttribute Html(string value) => new(value ?? string.Empty, true);
}
