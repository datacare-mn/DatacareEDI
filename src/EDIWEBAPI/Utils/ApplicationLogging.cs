using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace EDIWEBAPI.Utils
{
    public static class ApplicationLogging
    {
        public static ILoggerFactory LoggerFactory { get; set; } //= new LoggerFactory();
        public static ILogger CreateLogger<T>() =>
          LoggerFactory.CreateLogger<T>();
    }
}
