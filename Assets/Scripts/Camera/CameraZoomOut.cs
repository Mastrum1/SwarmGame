using UnityEngine;

namespace Scripts.Camera
{
    public class CameraZoomOut : MonoBehaviour
    {
        [SerializeField] private Transform _camera;
        [SerializeField] private float _initialZoom;
        [SerializeField] private float _endZoom;

        private void Start()
        {
            MainGame.Instance.OnGarbageCleaned += UpdateZoom;
            _camera.localPosition = new Vector3(_camera.localPosition.x, _initialZoom, _camera.localPosition.z);
        }

        private void UpdateZoom(Garbage.Garbage garbage)
        {
            var zoomLevel = Mathf.Lerp(_initialZoom, _endZoom, MainGame.Instance.CleaningManager.Percentage);
            _camera.localPosition = new Vector3(_camera.localPosition.x, zoomLevel, _camera.localPosition.z);
        }
    }
}