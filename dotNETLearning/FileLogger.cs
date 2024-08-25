namespace dotNETLearning
{
    public class FileLogger(string path) : ILogger, IDisposable
    {
        public string FilePath = path;
        private static object _lock = new object();

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public void Dispose()
        {
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            lock (_lock)
            {
                File.AppendAllText(FilePath, formatter(state, exception) + Environment.NewLine);
            }
        }
    }

    public class FileLoggerProvider(string path) : ILoggerProvider
    {
        string path = path;

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(path);
        }

        public void Dispose()
        {
        }
    }

    public static class FileLoggerExtensions
    {
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string filePath)
        {
            builder.AddProvider(new FileLoggerProvider(filePath));
            return builder;
        }
    }
}
