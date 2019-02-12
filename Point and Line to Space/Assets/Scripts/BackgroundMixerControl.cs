using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BackgroundMixerControl : MonoBehaviour
{
    public AudioMixerSnapshot snapshotMenu;
    
    void Start()
    {
        snapshotMenu.TransitionTo(0.0f);
    }
    
}
