using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource _AudioSource = null;
    public AudioSource Jump = null;
    public AudioSource Step = null;
    public AudioClip Dead = null;
    public AudioClip Score1 = null;
    public AudioClip Score2 = null;
    public AudioClip Win = null;
    private int score = 0;
    public void PlayWin()
    {
        _AudioSource.clip = Win;
        _AudioSource.Play();
    }

    public void PlayScore()
    {
        if (score>3)
        {
            _AudioSource.clip = Score2;
            score = 0;
        }
        else
        {
            _AudioSource.clip = Score1;
            score++;
        }
        _AudioSource.Play();
    }

    public void PlayDead()
    {
        Step.Stop();
        Jump.Stop();
        _AudioSource.clip = Dead;
        _AudioSource.Play();
    }

    public void PlayJump()
    {
        Step.Stop();
        Jump.Play();
    }

    public void PlayStep(bool v)
    {
        if (v)
        {
            if (Step.isPlaying)
            {
                return;
            }
            Step.Play();
        }
        else
        {
            Step.Stop();
        }

    }
}