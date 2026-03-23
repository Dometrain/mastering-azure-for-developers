using System.Diagnostics.Metrics;

namespace TravelInspiration.API.Destinations.Shared.Metrics;

public sealed class HandlerPerformanceMetric
{
    private readonly Counter<long> _milliSecondsElapsed;
    public HandlerPerformanceMetric(IMeterFactory meterFactory)
    {
        // a meter
        var meter = meterFactory.Create("TravelInspiration.API.Destinations");
        _milliSecondsElapsed = meter.CreateCounter<long>(
            "TravelInspiration.API.Destinations.requesthandler.millisecondselapsed");        
    }

    public void MilliSecondsElapsed(long milliSecondsElapsed)
           => _milliSecondsElapsed.Add(milliSecondsElapsed);
}
