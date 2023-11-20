using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Common
{
    public class LOGGER
    {
        private static readonly ILog log = LogManager.GetLogger("app");

        public static void Error(string message)
        {
            Console.WriteLine(message);
            log.Error(message);
        }

        public static void Error(string message, Exception mobject)
        {
            Console.WriteLine(message);
            Console.WriteLine(mobject);
            log.Error(message, mobject);
        }

        public static void Info(string message)
        {
            Console.WriteLine(message);
            log.Info(message);
        }

        public static void Warning(string message)
        {
            Console.WriteLine(message);
            log.Warn(message);
        }

        public static void console(string message)
        {
            Console.Write(message);
        }
    }
}
