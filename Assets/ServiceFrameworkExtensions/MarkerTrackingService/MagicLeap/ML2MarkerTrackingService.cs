using System.Threading.Tasks;
using MagicLeap.OpenXR.Features.MarkerUnderstanding;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.OpenXR;

namespace ServiceFrameworkExtensions.MarkerTracking
{
    [System.Runtime.InteropServices.Guid("00ba5159-64dc-4078-a08f-ec82cf208b57")]
    public class ML2MarkerTrackingService : BaseMarkerTrackingService, IMarkerTrackingService
    {
        public ML2MarkerTrackingService(string name, uint priority, TrackedMarkerServiceProfile profile)
            : base(name, priority, profile)
        {
        }

#if MAGICLEAP

        private MagicLeapMarkerUnderstandingFeature markerFeature;
        private bool isInitialized;
        private XROrigin xrOrigin;
        private MarkerDetectorSettings detectorSettings;

        public override void Enable()
        {
            if (isInitialized)
            {
                return;
            }

            markerFeature = OpenXRSettings.Instance.GetFeature<MagicLeapMarkerUnderstandingFeature>();
            xrOrigin = Object.FindAnyObjectByType<XROrigin>();
            if (xrOrigin == null)
            {
                throw new MissingComponentException("No XR Origin Found, markers will not work.");
            }

            if (markerFeature == null || !markerFeature.enabled)
            {
                throw new MissingComponentException(
                    "The Magic Leap 2 Marker Understanding OpenXR Feature is missing or disabled.");
            }

            detectorSettings.MarkerDetectorProfile = Profile.DetectorProfile;
            detectorSettings = new MarkerDetectorSettings();
            if (Profile.EnableQR)
            {
                detectorSettings.QRSettings.EstimateQRLength = true;
                detectorSettings.MarkerType = MarkerType.QR;
                markerFeature.CreateMarkerDetector(detectorSettings);
            }

            if (Profile.EnableAruco)
            {
                detectorSettings.ArucoSettings.EstimateArucoLength = true;
                detectorSettings.ArucoSettings.ArucoType = ArucoType.Dictionary_5x5_50;
                detectorSettings.MarkerType = MarkerType.Aruco;
                markerFeature.CreateMarkerDetector(detectorSettings);
            }

            isInitialized = true;
        }

        public override void Update()
        {
            if (!isInitialized || !IsEnabled)
            {
                return;
            }

            var shouldNotify = false;
            base.Update();
            markerFeature.UpdateMarkerDetectors();

            foreach (var detector in markerFeature.MarkerDetectors)
            {
                if (detector.Status == MarkerDetectorStatus.Ready && ProcessDetector(detector))
                {
                    shouldNotify = true;
                }
            }

            if (shouldNotify)
            {
                NotifyMarkersChanged(CreateMarkerArgs());
            }
        }

        private bool ProcessDetector(MarkerDetector detector)
        {
            var detectedMarkers = 0;
            foreach (var data in detector.Data)
            {
                var id = GetId(data, detector.Settings.MarkerType);
                if (data.MarkerPose.HasValue)
                {
                    detectedMarkers++;
                    if (!KnownMarkers.TryGetValue(id, out var trackedMarker))
                    {
                        trackedMarker = CreateNewMarker(detector, data);
                        KnownMarkers.Add(id, trackedMarker);
                    }
                    else
                    {
                        UpdateMarker(data, trackedMarker);
                    }
                }
            }

            return detectedMarkers > 0;
        }

        public override void Destroy()
        {
            base.Destroy();
            if (markerFeature != null)
            {
                markerFeature.DestroyAllMarkerDetectors();
            }
        }

        private TrackedMarker CreateNewMarker(MarkerDetector detector, MarkerData markerData)
        {
            var id = GetId(markerData, detector.Settings.MarkerType);
            var marker = new TrackedMarker
            {
                Id = id,
                Payload = id,
                IsTracked = true
            };
            UpdateMarker(markerData, marker);
            return marker;
        }

        private void UpdateMarker(MarkerData markerData, TrackedMarker trackedMarker)
        {
            if (!markerData.MarkerPose.HasValue)
            {
                return;
            }

            var originTransform = xrOrigin.CameraFloorOffsetObject.transform;

            trackedMarker.Pose = new Pose(
                originTransform.TransformPoint(markerData.MarkerPose.Value.position),
                (originTransform.rotation * markerData.MarkerPose.Value.rotation) * Quaternion.Euler(-90, 0, 0));
            trackedMarker.Size = new Vector2(markerData.MarkerLength, markerData.MarkerLength);
            trackedMarker.LastSeenTime = Time.time;
            trackedMarker.IsTracked = true;
        }

        private string GetId(MarkerData arMarker, MarkerType markerType)
        {
            return markerType switch
            {
                MarkerType.Aruco => arMarker.MarkerNumber.ToString(),
                MarkerType.QR => arMarker.MarkerString,
                _ => null
            };
        }

#endif
    }
}