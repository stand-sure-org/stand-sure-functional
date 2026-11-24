using System.Diagnostics;

using Microsoft.Extensions.Logging;

using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using StandSure.Functional.Samples;

const string LibrarySource = "StandSure.Functional";

var endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT") ?? "http://localhost:4317";
var resource = ResourceBuilder.CreateDefault().AddService(SampleTelemetry.ServiceName);

using var loggerFactory = LoggerFactory.Create(builder => builder.AddSimpleConsole(o => o.SingleLine = true));
var logger = loggerFactory.CreateLogger("Samples");
logger.Exporting(endpoint);

using (var tracerProvider = Sdk.CreateTracerProviderBuilder()
           .SetResourceBuilder(resource)
           .AddSource(LibrarySource, SampleTelemetry.Source.Name)
           .AddOtlpExporter()
           .Build())
using (var meterProvider = Sdk.CreateMeterProviderBuilder()
           .SetResourceBuilder(resource)
           .AddMeter(LibrarySource)
           .AddOtlpExporter()
           .Build())
{
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
    var vendor = new VendorRateClient();

    var first = await ThirdPartyThrowsScenario.RunAsync(vendor, cts.Token);
    Report(logger, "third-party-throws", first.Match(e => e.Code, v => v.ToString()));

    var second = ValidationChainScenario.Run();
    Report(logger, "validation-chain", second.Match(e => e.Code, v => v.Count.ToString()));

    var third = await AsyncUpstreamFailureScenario.RunAsync(cts.Token);
    Report(logger, "async-upstream-failure", third);

    tracerProvider.ForceFlush();
    meterProvider.ForceFlush();
}

static void Report(ILogger logger, string scenario, string outcome) =>
    logger.ScenarioFinished(scenario, outcome, Activity.Current?.TraceId.ToString() ?? "(see collector)");
