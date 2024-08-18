using System;
using System.Collections.Generic;
using System.Linq;
using RealityCollective.ServiceFramework.Services;
using ServiceFrameworkExtensions.MarkerTracking;
using UnityEngine;
using UnityEngine.Serialization;

namespace MarkerTrackingDemo
{
    public class PerMarkerVisualizer: MonoBehaviour
    {
        [SerializeField]
        private List<MarkerConfig> markerConfiguration;
        
        private IMarkerTrackingService trackingService;
        private readonly Dictionary<string, GameObject> markers = new();

        private async void Start()
        {
            await ServiceManager.WaitUntilInitializedAsync();
            trackingService = ServiceManager.Instance.GetService<IMarkerTrackingService>();
            trackingService.MarkersChanged += OnMarkersChanged;
        }
        
        private void OnMarkersChanged(ITrackedMarkerChangedEventArgs args)
        {
            AddOrUpdateTrackedMarkers(args.Markers.Where(t => t.IsTracked));
            RemoveUntrackedMarkers(args.Markers.Where(t => !t.IsTracked));
        }
        
        private void AddOrUpdateTrackedMarkers(IEnumerable<ITrackedMarker> trackedMarkers)
        {
            foreach(var marker in trackedMarkers)
            {
                var markerConfig = GetMarkerConfigForPayload(marker.Payload);
                if( markerConfig == null)
                {
                    continue;
                }
                if (!markers.TryGetValue(marker.Id, out var go))
                {
                    go = Instantiate(markerConfig.Prefab, marker.Pose.position, marker.Pose.rotation);
                    go.GetComponent<AnnotationController>().SetText(marker.Payload);
                    markers.Add(marker.Id, go);
                }
                else
                {
                    go.transform.position = marker.Pose.position;
                    go.transform.rotation = marker.Pose.rotation;
                }
                
                if( markerConfig.Scale)
                {
                    ScaleMarker(go, marker.Size);
                }
            }
        }
        
        private void RemoveUntrackedMarkers(IEnumerable<ITrackedMarker>untrackedMarkers)
        {
            foreach (var marker in untrackedMarkers)
            {
                if (markers.ContainsKey(marker.Id))
                {
                    Destroy(markers[marker.Id]);
                    markers.Remove(marker.Id);
                }
            }
        }
        
        private MarkerConfig GetMarkerConfigForPayload(string payload)
        {
            return markerConfiguration.FirstOrDefault(p => p.PayLoad == payload);
        }
        
        private void ScaleMarker(GameObject marker, Vector2 size)
        {
            var scaleFactor = (float)Math.Sqrt(size.x * size.y);
            marker.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }
        
        private void OnDestroy()
        {
            trackingService.MarkersChanged -= OnMarkersChanged;
        }
    }
}