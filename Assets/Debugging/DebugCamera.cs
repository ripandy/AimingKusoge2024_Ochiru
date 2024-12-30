using UnityEngine;

namespace Features.Debugging
{
    [RequireComponent(typeof(Camera))]
    public class DebugCamera : MonoBehaviour
    {
        private void Start()
        {
            var mainCamera = Camera.main;
            if (mainCamera != null)
                Destroy(gameObject);
        }
    }
}