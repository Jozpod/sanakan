namespace Shinden.Logger.In
{
    public class InLogger : IInLogger
    {
        private LogLevel LoggingLevel { get; set; }
        private ILogger Logger { get; set; }

        public InLogger()
        {
            LoggingLevel = LogLevel.None;
            Logger = null;
        }

        public void EnableLogger(LogLevel level, ILogger logger)
        {
            LoggingLevel = level;
            Logger = logger;
        }

        // IInLogger
        public void Log(LogLevel level, string message)
        {
            if (Logger == null) return;

            var configured = (int) LoggingLevel;
            var entry = (int) level;

            if (entry < configured) return;

            Logger.Log(message);
        }
    }
}