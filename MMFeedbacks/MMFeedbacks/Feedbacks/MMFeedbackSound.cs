using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace MoreMountains.Feedbacks
{
    public struct MMSfxEvent
    {
        public delegate void Delegate(AudioClip clipToPlay, AudioMixerGroup audioGroup = null, float volume = 1f, float pitch = 1f);
        static private event Delegate OnEvent;

        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        static public void Trigger(AudioClip clipToPlay, AudioMixerGroup audioGroup = null, float volume = 1f, float pitch = 1f)
        {
            OnEvent?.Invoke(clipToPlay, audioGroup, volume, pitch);
        }
    }

    [AddComponentMenu("")]
    [FeedbackPath("Sound")]
    [FeedbackHelp("This feedback lets you play the specified AudioClip, either via event (you'll need something to catch a MMSfxEvent, that's not included in this package, but that's how it's done in the Corgi Engine and TopDown Engine), or cached (AudioSource gets created on init, and is then ready to be played), or on demand (instantiated on Play). For all these methods you can define a random volume between min/max boundaries (just set the same value in both fields if you don't want randomness), random pitch, and an optional AudioMixerGroup.")]
    public class MMFeedbackSound : MMFeedback
    {
        /// <summary>
        /// The possible methods to play the sound with. 
        /// Event : sends a MMSfxEvent, you'll need a class to catch this event and play the sound
        /// Cached : creates and stores an audiosource to play the sound with, parented to the owner
        /// OnDemand : creates an audiosource and destroys it everytime you want to play the sound
        /// </summary>
        public enum PlayMethods { Event, Cached, OnDemand }

        [Header("Sound")]
        /// the sound clip to play
        public AudioClip Sfx;

        [Header("Random Sound")]
        /// an array to pick a random sfx from
        public AudioClip[] RandomSfx;

        [Header("Method")]
        /// the play method to use when playing the sound (event, cached or on demand)
        public PlayMethods PlayMethod = PlayMethods.Event;

        [Header("Volume")]
        /// the minimum volume to play the sound at
        public float MinVolume = 1f;
        /// the maximum volume to play the sound at
        public float MaxVolume = 1f;

        [Header("Pitch")]
        /// the minimum pitch to play the sound at
        public float MinPitch = 1f;
        /// the maximum pitch to play the sound at
        public float MaxPitch = 1f;

        [Header("Mixer")]
        /// the audiomixer to play the sound with (optional)
        public AudioMixerGroup SfxAudioMixerGroup;

        protected AudioClip _randomClip;
        protected AudioSource _cachedAudioSource;

        /// <summary>
        /// Custom init to cache the audiosource if required
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(GameObject owner)
        {
            base.CustomInitialization(owner);
            if (PlayMethod == PlayMethods.Cached)
            {
                // we create a temporary game object to host our audio source
                GameObject temporaryAudioHost = new GameObject("CachedFeedbackAudioSource");
                // we set the temp audio's position
                temporaryAudioHost.transform.position = owner.transform.position;
                temporaryAudioHost.transform.SetParent(owner.transform);
                // we add an audio source to that host
                _cachedAudioSource = temporaryAudioHost.AddComponent<AudioSource>() as AudioSource;
            }
        }

        /// <summary>
        /// Plays either a random sound or the specified sfx
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                if (Sfx != null)
                {
                    PlaySound(Sfx, position);
                    return;
                }

                if (RandomSfx.Length > 0)
                {
                    _randomClip = RandomSfx[Random.Range(0, RandomSfx.Length)];

                    if (_randomClip != null)
                    {
                        PlaySound(_randomClip, position);
                    }
                    
                }
            }
        }

        /// <summary>
        /// Plays a sound differently based on the selected play method
        /// </summary>
        /// <param name="sfx"></param>
        /// <param name="position"></param>
        protected virtual void PlaySound(AudioClip sfx, Vector3 position)
        {
            if (PlayMethod == PlayMethods.Event)
            {
                float volume = Random.Range(MinVolume, MaxVolume);
                float pitch = Random.Range(MinPitch, MaxPitch);
                MMSfxEvent.Trigger(sfx, SfxAudioMixerGroup, volume, pitch);
                return;
            }

            if (PlayMethod == PlayMethods.OnDemand)
            {
                float volume = Random.Range(MinVolume, MaxVolume);
                float pitch = Random.Range(MinPitch, MaxPitch);

                // we create a temporary game object to host our audio source
                GameObject temporaryAudioHost = new GameObject("TempAudio");
                // we set the temp audio's position
                temporaryAudioHost.transform.position = position;
                // we add an audio source to that host
                AudioSource audioSource = temporaryAudioHost.AddComponent<AudioSource>() as AudioSource;
                // we set that audio source clip to the one in paramaters
                audioSource.clip = sfx;
                // we set the audio source volume to the one in parameters
                audioSource.volume = volume;
                audioSource.pitch = pitch;
                // we set our loop setting
                audioSource.loop = false;
                // we start playing the sound
                audioSource.Play();
                // we destroy the host after the clip has played
                Destroy(temporaryAudioHost, sfx.length);
            }

            if (PlayMethod == PlayMethods.Cached)
            {
                float volume = Random.Range(MinVolume, MaxVolume);
                float pitch = Random.Range(MinPitch, MaxPitch);
                // we set that audio source clip to the one in paramaters
                _cachedAudioSource.clip = sfx;
                // we set the audio source volume to the one in parameters
                _cachedAudioSource.volume = volume;
                _cachedAudioSource.pitch = pitch;
                // we set our loop setting
                _cachedAudioSource.loop = false;
                // we start playing the sound
                _cachedAudioSource.Play();
            }
        }
    }
}
