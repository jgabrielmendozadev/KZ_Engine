using UnityEngine;
using UnityEngine.Audio;

namespace KZ.Audio {
    [RequireComponent(typeof(AudioSource))]
    public class AudioEmitter : MonoBehaviour {

        public event System.Action<AudioEmitter> OnFinishedAudio = delegate { };

        [SerializeField] AudioSource myAudioSource = null;
        public AudioSource GetAudioSource() { return myAudioSource; }

        public AudioEmitter SetValues(Vector3 position, AudioClip clip, AudioMixerGroup mixerGroup = null) {
            myAudioSource.outputAudioMixerGroup = mixerGroup;
            transform.position = position;
            myAudioSource.clip = clip;
            myAudioSource.Play();
            return this;
        }

        private void Update() {
            if (!myAudioSource.isPlaying)
                OnFinishedAudio(this);
        }
    }
}