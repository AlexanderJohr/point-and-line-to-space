using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	void Awake () {
        SoundManager[] existingMusicManagers =  GameObject.FindObjectsOfType<SoundManager>();
        if (existingMusicManagers.Length > 1) {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    
}
