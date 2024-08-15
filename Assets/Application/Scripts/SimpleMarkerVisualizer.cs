using System;
using System.Collections.Generic;
using System.Linq;
using RealityCollective.ServiceFramework.Services;
using ServiceFrameworkExtensions.MarkerTracking;
using UnityEngine;

namespace MarkerTrackingDemo
{
    public class MarkerVisualizer : MonoBehaviour
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
            foreach(var marker in args.Markers.Where(p=> p.IsTracked))
            {
                if (!markers.TryGetValue(marker.Id, out var go))
                {
                    go = Instantiate(markerPrefab, marker.Pose.position, marker.Pose.rotation);
                    go.GetComponent<AnnotationController>().SetText(marker.Payload);
                    markers.Add(marker.Id, go);
                }
                else
                {
                    go.transform.position = marker.Pose.position;
                    go.transform.rotation = marker.Pose.rotation;
                }
                ScaleMarker(go, marker.Size);
            }

            foreach(var marker in args.Markers.Where(p=> !p.IsTracked))
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
            var scaleFactor = (float)Math.Sqrt(size.x * size.y);
            marker.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }
        
        private void OnDestroy()
        {
            trackingService.MarkersChanged -= OnMarkersChanged;
        }
    }
}