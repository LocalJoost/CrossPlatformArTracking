using System.Collections.Generic;

namespace ServiceFrameworkExtensions.MarkerTracking
{
    public class TrackedMarkerArgs : ITrackedMarkerChangedEventArgs
    {
        public TrackedMarkerArgs()
        {
            MarkersInternal = new List<ITrackedMarker>();
        }
        
        public IReadOnlyList<ITrackedMarker> Markers => MarkersInternal;
        
        internal List<ITrackedMarker> MarkersInternal { get; set; }
    }
}