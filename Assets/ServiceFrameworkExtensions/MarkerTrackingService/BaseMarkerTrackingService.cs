using System;
using System.Collections.Generic;
using RealityCollective.ServiceFramework.Services;
using UnityEngine;

namespace ServiceFrameworkExtensions.MarkerTracking
{
    public abstract class BaseMarkerTrackingService : BaseServiceWithConstructor
    {
        protected readonly TrackedMarkerServiceProfile Profile;
        protected readonly Dictionary<string, TrackedMarker> KnownMarkers = new();
        protected float LastEventUpdate = -2f;

        public event Action<ITrackedMarkerChangedEventArgs> MarkersChanged;
        
        protected BaseMarkerTrackingService(string name, uint priority, TrackedMarkerServiceProfile profile)
            : base(name, priority)
        {
            Profile = profile;
        }
        
        public override void Update()
        {
            if (Time.time - LastEventUpdate > Profile.TrackingLostTimeOut)
            {
                NotifyMarkersChanged(CreateMarkerArgs());
            }
        }
        
        protected void NotifyMarkersChanged(TrackedMarkerArgs args)
        {
            MarkersChanged?.Invoke(args);
            LastEventUpdate = Time.time;
        }
        
        protected TrackedMarkerArgs CreateMarkerArgs()
        {
            PruneUntrackedMarkers();

            var result = new TrackedMarkerArgs();
            result.MarkersInternal.AddRange(KnownMarkers.Values);
            return result;  
        }
        
        private void PruneUntrackedMarkers()
        {
            foreach( var marker in KnownMarkers.Values)
            {
                if( marker.LastSeenTime < Time.time - Profile.TrackingLostTimeOut)
                {
                    marker.IsTracked = false;
                }
            }
        }
    }
}