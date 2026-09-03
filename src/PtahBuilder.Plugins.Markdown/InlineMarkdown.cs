using System.Text;
using System.Text.RegularExpressions;

namespace PtahBuilder.Plugins.Markdown;

/// <summary>
/// Validates and normalizes the inline markdown subset used by Slackjaw
/// <c>MarkdownRenderer.ToRichText</c>: emphasis, strike, code spans, and links.
/// Block constructs (headings, lists, quotes, fences, rules) are rejected.
/// </summary>
public static class InlineMarkdown
{
    private static readonly Regex AtxHeadingRegex = new(@"^(#{1,6})\s+", RegexOptions.Compiled);
    private static readonly Regex UnorderedListRegex = new(@"^[-*+]\s+", RegexOptions.Compiled);
    private static readonly Regex OrderedListRegex = new(@"^\d+[.)]\s+", RegexOptions.Compiled);
    private static readonly Regex HorizontalRuleRegex = new(@"^(\s*[-*_]){3,}\s*$", RegexOptions.Compiled);

    private static readonly string[] TerminalPunctuation = { ".", "!", "?" };

    public static IReadOnlyList<string> Validate(string? markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return Array.Empty<string>();

        var errors = new List<string>();
        ValidateBlocks(markdown, errors);
        ValidateInline(markdown, errors);
        return errors;
    }

    public static string ToPlainText(string? markdown)
    {
        if (string.IsNullOrEmpty(markdown))
            return string.Empty;

        var sb = new StringBuilder(markdown.Length);
        WritePlain(markdown, 0, markdown.Length, sb);
        return sb.ToString();
    }

    public static string EnsureTerminalPunctuation(string? markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return markdown ?? string.Empty;

        var trimmed = markdown.Trim();
        var plain = ToPlainText(trimmed).TrimEnd();
        if (string.IsNullOrEmpty(plain))
            return trimmed;

        foreach (var ending in TerminalPunctuation)
        {
            if (plain.EndsWith(ending, StringComparison.Ordinal))
                return trimmed;
        }

        return trimmed + ".";
    }

    private static void ValidateBlocks(string markdown, List<string> errors)
    {
        var lines = markdown.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        foreach (var line in lines)
        {
            var trimmed = line.TrimStart();
            if (string.IsNullOrWhiteSpace(trimmed))
                continue;

            if (trimmed.StartsWith("```", StringComparison.Ordinal))
            {
                errors.Add("Contains a fenced code block. Descriptions only support inline markdown.");
                return;
            }

            if (AtxHeadingRegex.IsMatch(trimmed))
            {
                errors.Add("Contains a markdown heading. Descriptions only support inline markdown.");
                return;
            }

            if (IsHorizontalRule(trimmed))
            {
                errors.Add("Contains a horizontal rule. Descriptions only support inline markdown.");
                return;
            }

            if (trimmed.StartsWith(">", StringComparison.Ordinal))
            {
                errors.Add("Contains a blockquote. Descriptions only support inline markdown.");
                return;
            }

            if (UnorderedListRegex.IsMatch(trimmed) || OrderedListRegex.IsMatch(trimmed))
            {
                errors.Add("Contains a markdown list. Descriptions only support inline markdown.");
                return;
            }
        }
    }

    private static bool IsHorizontalRule(string trimmed) =>
        trimmed.Length >= 3 && HorizontalRuleRegex.IsMatch(trimmed);

    private static void ValidateInline(string text, List<string> errors)
    {
        var i = 0;
        while (i < text.Length)
        {
            if (text[i] == '\\' && i + 1 < text.Length)
            {
                i += 2;
                continue;
            }

            if (text[i] == '`')
            {
                if (!TryTakeDelimited(text, i, "`", out _, out var codeEnd))
                {
                    errors.Add("Unclosed code span (`).");
                    i++;
                    continue;
                }

                i = codeEnd;
                continue;
            }

            if (text[i] == '!' && i + 1 < text.Length && text[i + 1] == '[')
            {
                if (TryParseLink(text, i + 1, out _, out _, out var imageEnd))
                {
                    i = imageEnd;
                    continue;
                }

                if (LooksLikeLink(text, i + 1))
                {
                    errors.Add("Unclosed image link.");
                    i += 2;
                    continue;
                }
            }

            if (text[i] == '[')
            {
                if (TryParseLink(text, i, out _, out _, out var linkEnd))
                {
                    i = linkEnd;
                    continue;
                }

                if (LooksLikeLink(text, i))
                {
                    errors.Add("Unclosed markdown link.");
                    i++;
                    continue;
                }
            }

            if (TryTakeEmphasis(text, i, out _, out var emphasisEnd))
            {
                i = emphasisEnd;
                continue;
            }

            if (HasUnclosedPairedDelimiter(text, i, "***")
                || HasUnclosedPairedDelimiter(text, i, "___")
                || HasUnclosedPairedDelimiter(text, i, "**")
                || HasUnclosedPairedDelimiter(text, i, "__")
                || HasUnclosedPairedDelimiter(text, i, "~~"))
            {
                var delimiter = PeekPairedDelimiter(text, i);
                errors.Add($"Unclosed emphasis ({delimiter}).");
                i += delimiter.Length;
                continue;
            }

            i++;
        }
    }

