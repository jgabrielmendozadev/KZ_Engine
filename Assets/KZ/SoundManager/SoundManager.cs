using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KZ.Audio {
    public static class SoundManager {

        const string AUDIO_EMITTER_PATH = "Audio/AudioEmitter";
        [RuntimeInitializeOnLoadMethod]
        static void Initialize() {
            _n = new GameObject().AddComponent<SoundManager_CoroutineHandler>();
            _parent = _n.transform;
            Object.DontDestroyOnLoad(_n);
            _parent.name = "SoundManager";

            //MIXER
            _mixer = Resources.Load<AudioEmitter>(AUDIO_EMITTER_PATH)
                .GetAudioSource().outputAudioMixerGroup.audioMixer;

            //AUDIO SOURCES
            _SFX = new GameObject("SFX").AddComponent<AudioSource>();
            _BGM = new GameObject("BGM").AddComponent<AudioSource>();
            _VOICE = new GameObject("VOICE").AddComponent<AudioSource>();

            _SFX.outputAudioMixerGroup = _mixer.FindMatchingGroups("SFX")[0];
            _BGM.outputAudioMixerGroup = _mixer.FindMatchingGroups("BGM")[0];
            _VOICE.outputAudioMixerGroup = _mixer.FindMatchingGroups("VOICE")[0];

            _BGM.loop = true;
            _SFX.playOnAwake = _BGM.playOnAwake = _VOICE.playOnAwake = false;
            _SFX.transform.parent = _BGM.transform.parent = _VOICE.transform.parent = _parent;

            //EMITTERS
            _emitters = new Stack<AudioEmitter>();

            LoadScene.OnResetApp += ResetValues;
        }
        static void ResetValues() {
            if (_emitterParent) Object.Destroy(_emitterParent.gameObject); //Destroys every audioEmitter
            _emitterParent = new GameObject("Emitters").transform;
            _emitterParent.SetParent(_parent);
            _emitters.Clear();
            _SFX.Stop();
            _BGM.Stop();
            _VOICE.Stop();
            _SFX.clip = null;
            _BGM.clip = null;
            _VOICE.clip = null;
            _n.StopAllCoroutines();
            _n.StartCoroutine(CR_PoolHandler());
        }


        static UnityEngine.Audio.AudioMixer _mixer;
        static SoundManager_CoroutineHandler _n;
        static Transform _parent;
        static AudioSource _SFX, _BGM, _VOICE;


        #region SFX
        public static void SFX_Play(AudioClip audioClip) {
            _SFX.PlayOneShot(audioClip);
        }
        public static void SFX_Play(AudioClip audioClip, Action onLoopPointReached) {
            _SFX.PlayOneShot(audioClip);
            _n.StartCoroutine(CR_ExecuteInSeconds(audioClip.length, onLoopPointReached));
        }
        public static void SFX_Play(AudioClip audioClip, Vector3 position) {
            //PoolManager.GetObject<AudioEmitter>("AudioEmitters")
            GetEmitter().SetValues(position, audioClip, _SFX.outputAudioMixerGroup);
        }
        public static void SFX_Play(AudioClip audioClip, Vector3 position, Action onLoopPointReached) {
            //PoolManager.GetObject<AudioEmitter>("AudioEmitters")
            GetEmitter().SetValues(position, audioClip, _SFX.outputAudioMixerGroup);
            _n.StartCoroutine(CR_ExecuteInSeconds(audioClip.length, onLoopPointReached));
        }
        #endregion

        #region BGM
        public static AudioClip BGM_GetCurrent() { return _BGM.clip; }

        /// <summary>from the beginning</summary>
        public static void BGM_Play(AudioClip clip, bool useLoop = false) {
            BGM_Stop();
            _BGM.clip = clip;
            _BGM.loop = useLoop;
            _BGM.Play();
        }
        public static void BGM_Play(AudioClip clip, Action onLoopPointReached, bool useLoop = false) {
            BGM_Stop();
            _BGM.clip = clip;
            _BGM.loop = useLoop;
            _BGM.Play();
            _n.StartCoroutine(CR_ExecuteInSeconds(clip.length, onLoopPointReached));
        }

        /// <summary>Pause/Unpause</summary>
        public static void BGM_Toggle() {
            if (_BGM.isPlaying) _BGM.Pause();
            else _BGM.UnPause();
        }
        public static void BGM_Pause() { _BGM.Pause(); }
        public static void BGM_Unpause() { _BGM.UnPause(); }

        /// <summary>Stops music</summary>
        public static void BGM_Stop() {
            _BGM.Stop();
        }
        public static void BGM_FadeIn(AudioClip newAudioClip, float fadeTime, bool useLoop = false) {
            _BGM.Stop();
            _BGM.clip = null;
            BGM_CrossFadeTo(newAudioClip, fadeTime, useLoop);
        }
        public static void BGM_FadeOut(float fadeTime) {
            BGM_CrossFadeTo(null, fadeTime);
        }
        public static void BGM_CrossFadeTo(AudioClip newAudioClip, float crossFadeTime, bool useLoop = false) {
            BGM_CrossFadeTo(newAudioClip, crossFadeTime, delegate { }, useLoop);
        }
        public static void BGM_CrossFadeTo(AudioClip newAudioClip, float crossFadeTime, Action onFinished, bool useLoop = false) {
            if (_auxBgm != null)
                Object.Destroy(_auxBgm.gameObject);
            _n.StopAllCoroutines();

            _auxBgm = _BGM;

            var b = Object.Instantiate(_auxBgm);
            b.loop = useLoop;
            b.name = _BGM.name;
            b.transform.SetParent(_BGM.transform.parent);
            b.transform.SetAsFirstSibling();
            b.Stop();
            b.volume = 0;
            b.clip = newAudioClip;
            b.Play();

            _BGM = b;

            _n.StartCoroutine(CR_CrossFade(_auxBgm, _BGM, crossFadeTime, onFinished));
        }

        static AudioSource _auxBgm;
        static IEnumerator CR_CrossFade(AudioSource oldAud, AudioSource newAud, float crossFadeTime, Action onFinished) {
            float originalVolume = oldAud.volume;
            for (float t = crossFadeTime; t >= 0; t -= Time.deltaTime) {
                var p = t / crossFadeTime;
                oldAud.volume = p * originalVolume;
                newAud.volume = 1 - p;
                yield return new WaitForEndOfFrame();
            }

            newAud.volume = 1;

            _BGM = newAud;
            Object.Destroy(oldAud.gameObject);

            onFinished();
        }

        #endregion

        #region VOICE
        public static void VOICE_Play(AudioClip clip) {
            _VOICE.PlayOneShot(clip);
        }
        public static void VOICE_Play(AudioClip clip, Action onLoopPointReached) {
            _VOICE.PlayOneShot(clip);
            _n.StartCoroutine(CR_ExecuteInSeconds(clip.length, onLoopPointReached));
        }
        public static void VOICE_Play(AudioClip clip, Vector3 pos) {
            GetEmitter().SetValues(pos, clip, _VOICE.outputAudioMixerGroup);
        }
        public static void VOICE_Play(AudioClip clip, Vector3 pos, Action onLoopPointReached) {
            GetEmitter().SetValues(pos, clip, _VOICE.outputAudioMixerGroup);
            _n.StartCoroutine(CR_ExecuteInSeconds(clip.length, onLoopPointReached));
        }
        #endregion

        #region MIXER
        public static void MIXER_SetFloat(string name, float value) {
            _mixer.SetFloat(name, value);
        }
        public static void MIXER_ClearFloat(string name) {
            _mixer.ClearFloat(name);
        }
        #endregion



        #region POOL_AUDIO_EMITTER
        static Stack<AudioEmitter> _emitters;
        static Transform _emitterParent;
        static AudioEmitter GetEmitter() {
            if (!_emitters.Any()) CreateEmitter();
            var e = _emitters.Pop();
            e.gameObject.SetActive(true);
            return e;
        }
        static void CreateEmitter() {
            var e = Object.Instantiate(Resources.Load<AudioEmitter>(AUDIO_EMITTER_PATH), _emitterParent);
            _emitters.Push(e);
            e.OnFinishedAudio += ReturnEmitterToPool;
        }
        static void ReturnEmitterToPool(AudioEmitter e) {
            _emitters.Push(e);
            e.gameObject.SetActive(false);
        }

        //destroy one emitter every 60 seconds
        static float _timeToDestroy = 60f;
        static int _minPoolSize = 3;
        static IEnumerator CR_PoolHandler() {    
            while (true) {
                yield return new WaitForSeconds(_timeToDestroy);
                if (_emitters.Any() && _emitters.Count > _minPoolSize)
                    Object.Destroy(_emitters.Pop().gameObject);
            }
        }
        #endregion


        //UTILITY
        static IEnumerator CR_ExecuteInSeconds(float time, Action Callback) {
            yield return new WaitForSeconds(time);
            Callback();
        }

    }

    class SoundManager_CoroutineHandler : MonoBehaviour { }
}