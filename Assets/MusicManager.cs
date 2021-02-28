using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;
using Gardarike;

public class MusicManager : MonoBehaviour
{
    public AudioClip ingameBGM;

    // Start is called before the first frame update
    void Start()
    {
        EventBus.instance.onLoginComplete += ChangeToIngameBGM;
    }

    private void ChangeToIngameBGM(string sessionID, RepeatedField<Character> characters)
    {
        this.GetComponent<AudioSource>().Stop();
        this.GetComponent<AudioSource>().clip = ingameBGM;
        this.GetComponent<AudioSource>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
