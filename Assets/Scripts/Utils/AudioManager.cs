using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
	public AudioMixerGroup mixerGroup;
	[SerializeField] private CustomSound[] sounds;

    private void Awake()
	{
		foreach (CustomSound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;

			s.source.outputAudioMixerGroup = mixerGroup;
		}
	}

	public static void PlaySound(string sound)
    {
        if (Instance == null || Instance.sounds == null)
            return;
        CustomSound s = Array.Find(Instance.sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + sound + " not found!");
			return;
		}

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		s.source.Play();
	}

	public static void StopSound(string sound)
    {
		if (Instance == null || Instance.sounds == null)
			return;
        CustomSound s = Array.Find(Instance.sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + sound + " not found!");
            return;
        }
		if (s.source.isPlaying)
			s.source.Stop();
    }
}
