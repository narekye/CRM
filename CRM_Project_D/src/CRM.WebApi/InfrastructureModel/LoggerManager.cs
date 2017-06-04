namespace CRM.WebApi.InfrastructureModel
{
    using NLog;
    using NLog.Targets;
    using System;
    using System.IO;
    using System.Net.Http;
    public class LoggerManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public LoggerManager()
        {
            FileTarget loggerTarget = (FileTarget)LogManager.Configuration.FindTargetByName("file");
            loggerTarget.DeleteOldFileOnStartup = false;
        }
        public void LogError(Exception ex, HttpMethod request, Uri uri)
        {
            Logger.Log(LogLevel.Fatal, $"\nRequest: [ {request} ] | URL [ {uri} ]\nErr: {ex.Message}\nInner: {ex.InnerException?.Message}\n" + new string('-', 120));
        }
        public string ReadData()
        {
            var fileTarget = (FileTarget)LogManager.Configuration.FindTargetByName("file");
            var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
            string fileName = fileTarget.FileName.Render(logEventInfo);
            if (!File.Exists(fileName))
                File.Create($"{logEventInfo.TimeStamp}.log");
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