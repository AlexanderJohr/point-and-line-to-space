using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Point and Line to Space/BackgroundAudioMixerControl")]
public class BackgroundAudioMixerControl : ScriptableObject {
    public AudioMixerSnapshot snapshotMenu;
    public AudioMixerSnapshot snapshot1;
    public AudioMixerSnapshot snapshot2;
    public AudioMixerSnapshot snapshot3;
    public AudioMixerSnapshot snapshot4;
    
    public void TransitionTo(int track)
    {
        switch (track)
        {
            case 0:
                snapshotMenu.TransitionTo(.01f);
                break;
            case 1:
                snapshot1.TransitionTo(.01f);
                break;
            case 2:
                snapshot2.TransitionTo(.01f);
                break;
            case 3:
                snapshot3.TransitionTo(.01f);
                break;
            case 4:
                snapshot4.TransitionTo(.01f);
                break;
        }
    }

}
