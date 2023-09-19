using Microsoft.Extensions.Logging;

namespace Lycoris.Base.Logging.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class LycorisLoggerFactory : ILycorisLoggerFactory
    {
        private readonly ILoggerFactory _factory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        public LycorisLoggerFactory(ILoggerFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ILycorisLogger CreateLogger<T>()
        {
            var logger = _factory.CreateLogger<T>();
            return new LycorisLogger(logger);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ILycorisLogger CreateLogger(Type type)
        {
            var logger = _factory.CreateLogger(type);
            return new LycorisLogger(logger);
        }
    }
}
