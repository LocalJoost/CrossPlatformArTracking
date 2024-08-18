using System;
using System.Collections.Generic;
using System.Linq;
using RealityCollective.ServiceFramework.Services;
using ServiceFrameworkExtensions.MarkerTracking;
using UnityEngine;

namespace MarkerTrackingDemo
{
    public class SimpleMarkerVisualizer : MonoBehaviour
    {
        [SerializeField]
        private GameObject markerPrefab;

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
            foreach (var marker in trackedMarkers)
            {
                if (!markers.TryGetValue(marker.Id, out var markerGameObject))
                {
                    markerGameObject = Instantiate(markerPrefab, marker.Pose.position, marker.Pose.rotation);
                    markerGameObject.GetComponent<AnnotationController>().SetText(marker.Payload);
                    markers.Add(marker.Id, markerGameObject);
                }
                else
                {
                    markerGameObject.transform.position = marker.Pose.position;
                    markerGameObject.transform.rotation = marker.Pose.rotation;
                }
                
                ScaleMarker(markerGameObject, marker.Size);
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
        
        private void ScaleMarker(GameObject marker, Vector2 size)
        {
            var scaleFactor = Mathf.Sqrt(size.x * size.y);
            marker.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }

        private void OnDestroy()
        {
            trackingService.MarkersChanged -= OnMarkersChanged;
        }
    }
}