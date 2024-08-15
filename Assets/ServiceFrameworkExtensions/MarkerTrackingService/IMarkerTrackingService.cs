using System;
using RealityCollective.ServiceFramework.Interfaces;

namespace ServiceFrameworkExtensions.MarkerTracking
{
    public interface IMarkerTrackingService : IService
    {
        event Action<ITrackedMarkerChangedEventArgs> MarkersChanged;
    }
}