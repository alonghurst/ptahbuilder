using System.Text;

namespace PtahBuilder.BuildSystem.Services.Reporting;

internal sealed class Report : IReport
{
    private readonly StringBuilder _builder = new();

    public void AppendLine(string line)
    {
        _builder.AppendLine(line);
    }

    internal string GetContent() => _builder.ToString();
}
