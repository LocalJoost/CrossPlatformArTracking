using System.Collections.Generic;

namespace ServiceFrameworkExtensions.MarkerTracking
{
    public interface ITrackedMarkerChangedEventArgs
    {
        IReadOnlyList<ITrackedMarker> Markers { get; }
    }
}