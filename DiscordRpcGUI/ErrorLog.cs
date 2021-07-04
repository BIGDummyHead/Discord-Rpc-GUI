using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordRPC.Logging;

namespace DiscordRpcGUI
{
    public class ErrorLog : ILogger
    {
        public static ErrorLog Instance { get; private set; }
        public LogLevel Level { get; set; } = LogLevel.Trace;

        public ErrorLog()
        {
            Instance = this;
            Wipe();
        }

        const string errorLog = "rpc.log";

        public void Wipe()
        {
            File.WriteAllText(errorLog, "");
        }

       

        public void Write(string msg, LogLevel msgType = LogLevel.Info)
        {
            try
            {
                File.AppendAllText(errorLog, Format(msg, msgType));
            }
            catch (IOException ex)
            {

            }
        }

        string Format(string msg, LogLevel deb)
        {
            return $"[{DateTime.UtcNow}] - {msg} | {deb}\r\n";
        }

        public void Trace(string message, params object[] args)
        {
            Write(message, LogLevel.Trace);
        }

        public void Info(string message, params object[] args)
        {
            Write(message, LogLevel.Info);
        }

        public void Warning(string message, params object[] args)
        {
            Write(message, LogLevel.Warning);
        }

        public void Error(string message, params object[] args)
        {
            Write(message, LogLevel.Error);
        }

    }
}
