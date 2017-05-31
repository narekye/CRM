using System.IO;

namespace CRM.WebApi.InfrastructureModel
{
    using System;
    using System.Net.Http;
    using NLog;
    using NLog.Targets;
    public class LoggerManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public LoggerManager()
        {
            FileTarget loggerTarget = (FileTarget)LogManager.Configuration.FindTargetByName("file");
            loggerTarget.DeleteOldFileOnStartup = false;
        }
        public void LogInfo(HttpMethod request, Uri uri)
        {
            Logger.Info($"Request: [ {request} ] | URL [ {uri} ]");
        }
        public void LogError(Exception ex, HttpMethod request, Uri uri)
        {

            Logger.Error(new string('-', 60) + $"\nRequest: [ {request} ] | URL [ {uri} ]\nErr: [ {ex.Message} ] Inner: [ {ex.InnerException?.Message} ]\n" + new string('-', 120));
        }

        public string ReadData()
        {
            var fileTarget = (FileTarget)LogManager.Configuration.FindTargetByName("file");
            var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
            string fileName = fileTarget.FileName.Render(logEventInfo);
            if (!File.Exists(fileName))
                throw new Exception("Log file does not exist.");
            var data = File.ReadAllLines(fileName);
            string path = System.Web.HttpContext.Current?.Request.MapPath("~//Templates//log.html");
            var html = File.ReadAllText(path);
            string res = "";
            foreach (string s in data)
                res += s + "</br>";
            var t = html.Replace("{data}", res).Replace("{filename}", fileName);
            return t;
        }
    }
}