using PtahBuilder.Plugins.GraphViz;

namespace PtahBuilder.Tests.GraphViz;

public sealed class DotWriterTests
{
    [Fact]
    public void ToDot_WritesEmptyDirectedGraph()
    {
        var graph = new RootGraph("Empty");

        AssertDot(
            """
            digraph Empty {
            }

            """,
            graph.ToDot());
    }

    [Fact]
    public void ToDot_WritesUndirectedGraph()
    {
        var graph = new RootGraph("Friends", directed: false);
        var a = graph.GetOrAddNode("a");
        var b = graph.GetOrAddNode("b");
        graph.GetOrAddEdge(a, b, "knows");

        AssertDot(
            """
            graph Friends {
                a;
                b;
                a -- b;
            }

            """,
            graph.ToDot());
    }

    [Fact]
    public void ToDot_WritesAttributesNodesAndEdges()
    {
        var graph = new RootGraph("ItemsToMinions");
        graph.SetAttribute("rankdir", "LR");

        var item = graph.GetOrAddNode("item_Oil");
        item.SetAttribute("label", "Oil");
        item.SetAttribute("shape", "ellipse");

        var minion = graph.GetOrAddNode("minion_Skeleton");
        minion.SetAttribute("label", "Skeleton [0]");
        minion.SetAttribute("shape", "box");

        var edge = graph.GetOrAddEdge(item, minion, "item_0");
        edge.SetAttribute("label", "x3 via Exosmithy");

        AssertDot(
            """
            digraph ItemsToMinions {
                rankdir="LR";
                item_Oil [label="Oil", shape="ellipse"];
                minion_Skeleton [label="Skeleton [0]", shape="box"];
                item_Oil -> minion_Skeleton [label="x3 via Exosmithy"];
            }

            """,
            graph.ToDot());
    }

    [Fact]
    public void ToDot_WritesHtmlLabelsAndEscapesQuotedValues()
    {
        var graph = new RootGraph("Labels");
        var html = graph.GetOrAddNode("html");
        html.SetHtmlAttribute("label", "<FONT>Lich [4]</FONT><BR/><I>Calamity Crucible</I>");

        var quoted = graph.GetOrAddNode("quoted");
        quoted.SetAttribute("label", "Say \"hi\"");

        var path = graph.GetOrAddNode("path");
        path.SetAttribute("label", "C:\\temp");

        var wrapped = graph.GetOrAddNode("wrapped");
        wrapped.SetAttribute("label", "line1\nline2");

        AssertDot(
            """
            digraph Labels {
                html [label=<<FONT>Lich [4]</FONT><BR/><I>Calamity Crucible</I>>];
                quoted [label="Say \"hi\""];
                path [label="C:\\temp"];
                wrapped [label="line1\nline2"];
            }

            """,
            graph.ToDot());
    }

    [Fact]
    public void ToDot_QuotesKeywordsAndNonPlainIds()
    {
        var graph = new RootGraph("Keywords");
        graph.GetOrAddNode("graph");
        graph.GetOrAddNode("has-dash");

        AssertDot(
            """
            digraph Keywords {
                "graph";
                "has-dash";
            }

            """,
            graph.ToDot());
    }

    [Fact]
    public void ToDot_WritesSameRankSubgraphAndReusesNodes()
    {
        var graph = new RootGraph("MinionRelations");
        var rank = graph.GetOrAddSubgraph("rank_fodder");
        rank.SetAttribute("rank", "same");

        var fodder = rank.GetOrAddNode("minion_Zombie");
        fodder.SetAttribute("label", "Zombie");

        var apparatus = graph.GetOrAddNode("apparatus_FleshBench");
        apparatus.SetAttribute("label", "Flesh Bench");

        var sameFodder = graph.GetOrAddNode("minion_Zombie");
        graph.GetOrAddEdge(sameFodder, apparatus, "minion_0");

        Assert.Same(fodder, sameFodder);
        AssertDot(
            """
            digraph MinionRelations {
                subgraph rank_fodder {
                    rank="same";
                    minion_Zombie [label="Zombie"];
                }
                apparatus_FleshBench [label="Flesh Bench"];
                minion_Zombie -> apparatus_FleshBench;
            }

            """,
            graph.ToDot());
    }

    [Fact]
    public void GetOrAdd_IsIdempotent()
    {
        var graph = new RootGraph("Reuse");
        var subgraph = graph.GetOrAddSubgraph("rank_fodder");

        Assert.Same(subgraph, graph.GetOrAddSubgraph("rank_fodder"));
        Assert.Same(graph.GetOrAddNode("a"), graph.GetOrAddNode("a"));
        Assert.Same(graph.GetOrAddEdge(graph.GetOrAddNode("a"), graph.GetOrAddNode("b"), "e"),
            graph.GetOrAddEdge(graph.GetOrAddNode("a"), graph.GetOrAddNode("b"), "e"));
    }

    [Fact]
    public void WriteDotFile_CreatesDirectoryAndWritesDot()
    {
        var directory = Path.Combine(Path.GetTempPath(), "PtahBuilder-DotWriterTests", Guid.NewGuid().ToString("N"));
        var path = Path.Combine(directory, "nested", "graph.dot");
        var graph = new RootGraph("Written");
        graph.GetOrAddNode("n");

        try
        {
            graph.WriteDotFile(path);

            Assert.True(File.Exists(path));
            AssertDot(graph.ToDot(), File.ReadAllText(path));
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }
    }

    private static void AssertDot(string expected, string actual) =>
        Assert.Equal(expected.Replace("\r\n", "\n"), actual.Replace("\r\n", "\n"));
}
