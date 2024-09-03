using System.Linq;
#if UNITY_WSA
using Microsoft.MixedReality.OpenXR;
#endif
using RealityCollective.Utilities.Extensions;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace ServiceFrameworkExtensions.MarkerTracking
{
    [System.Runtime.InteropServices.Guid("00ba5159-64dc-4078-a08f-ec82cf208b56")]
    public class HL2QrCodeTrackingService : BaseMarkerTrackingService, IMarkerTrackingService
    {
        public HL2QrCodeTrackingService(string name, uint priority, TrackedMarkerServiceProfile profile)
            : base(name, priority, profile)
        {
        }
        
#if UNITY_WSA        
        private ARMarkerManager arMarkerManager;

        public override void Enable()
        {
            if (arMarkerManager == null)
            {
                var markerManagerGameObject = new GameObject
                {
                    name = "MarkerManager"
                };
                markerManagerGameObject.AddComponent<XROrigin>();
                arMarkerManager = markerManagerGameObject.AddComponent<ARMarkerManager>();
                arMarkerManager.defaultTransformMode = TransformMode.Center;
            }
            arMarkerManager.markersChanged += OnMarkersChanged;
        }

        public override void Disable()
        {
            if(arMarkerManager != null)
            {
                arMarkerManager.markersChanged -= OnMarkersChanged;
            }
        }

        public override void Destroy()
        {
            if(arMarkerManager != null)
            {
                arMarkerManager.markersChanged -= OnMarkersChanged;
                arMarkerManager.gameObject.Destroy();
            }
        }
        
        private void OnMarkersChanged(ARMarkersChangedEventArgs changedEventArgs)
        {
            NotifyMarkersChanged(ToMarkerArgs(changedEventArgs));
        }
        
        private TrackedMarkerArgs ToMarkerArgs(ARMarkersChangedEventArgs args)
        {
            foreach (var marker in args.added.Concat(args.updated))
            {
                var id = marker.trackableId.ToString();
                if( KnownMarkers.TryGetValue(id, out var knownMarker))
                {
                    UpdateMarker(marker, knownMarker);
                }
                else
                {
                    KnownMarkers.Add(id, CreateNewMarker(marker, id));
                }
            }

            return CreateMarkerArgs();
        }

        private TrackedMarker CreateNewMarker(ARMarker arMarker, string id)
        {
            var marker = new TrackedMarker
            {
                Id = id,
                Payload = arMarker.GetDecodedString(),
                IsTracked = true
            };
            UpdateMarker(arMarker, marker);
            return marker;
        }
        
        private void UpdateMarker(ARMarker marker, TrackedMarker trackedMarker)
        {
            trackedMarker.Pose = new Pose(marker.transform.position, 
                marker.transform.rotation * Quaternion.Euler(90, 0, 0));
            trackedMarker.Size = marker.size;
            trackedMarker.LastSeenTime = marker.lastSeenTime;
            trackedMarker.IsTracked = true;
        }
#endif 
    }
}
