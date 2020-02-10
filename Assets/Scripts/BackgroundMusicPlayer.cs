using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicPlayer : MonoBehaviour {

	public AudioClip backgroundMusicClip;
	AudioSource radio;

	public static BackgroundMusicPlayer instance;

	void Awake(){
		instance = this;

        radio = GetComponent<AudioSource>();
        
        try
        {

            radio.loop = true;
            radio.clip = backgroundMusicClip;
            radio.Play();

        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }
	}

    public AudioClip ballHitWall, ballDie, ballWin;

    public void PlayBallHitWall()
    {

        GetComponentInChildren<AudioSource>().PlayOneShot(ballHitWall);

    }

    public void PlayBallDie()
    {

        GetComponentInChildren<AudioSource>().PlayOneShot(ballDie);

    }

    public void PlayBallWin()
    {
        GetComponentInChildren<AudioSource>().PlayOneShot(ballWin);

    }

}
