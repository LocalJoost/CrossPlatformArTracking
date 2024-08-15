using TMPro;
using UnityEngine;

namespace MarkerTrackingDemo
{
    public class AnnotationController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro text;

        public void SetText(string txt)
        {
            text.text = txt;
        }

    }
}
