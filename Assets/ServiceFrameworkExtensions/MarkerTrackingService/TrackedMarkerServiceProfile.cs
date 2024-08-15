#if MAGICLEAP
using MagicLeap.OpenXR.Features.MarkerUnderstanding;
#endif
using RealityCollective.ServiceFramework.Definitions;
using RealityCollective.ServiceFramework.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace ServiceFrameworkExtensions.MarkerTracking
{
    [CreateAssetMenu(menuName = "TrackedMarkerServiceProfile", fileName = "TrackedMarkerServiceProfile",
        order = (int)CreateProfileMenuItemIndices.ServiceConfig)]
    public class TrackedMarkerServiceProfile : BaseServiceProfile<IServiceModule>
    {
        [SerializeField]
        private float trackingLostTimeOut = 2.0f; 
        
        public float TrackingLostTimeOut => trackingLostTimeOut;

        #if MAGICLEAP
        
        public MarkerDetectorProfile DetectorProfile => detectorProfile;
        
        public bool EnableAruco => enableAruco;
        
        public bool EnableQR => enableQR;
        
        [SerializeField]
        MarkerDetectorProfile detectorProfile = MarkerDetectorProfile.Default; 
        
        [SerializeField]
        private bool enableAruco = false;

        [SerializeField]
        private bool enableQR = true;
#endif
    }
}
