using UnityEngine;

namespace ServiceFrameworkExtensions.MarkerTracking
{
    public interface ITrackedMarker
    {
        string Id { get;  }
        public string Payload { get; }
        public Pose Pose { get; }
        public Vector2 Size { get; }
        public float LastSeenTime { get; }
        public bool IsTracked { get; }
    }
}