using System.Text;

namespace PtahBuilder.Plugins.GraphViz;

internal static class DotWriter
{
    private static readonly HashSet<string> Keywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "node",
        "edge",
        "graph",
        "digraph",
        "subgraph",
        "strict",
    };

    public static string Write(RootGraph graph)
    {
        ArgumentNullException.ThrowIfNull(graph);

        var builder = new StringBuilder();
        var keyword = graph.Directed ? "digraph" : "graph";
        builder.Append(keyword).Append(' ').Append(EscapeId(graph.Name)).Append(" {\n");

        WriteAttributes(builder, graph.Attributes, indent: 1, inline: false);

        var declaredNodes = new HashSet<string>(StringComparer.Ordinal);
        WriteSubgraphs(builder, graph.Subgraphs, declaredNodes, indent: 1);

        foreach (var node in RemainingNodes(graph, declaredNodes))
        {
            WriteNode(builder, node, indent: 1);
            declaredNodes.Add(node.Id);
        }

        var edgeOperator = graph.Directed ? "->" : "--";
        foreach (var edge in graph.Edges)
        {
            builder.Append("    ")
                .Append(EscapeId(edge.From.Id))
                .Append(' ')
                .Append(edgeOperator)
                .Append(' ')
                .Append(EscapeId(edge.To.Id));
            WriteInlineAttributes(builder, edge.Attributes);
            builder.Append(";\n");
        }

        builder.Append("}\n");
        return builder.ToString();
    }

    private static IEnumerable<Node> RemainingNodes(RootGraph graph, HashSet<string> declaredNodes)
    {
        foreach (var node in graph.Nodes)
        {
            if (declaredNodes.Add(node.Id))
            {
                yield return node;
            }
        }

        foreach (var node in graph.NodesById.Values)
        {
            if (declaredNodes.Add(node.Id))
            {
                yield return node;
            }
        }
    }

    private static void WriteSubgraphs(
        StringBuilder builder,
        IReadOnlyList<Graph> subgraphs,
        HashSet<string> declaredNodes,
        int indent)
    {
        foreach (var subgraph in subgraphs)
        {
            builder.Append(Indent(indent))
                .Append("subgraph ")
                .Append(EscapeId(subgraph.Name))
                .Append(" {\n");

            WriteAttributes(builder, subgraph.Attributes, indent + 1, inline: false);
            WriteSubgraphs(builder, subgraph.Subgraphs, declaredNodes, indent + 1);

            foreach (var node in subgraph.Nodes)
            {
                if (declaredNodes.Add(node.Id))
                {
                    WriteNode(builder, node, indent + 1);
                }
                else
                {
                    builder.Append(Indent(indent + 1)).Append(EscapeId(node.Id)).Append(";\n");
                }
            }

            builder.Append(Indent(indent)).Append("}\n");
        }
    }

    private static void WriteNode(StringBuilder builder, Node node, int indent)
    {
        builder.Append(Indent(indent)).Append(EscapeId(node.Id));
        WriteInlineAttributes(builder, node.Attributes);
        builder.Append(";\n");
    }

    private static void WriteAttributes(
        StringBuilder builder,
        IReadOnlyDictionary<string, GraphAttribute> attributes,
        int indent,
        bool inline)
    {
        if (inline)
        {
            WriteInlineAttributes(builder, attributes);
            return;
        }

        foreach (var (name, attribute) in attributes)
        {
            builder.Append(Indent(indent))
                .Append(EscapeId(name))
                .Append('=')
                .Append(FormatValue(attribute))
                .Append(";\n");
        }
    }

    private static void WriteInlineAttributes(
        StringBuilder builder,
        IReadOnlyDictionary<string, GraphAttribute> attributes)
    {
        if (attributes.Count == 0)
        {
            return;
        }

        builder.Append(" [");
        var first = true;
        foreach (var (name, attribute) in attributes)
        {
            if (!first)
            {
                builder.Append(", ");
            }

            builder.Append(EscapeId(name)).Append('=').Append(FormatValue(attribute));
            first = false;
        }

        builder.Append(']');
    }

    internal static string EscapeId(string id)
    {
        if (IsPlainId(id) && !Keywords.Contains(id))
        {
            return id;
        }

        return Quote(id);
    }

    internal static string FormatValue(GraphAttribute attribute) =>
        attribute.IsHtml ? $"<{attribute.Value}>" : Quote(attribute.Value);

    internal static string Quote(string value)
    {
        var builder = new StringBuilder(value.Length + 2);
        builder.Append('"');
        foreach (var character in value)
        {
            switch (character)
            {
                case '\\':
                    builder.Append("\\\\");
                    break;
                case '"':
                    builder.Append("\\\"");
                    break;
                case '\n':
                    builder.Append("\\n");
                    break;
                case '\r':
                    builder.Append("\\r");
                    break;
                default:
                    builder.Append(character);
                    break;
            }
        }

        builder.Append('"');
        return builder.ToString();
    }

    private static bool IsPlainId(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }

        if (!IsIdStart(id[0]))
        {
            return false;
        }

        for (var i = 1; i < id.Length; i++)
        {
            if (!IsIdPart(id[i]))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsIdStart(char character) =>
        character is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or '_';

    private static bool IsIdPart(char character) =>
        IsIdStart(character) || character is >= '0' and <= '9';

    private static string Indent(int indent) => new(' ', indent * 4);
}
