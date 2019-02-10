using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

	void Awake () {
        MusicManager[] existingMusicManagers =  GameObject.FindObjectsOfType<MusicManager>();
        if (existingMusicManagers.Length > 1) {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    
}