    private static void WritePlain(string text, int start, int end, StringBuilder sb)
    {
        var i = start;
        while (i < end)
        {
            if (text[i] == '\\' && i + 1 < end)
            {
                sb.Append(text[i + 1]);
                i += 2;
                continue;
            }

            if (text[i] == '`' && TryTakeDelimited(text, i, "`", out var code, out var codeEnd))
            {
                sb.Append(code);
                i = codeEnd;
                continue;
            }

            if (text[i] == '!' && i + 1 < end && text[i + 1] == '['
                && TryParseLink(text, i + 1, out var alt, out _, out var imageEnd))
            {
                sb.Append(alt);
                i = imageEnd;
                continue;
            }

            if (text[i] == '[' && TryParseLink(text, i, out var label, out _, out var linkEnd))
            {
                sb.Append(label);
                i = linkEnd;
                continue;
            }

            if (TryTakeEmphasis(text, i, out var inner, out var emphasisEnd))
            {
                WritePlain(inner, 0, inner.Length, sb);
                i = emphasisEnd;
                continue;
            }

            sb.Append(text[i]);
            i++;
        }
    }

    private static bool TryTakeEmphasis(string text, int start, out string inner, out int end)
    {
        inner = string.Empty;
        end = start;

        if (TryTakeDelimited(text, start, "***", out inner, out end)
            || TryTakeDelimited(text, start, "___", out inner, out end)
            || TryTakeDelimited(text, start, "**", out inner, out end)
            || TryTakeFlankingDelimited(text, start, "__", out inner, out end)
            || TryTakeDelimited(text, start, "~~", out inner, out end)
            || TryTakeDelimited(text, start, "*", out inner, out end)
            || TryTakeFlankingDelimited(text, start, "_", out inner, out end))
        {
            return true;
        }

        return false;
    }

    private static bool HasUnclosedPairedDelimiter(string text, int start, string delimiter)
    {
        if (start + delimiter.Length > text.Length)
            return false;

        if (string.CompareOrdinal(text, start, delimiter, 0, delimiter.Length) != 0)
            return false;

        return !TryTakeDelimited(text, start, delimiter, out _, out _);
    }

    private static string PeekPairedDelimiter(string text, int start)
    {
        foreach (var delimiter in new[] { "***", "___", "**", "__", "~~" })
        {
            if (start + delimiter.Length <= text.Length
                && string.CompareOrdinal(text, start, delimiter, 0, delimiter.Length) == 0)
            {
                return delimiter;
            }
        }

        return "**";
    }

    private static bool TryTakeFlankingDelimited(string text, int start, string delimiter, out string inner, out int end)
    {
        inner = string.Empty;
        end = start;

        if (!IsLeftFlanking(text, start))
            return false;

        return TryTakeDelimited(text, start, delimiter, out inner, out end);
    }

    private static bool IsLeftFlanking(string text, int index)
    {
        if (index == 0)
            return true;

        return char.IsWhiteSpace(text[index - 1]) || char.IsPunctuation(text[index - 1]);
    }

    private static bool TryTakeDelimited(string text, int start, string delimiter, out string inner, out int end)
    {
        inner = string.Empty;
        end = start;

        if (start + delimiter.Length * 2 > text.Length)
            return false;

        if (string.CompareOrdinal(text, start, delimiter, 0, delimiter.Length) != 0)
            return false;

        var close = text.IndexOf(delimiter, start + delimiter.Length, StringComparison.Ordinal);
        if (close < 0)
            return false;

        inner = text.Substring(start + delimiter.Length, close - start - delimiter.Length);
        if (inner.Length == 0)
            return false;

        end = close + delimiter.Length;
        return true;
    }

    private static bool TryParseLink(string text, int start, out string label, out string url, out int end)
    {
        label = url = string.Empty;
        end = start;

        if (start >= text.Length || text[start] != '[')
            return false;

        var close = text.IndexOf("](", start + 1, StringComparison.Ordinal);
        if (close < 0)
            return false;

        var urlEnd = text.IndexOf(')', close + 2);
        if (urlEnd < 0)
            return false;

        label = text.Substring(start + 1, close - start - 1);
        url = text.Substring(close + 2, urlEnd - close - 2);
        end = urlEnd + 1;
        return true;
    }

    private static bool LooksLikeLink(string text, int start)
    {
        if (start >= text.Length || text[start] != '[')
            return false;

        return text.IndexOf("](", start + 1, StringComparison.Ordinal) >= 0;
    }
}
