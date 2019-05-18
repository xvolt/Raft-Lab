using NLog;
using System;
using System.Collections.Generic;

namespace Common
{
    public static class Logger
    {
        private static List<ILogger> loggers = new List<ILogger>();
        public static void Log(string message)
        {
            loggers.ForEach(x=>x.Info(message));
        }

        public static void Log(string message, Exception ex)
        {
            loggers.ForEach(x => x.Error(ex,message));
        }
        public static void Log( Exception ex)
        {
            loggers.ForEach(x => x.Error(ex));
        }

        public static void Configure()
        {
            var f = LogManager.LoadConfiguration("nlog.config");
            loggers.Add(f.GetLogger("Console"));
            loggers.Add(f.GetLogger("File"));
        }
    }
}
