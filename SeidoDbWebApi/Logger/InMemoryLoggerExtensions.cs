using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SeidoDbWebApi.Logger
{
    public static class InMemoryLoggerExtensions
    {
        public static ILoggingBuilder AddInMemoryLogger (this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, InMemoryLoggerProvider>();
            return builder;
        }
    }
}
