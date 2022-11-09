using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Web;

// ReSharper disable UnusedVariable

namespace PtahBuilder.BuildSystem;

public class Logger
{
    private readonly List<string> _warnings = new List<string>();
    private readonly List<string> _infos = new List<string>();
    private readonly List<string> _errors = new List<string>();
    private readonly Dictionary<string, Section> _sections = new Dictionary<string, Section>();
    private readonly Dictionary<string, object[]> _contents = new Dictionary<string, object[]>();
    private class Section
    {
        public Section(int priority, string[] messages)
        {
            Messages = messages;
            Priority = priority;
        }

        public int Priority { get; }
        public string[] Messages { get; private set; }

        public void AppendMessages(params string[] messages)
        {
            Messages = Messages.Union(messages).ToArray();
        }
    }

    public void LogSection(string title, IEnumerable<string> messages, int priority = 0)
    {
        LogSection(title, priority, messages.ToArray());
    }

    public void LogSection(string title, params string[] messages)
    {
        LogSection(title, 0, messages);
    }

    public void LogSection(string title, int priority, params string[] messages)
    {
        if (!_sections.ContainsKey(title))
        {
            _sections.Add(title, new Section(priority, messages));
        }
        else
        {
            _sections[title].AppendMessages(messages);
        }
    }

    public void Error(string error)
    {
        _errors.Add(error);
        var col = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(error);
        Console.ForegroundColor = col;
    }

    public void Warning(string warning)
    {
        _warnings.Add(warning);
        var col = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(warning);
        Console.ForegroundColor = col;
    }

    public void Info(string info)
    {
        _infos.Add(info);
        Console.WriteLine(info);
    }

    public string ToHtml()
    {
        var sb = new StringBuilder();

        using (var html = new LazyHtml(sb, "html"))
        {
            using (var body = new LazyHtml(sb, "body"))
            {
                using (var h1 = new LazyHtml(sb, "h1"))
                {
                    h1.Write($"Build Report {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
                }

                LiWithHeader(sb, "Errors", _errors);
                
                LiWithHeader(sb, "Warnings", _warnings);

                foreach (var content in _contents)
                {
                    Content(sb, content);
                }

                foreach (var section in _sections.Where(s => s.Value.Messages.Any()).OrderByDescending(s => s.Value.Priority))
                {
                    LiWithHeader(sb, section.Key, section.Value.Messages);
                }

                LiWithHeader(sb, "Info", _infos);
            }
        }

        return sb.ToString();
    }

    private void Content(StringBuilder sb, KeyValuePair<string, object[]> content)
    {
        if (content.Value.Length == 0)
        {
            return;
        }

        using (var h2 = new LazyHtml(sb, "h2"))
        {
            h2.Write(content.Key);
        }

        var properties = content.Value.First().GetType().GetProperties();
        using (var table = new LazyHtml(sb, "table cellspacing=0 cellpadding=5"))
        {
            using (var trh = new LazyHtml(sb, "tr"))
            {
                foreach (var property in properties)
                {
                    using (var tdh = new LazyHtml(sb, "td style=\"border: 1px solid gray\""))
                    {
                        tdh.Write(property.Name);
                    }
                }
            }

            foreach (var entity in content.Value)
            {
                using (var tr = new LazyHtml(sb, "tr"))
                {
                    foreach (var property in properties)
                    {
                        using (var td = new LazyHtml(sb, "td  style=\"border: 1px solid gray\""))
                        {
                            var value = property.GetValue(entity);
                            if (value != null)
                            {
                                td.Write(value.ToString());
                            }
                        }
                    }
                }
            }
        }
    }

    public void ToReportHtml(string filename = "report.html", bool open = false)
    {
        var report = ToHtml();
        var html = Path.Combine(Directory.GetCurrentDirectory(), filename);
        File.WriteAllText(html, report);
        if (open)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start {html}"));
        }
    }

    private class LazyHtml : IDisposable
    {
        private readonly StringBuilder _sb;
        private readonly string _tag;

        public LazyHtml(StringBuilder sb, string tag)
        {
            _sb = sb;
            _tag = tag;

            sb.AppendLine($"<{_tag}>");
        }

        public void Dispose()
        {
            _sb.AppendLine($"</{_tag}>");
        }

        public void Write(string text)
        {
            _sb.Append(HttpUtility.HtmlEncode(text));
        }
    }

    private void LiWithHeader(StringBuilder sb, string header, IEnumerable<string> messages)
    {
        var enumerable = messages as string[] ?? messages.ToArray();
        if (!enumerable.Any())
        {
            return;
        }

        using (var h2 = new LazyHtml(sb, "h2"))
        {
            h2.Write(header);
        }

        using (var ul = new LazyHtml(sb, "ul"))
        {
            foreach (var message in enumerable)
            {
                using (var li = new LazyHtml(sb, "li"))
                {
                    ul.Write(message);
                }
            }
        }
    }

    public void AddContent<T>(string name, IEnumerable<T> content)
    {
        _contents.Add(name, content.Cast<object>().ToArray());
    }
}