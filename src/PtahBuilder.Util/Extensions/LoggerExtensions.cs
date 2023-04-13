using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.Util.Extensions;

public static class LoggerExtensions
{
    public static void InfoFrom(this ILogger logger, object context, string message)
    {
        var desc = context.GetType().GetTypeName();

        logger.Info($"[{desc}] {message}");
    }
}