using Quartz.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Meloht.Quartz.JobCore
{
    public class CustomConsoleLogProvider : ILogProvider
    {
        public Logger GetLogger(string name)
        {
            return (level, func, exception, parameters) =>
            {

                if (func != null)
                {
                    var message = string.Format(CultureInfo.InvariantCulture, func(), parameters);
                    if (exception != null)
                    {
                        message = message + "|" + exception;
                    }

                    Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + message);
                }
                return true;
            };

            // return LoggerFun;
        }

        public IDisposable OpenMappedContext(string key, string value)
        {
            throw new NotImplementedException();
        }

        public IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }
    }
}
