using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _music;
    [SerializeField] private AudioClip[] _clips;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }

    //public void PlayMusic()
    //{
    //    _source.clip = _music;
    //    _source.loop = true;
    //    _source.volume = 0.2f;
    //    _source.Play();
    //}

    public void Play(string soundName)
    {
        var clip = GetClip(soundName);
        if (clip != null)
        {
            //_source.clip = clip;
            _source.loop = false;
            _source.volume = 1;
            _source.PlayOneShot(clip);
        }
    }

    private AudioClip GetClip(string name)
    {
        for (int i = 0; i < _clips.Length; i++)
        {
            if (_clips[i].name.Contains(name))
                return _clips[i];
        }

        return null;
    }
}
