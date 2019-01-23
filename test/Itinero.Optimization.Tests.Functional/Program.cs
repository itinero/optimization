using System;
using System.Collections.Generic;
using Itinero.Logging;
using Itinero.Optimization.Solvers.Shared.Seeds;
using Itinero.Optimization.Tests.Functional.Solvers.CVRP_ND.Construction;
using Itinero.Optimization.Tests.Functional.Solvers.Shared.Seeds;
using Itinero.Optimization.Tests.Functional.Solvers.Tours.Hull;
using Serilog;
using Serilog.Events;

namespace Itinero.Optimization.Tests.Functional
{
    public class Program
    {
        public static void Main(string[] args)
        {
            EnableLogging();

            // setup json stuff.
            Itinero.Optimization.IO.Json.JsonSerializer.ToJsonFunc = Newtonsoft.Json.JsonConvert.SerializeObject;
            Itinero.Optimization.IO.Json.JsonSerializer.FromJsonFunc = Newtonsoft.Json.JsonConvert.DeserializeObject;

#if DEBUG
            Logger.Log($"{typeof(Program)}.{nameof(Main)}", TraceEventType.Information,
                "Performance tests are running in Debug, please run in Release mode.");
#endif

            // shared tools fuctional testing.
            //SeedHeuristicsTest.TestLocations1_GetSeedsKMeans();

            // CVRP_ND tools functional testing.
            SeededConstructionHeuristicTests.TestLocations1_SeededConstructionHeuristic();
            
            // quick hull functional testing.
            //QuickHullTests.Run();
            
            // invoke case-specific tests.
            //TSP.TSPTests.Run();
            //TSP_D.TSPDTests.Run();
            //TSP_TW.TSPTWTests.Run();
            //STSP.STSPTests.Run();
            //CVRP.CVRPTests.Run();
            //CVRP_ND.CVRPNDTests.Run();
        }

        public static bool DoIntermediates { get; set; } = false;

        private static void EnableLogging()
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

#if DEBUG
            var loggingBlacklist = new HashSet<string>();
#else
            var loggingBlacklist = new HashSet<string>();
#endif
            Logging.Logger.LogAction = (o, level, message, parameters) =>
            {
                if (loggingBlacklist.Contains(o))
                {
                    return;
                }

                if (level == Logging.TraceEventType.Verbose.ToString().ToLower())
                {
                    Log.Debug($"[{o}] {level} - {message}");
                }
                else if (level == Logging.TraceEventType.Information.ToString().ToLower())
                {
                    Log.Information($"[{o}] {level} - {message}");
                }
                else if (level == Logging.TraceEventType.Warning.ToString().ToLower())
                {
                    Log.Warning($"[{o}] {level} - {message}");
                }
                else if (level == Logging.TraceEventType.Critical.ToString().ToLower())
                {
                    Log.Fatal($"[{o}] {level} - {message}");
                }
                else if (level == Logging.TraceEventType.Error.ToString().ToLower())
                {
                    Log.Error($"[{o}] {level} - {message}");
                }
                else
                {
                    Log.Debug($"[{o}] {level} - {message}");
                }
            };
            OsmSharp.Logging.Logger.LogAction = (o, level, message, parameters) =>
            {
#if RELEASE
                if (level == "verbose")
                {
                    return;
                }
#endif
                Log.Information(message);
                Console.WriteLine(string.Format("[{0}] {1} - {2}", o, level, message));
            };
        }
    }
}