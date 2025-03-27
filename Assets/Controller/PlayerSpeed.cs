using UnityEngine;


    public class PlayerSpeed : MonoBehaviour
    {
        [SerializeField] private float _initialSpeed;
        [SerializeField] private float _endSpeed;
        private PlayerController _player;

        private void Start()
        {
            MainGame.Instance.OnGarbageCleaned += UpdateSpeed;
            _player = MainGame.Instance.Player;
            _player.Speed = _initialSpeed;
        }

        private void UpdateSpeed(Garbage.Garbage garbage)
        {
            var speedLevel = Mathf.Lerp(_initialSpeed, _endSpeed, MainGame.Instance.CleaningManager.CleanedPercentage);
            _player.Speed = speedLevel;
        }
    }
