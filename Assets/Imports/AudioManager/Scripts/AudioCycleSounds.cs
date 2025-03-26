using AntoineFoucault.Utilities;
using UnityEngine;

namespace Imports.AudioManager.Scripts
{
    public class AudioCycleSounds : MonoBehaviour
    {
        [SerializeField] private AudioClip[] _audioClips;
        [SerializeField] private AudioSourceVariationShifter _source;
        
        public void PlaySound()
        {
            var clip = _audioClips.GetRandomItem();
            _source.PlayOneShot(clip);
        }
    }
}