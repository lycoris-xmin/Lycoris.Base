using Microsoft.Extensions.Logging;

namespace Lycoris.Base.Logging.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class LycorisLogger : ILycorisLogger
    {
        private readonly ILogger _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public LycorisLogger(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public bool IsEnabled(LogLevel logLevel) => _logger.IsEnabled(logLevel);

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message) => _logger.LogInformation("{message}", message);

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message"></param>
        /// <param name="traceId"></param>
        public void Info(string message, string traceId) => _logger.LogInformation("{traceId} - {message}", traceId, message);

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message"></param>
        public void Warn(string message) => _logger.LogWarning("{message}", message);

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message"></param>
        /// <param name="traceId"></param>
        public void Warn(string message, string traceId) => _logger.LogWarning("{traceId} - {message}", traceId, message);

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void Warn(string message, Exception? ex) => _logger.LogWarning(ex, "{message}", message);

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="traceId"></param>
        public void Warn(string message, Exception? ex, string traceId) => _logger.LogWarning(ex, "{traceId} - {message}", traceId, message);

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message) => _logger.LogError("{message}", message);

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message"></param>
        /// <param name="traceId"></param>
        public void Error(string message, string traceId) => _logger.LogError("{traceId} - {message}", traceId, message);

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void Error(string message, Exception? ex) => _logger.LogError(ex, "{message}", message);

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="traceId"></param>
        public void Error(string message, Exception? ex, string traceId) => _logger.LogError(ex, "{traceId} - {message}", traceId, message);
    }
}
