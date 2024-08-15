using UnityEngine;
using UnityEngine.Serialization;

namespace MRTKExtensions.Animation
{
    public class LookatCamera : MonoBehaviour
    {
        [FormerlySerializedAs("RotateAngle")]
        [SerializeField]
        private float rotateAngle = 180f;

        void Update()
        {
            gameObject.transform.LookAt(Camera.main.transform);
            gameObject.transform.Rotate(Vector3.up, rotateAngle);
        }
    }
}
