using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public List<AudioClip> sounds;
    public AudioSource source;
    public static SoundManager instance;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound(string clipName)
    {
        source.PlayOneShot(FindClip(clipName));
    }

    private AudioClip FindClip(string name)
    {
        foreach (var sound in sounds)
        {
            if (sound.name == name)
            {
                return sound;
            }
        }

        Debug.LogError("sound " + name + " not found");
        throw new KeyNotFoundException("sound " + name + " not found");
    }
}
