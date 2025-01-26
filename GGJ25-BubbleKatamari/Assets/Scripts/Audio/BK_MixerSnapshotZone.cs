using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BK_MixerSnapshotZone : MonoBehaviour
{
    // The snapshot to switch to when we enter the zone
    [SerializeField] private AudioMixerSnapshot enterZoneSnapshot;

    // The snapshot to switch to when we exit the zone
    [SerializeField] private AudioMixerSnapshot exitZoneSnapshot;

    // How much time to spend transitioning to the new snapshot
    [SerializeField] private float transitionTime = 0.25f;

    // The mixer whose volume we'll be changing
    [SerializeField] private AudioMixer masterMixer;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SwitchAudioMixerSnapshot(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SwitchAudioMixerSnapshot(false);
        }
    }

    private void SwitchAudioMixerSnapshot(bool didEnter)
    {
        if (didEnter)
        {
            enterZoneSnapshot.TransitionTo(transitionTime);
            //masterMixer.SetFloat("BGMVolume", -3f);
        }
        else
        {
            exitZoneSnapshot.TransitionTo(transitionTime);
            //masterMixer.SetFloat("BGMVolume", 0f);
        }

        // NOTE: You can also use AudioMixer.TransitionToSnapshots(...)
        // to transition to more than 1 snapshot and blend
        // between them with a given set of weights.
    }
}