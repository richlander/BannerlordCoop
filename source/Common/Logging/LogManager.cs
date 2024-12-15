using System;
using System.Collections.Generic;
using Serilog;
using Serilog.Core;

namespace Common.Logging;

public static class LogManager
{
	public static LoggerConfiguration Configuration { get; set; } = new LoggerConfiguration();
	
	public static List<ILogEventSink> Sinks { get; } = new List<ILogEventSink>();


	// If this is called before the Configuration is setup, logging does not work
	private static Lazy<ILogger> _logger = new Lazy<ILogger>(() => {

		foreach (var sink in Sinks)
		{
			Configuration = Configuration.WriteTo.Sink(sink);
		}
		return Configuration.CreateLogger();
	});

	public static ILogger GetLogger<T>() => _logger.Value
		.ForContext<T>();
}
