using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{
    public AudioMixerSnapshot mainMenuSnap, woodsWithoutRainSnap, woodsWithRainSnap, cavesSnap, hideoutWithoutRainSnap, hideoutWithRainSnap, templeSnap;
    public AudioSource mainMenuTheme, woodsTheme, cavesTheme, rain, hideoutTheme, templeTheme;
    public float transitionTime = 10f;
    public Dictionary<int, AudioMixerSnapshot> snapshots = new();
    public AudioSource hoverSound, clickSound;
    public AudioSource bloodHit;
    public AudioSource encounterSound, transtionSound;

    void Start()
    {
        mainMenuTheme.Play();
        snapshots.Add(0, mainMenuSnap);
        snapshots.Add(1, woodsWithoutRainSnap);
        snapshots.Add(2, woodsWithRainSnap);
        snapshots.Add(3, woodsWithRainSnap);
        snapshots.Add(4, woodsWithRainSnap);
        snapshots.Add(5, woodsWithRainSnap);
        snapshots.Add(6, woodsWithRainSnap);
        snapshots.Add(7, woodsWithoutRainSnap);
        snapshots.Add(8, cavesSnap);
        snapshots.Add(9, cavesSnap);
        snapshots.Add(10, cavesSnap);
        snapshots.Add(11, cavesSnap);
        snapshots.Add(12, cavesSnap);
        snapshots.Add(13, cavesSnap);
        snapshots.Add(14, cavesSnap);
        snapshots.Add(15, hideoutWithRainSnap);
        snapshots.Add(16, hideoutWithRainSnap);
        snapshots.Add(17, hideoutWithoutRainSnap);
        snapshots.Add(18, hideoutWithoutRainSnap);
        snapshots.Add(19, hideoutWithRainSnap);
        snapshots.Add(20, hideoutWithoutRainSnap);
        snapshots.Add(21, hideoutWithRainSnap);
        snapshots.Add(22, templeSnap);
        snapshots.Add(23, templeSnap);
    }

    public void ChangeLevel(int levelId)
    {
        snapshots[levelId].TransitionTo(transitionTime);
        if (snapshots[levelId] == mainMenuSnap)
        {
            StartCoroutine(WaitAndPlayTrack(mainMenuTheme));
            StartCoroutine(WaitAndStopTrack(woodsTheme));
            StartCoroutine(WaitAndStopTrack(cavesTheme));
            StartCoroutine(WaitAndStopTrack(rain));
            StartCoroutine(WaitAndStopTrack(templeTheme));
            StartCoroutine(WaitAndStopTrack(hideoutTheme));
        }
        else if (snapshots[levelId] == woodsWithoutRainSnap)
        {
            StartCoroutine(WaitAndPlayTrack(woodsTheme));
            StartCoroutine(WaitAndStopTrack(cavesTheme));
            StartCoroutine(WaitAndStopTrack(rain));
            StartCoroutine(WaitAndStopTrack(mainMenuTheme));
            StartCoroutine(WaitAndStopTrack(templeTheme));
            StartCoroutine(WaitAndStopTrack(hideoutTheme));
        }
        else if (snapshots[levelId] == woodsWithRainSnap)
        {
            StartCoroutine(WaitAndPlayTrack(woodsTheme));
            StartCoroutine(WaitAndPlayTrack(rain));
            StartCoroutine(WaitAndStopTrack(cavesTheme));
            StartCoroutine(WaitAndStopTrack(mainMenuTheme));
            StartCoroutine(WaitAndStopTrack(templeTheme));
            StartCoroutine(WaitAndStopTrack(hideoutTheme));
        }
        else if (snapshots[levelId] == cavesSnap)
        {
            StartCoroutine(WaitAndPlayTrack(cavesTheme));
            StartCoroutine(WaitAndStopTrack(woodsTheme));
            StartCoroutine(WaitAndStopTrack(rain));
            StartCoroutine(WaitAndStopTrack(mainMenuTheme));
            StartCoroutine(WaitAndStopTrack(templeTheme));
            StartCoroutine(WaitAndStopTrack(hideoutTheme));
        }
        else if (snapshots[levelId] == templeSnap)
        {
            StartCoroutine(WaitAndPlayTrack(templeTheme));
            StartCoroutine(WaitAndStopTrack(woodsTheme));
            StartCoroutine(WaitAndStopTrack(rain));
            StartCoroutine(WaitAndStopTrack(mainMenuTheme));
            StartCoroutine(WaitAndStopTrack(cavesTheme));
            StartCoroutine(WaitAndStopTrack(hideoutTheme));
        }
        else if (snapshots[levelId] == hideoutWithoutRainSnap)
        {
            StartCoroutine(WaitAndPlayTrack(hideoutTheme));
            StartCoroutine(WaitAndStopTrack(woodsTheme));
            StartCoroutine(WaitAndStopTrack(rain));
            StartCoroutine(WaitAndStopTrack(mainMenuTheme));
            StartCoroutine(WaitAndStopTrack(cavesTheme));
            StartCoroutine(WaitAndStopTrack(templeTheme));
        }
        else if (snapshots[levelId] == hideoutWithRainSnap)
        {
            StartCoroutine(WaitAndPlayTrack(hideoutTheme));
            StartCoroutine(WaitAndPlayTrack(rain));
            StartCoroutine(WaitAndStopTrack(cavesTheme));
            StartCoroutine(WaitAndStopTrack(mainMenuTheme));
            StartCoroutine(WaitAndStopTrack(templeTheme));
            StartCoroutine(WaitAndStopTrack(woodsTheme));
        }
    }

    private void PlayDontDisrupt(AudioSource track)
    {
        if (!track.isPlaying)
            track.Play();
    }

    private IEnumerator WaitAndPlayTrack(AudioSource track)
    {
        yield return new WaitForSeconds(transitionTime / 1.3f);
        PlayDontDisrupt(track);
    }

    private IEnumerator WaitAndStopTrack(AudioSource track)
    {
        yield return new WaitForSeconds(transitionTime);
        track.Stop();
    }

    public void BloodHit()
    {
        bloodHit.Play();
    }

    public void ClickSound()
    {
        clickSound.Play();
    }

    public void TransitionSound()
    {
        transtionSound.Play();
    }

    public void EncounterSound()
    {
        encounterSound.Play();
    }
}
