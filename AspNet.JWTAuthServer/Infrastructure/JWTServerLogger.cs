using System;
using System.Configuration;
using System.Net.Mail;
using AspNet.IdentityEx.NPoco;
using NLog;
using NPoco;

namespace AspNet.JWTAuthServer.Infrastructure
{

    public class JWTServerLogger : IDisposable
    {

        private static Logger _logger = LogManager.GetLogger("AspNet.JWTAuthServer");

        public JWTServerLogger()
        {
        }

        public static JWTServerLogger Create()
        {
            return new JWTServerLogger();
        }


        public void SetName(string loggerName)
        {
            _logger = LogManager.GetLogger(loggerName);
        }


        private void Log(LogLevel lvl, string message)
        {
            _logger.Log(lvl, message);
        }


        public void LogDebug(string message)
        {
            _logger.Log(LogLevel.Debug, message);
        }


        public void LogError(string message)
        {
            _logger.Log(LogLevel.Error, message);
        }


        public void LogFatal(string message)
        {
            _logger.Log(LogLevel.Fatal, message);
        }


        public void LogInfo(string message)
        {
            _logger.Log(LogLevel.Info, message);
        }


        public void LogWarn(string message)
        {
            _logger.Log(LogLevel.Warn, message);
        }


        public void LogEx(string message, Exception ex)
        {
            _logger.Error(ex, message);
        }


        public void Dispose()
        {
            _logger = null;
            GC.SuppressFinalize(this);
        }

    }

}