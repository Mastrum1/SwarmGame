using UnityEngine;

namespace Scripts
{
    public class CameraZoomOut : MonoBehaviour
    {
        [SerializeField] private Transform _camera;
        [SerializeField] private float _initialZoom;
        [SerializeField] private float _endZoom;

        private void Start()
        {
            MainGame.Instance.OnGarbageCleaned += UpdateZoom;
            _camera.localPosition = new Vector3(_initialZoom-2, _initialZoom, _camera.localPosition.z);
        }

        private void UpdateZoom(Garbage.Garbage garbage)
        {
            var zoomLevel = Mathf.Lerp(_initialZoom, _endZoom, MainGame.Instance.CleaningManager.CleanedPercentage);
            _camera.localPosition = new Vector3(zoomLevel-2, zoomLevel, _camera.localPosition.z);
        }
    }
}