using System;

namespace log4net
{
    public interface ILog
    {
        void Info(string message);
        void Error(string message, Exception ex);
        void Error(string message);
        void Debug(string message);
    }

    public static class LogManager
    {
        public static ILog GetLogger(Type t) => new SimpleLogger(t);
    }

    internal class SimpleLogger : ILog
    {
        private readonly string _name;
        public SimpleLogger(Type t) { _name = t?.FullName ?? "log"; }
        public void Info(string message) { System.Diagnostics.Trace.WriteLine($"INFO [{_name}] {message}"); }
        public void Error(string message, Exception ex) { System.Diagnostics.Trace.WriteLine($"ERROR [{_name}] {message} - {ex}"); }
        public void Error(string message) { System.Diagnostics.Trace.WriteLine($"ERROR [{_name}] {message}"); }
        public void Debug(string message) { System.Diagnostics.Trace.WriteLine($"DEBUG [{_name}] {message}"); }
    }
}