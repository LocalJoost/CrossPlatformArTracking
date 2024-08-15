using UnityEngine;

namespace ServiceFrameworkExtensions.MarkerTracking
{
    public class TrackedMarker : ITrackedMarker
    {
        public string Id { get; set; }
        public string Payload { get; set; }
        public Pose Pose { get; set; }
        public Vector2 Size { get; set; }
        public float LastSeenTime { get; set; }
        public bool IsTracked { get; set; }
    }
}