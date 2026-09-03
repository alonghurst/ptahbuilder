using PtahBuilder.Plugins.Markdown;
using Xunit;

namespace PtahBuilder.Tests.BuildSystem;

public class InlineMarkdownTests
{
    [Theory]
    [InlineData("A plain blade.")]
    [InlineData("Minions deal **25% more damage**.")]
    [InlineData("Heals *you* and nearby allies.")]
    [InlineData("A ~~broken~~ relic.")]
    [InlineData("Costs `10` mana.")]
    [InlineData("See [Raise Dead](ability:RaiseDead).")]
    [InlineData("Use \\*literally\\*.")]
    public void Validate_AcceptsInlineMarkdown(string markdown)
    {
        Assert.Empty(InlineMarkdown.Validate(markdown));
    }

    [Fact]
    public void Validate_Empty_ReturnsNoErrors()
    {
        Assert.Empty(InlineMarkdown.Validate(null));
        Assert.Empty(InlineMarkdown.Validate(""));
        Assert.Empty(InlineMarkdown.Validate("   "));
    }

    [Fact]
    public void Validate_UnclosedBold_ReturnsError()
    {
        var errors = InlineMarkdown.Validate("Minions deal **25% more damage.");
        Assert.Contains(errors, error => error.Contains("Unclosed", StringComparison.Ordinal));
    }

    [Fact]
    public void Validate_UnclosedLink_ReturnsError()
    {
        var errors = InlineMarkdown.Validate("See [Raise Dead](ability:RaiseDead");
        Assert.Contains(errors, error => error.Contains("link", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validate_Heading_ReturnsError()
    {
        var errors = InlineMarkdown.Validate("# Title\nA blade.");
        Assert.Contains(errors, error => error.Contains("heading", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validate_List_ReturnsError()
    {
        var errors = InlineMarkdown.Validate("- Raises corpses\n- Costs mana.");
        Assert.Contains(errors, error => error.Contains("list", StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData("A plain blade", "A plain blade.")]
    [InlineData("A plain blade.", "A plain blade.")]
    [InlineData("Minions deal **25% more damage**", "Minions deal **25% more damage**.")]
    [InlineData("See [docs](url)", "See [docs](url).")]
    [InlineData("See [docs](url).", "See [docs](url).")]
    [InlineData("Already done!", "Already done!")]
    public void EnsureTerminalPunctuation_UsesPlainText(string input, string expected)
    {
        Assert.Equal(expected, InlineMarkdown.EnsureTerminalPunctuation(input));
    }

    [Fact]
    public void ToPlainText_StripsEmphasisAndLinks()
    {
        Assert.Equal(
            "Minions deal 25% more damage.",
            InlineMarkdown.ToPlainText("Minions deal **25% more damage**."));
        Assert.Equal("See docs.", InlineMarkdown.ToPlainText("See [docs](url)."));
    }
}
