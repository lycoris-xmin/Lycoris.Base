using Lycoris.Base.Logging.Impl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Lycoris.Base.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public static class LoggingBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static IServiceCollection AddLycorisLoggerFactory<T>(this IServiceCollection services) where T : ILycorisLoggerFactory
        {
            services.TryAddSingleton(typeof(ILycorisLoggerFactory), typeof(T));
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultLoggerFactory(this IServiceCollection services)
        {
            services.TryAddSingleton<ILycorisLoggerFactory, LycorisLoggerFactory>();
            return services;
        }
    }
}
