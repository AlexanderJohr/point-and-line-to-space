using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {
    public  string MenuSelect;
    public void MenuDisplay(){
        GameObject.Find("Menu").transform.Find(MenuSelect).gameObject.SetActive(true);
        GameObject.Find("Menu").transform.Find("Buttons").gameObject.SetActive(false);
    }
    public void MenuDelete(){
        this.gameObject.SetActive(false);
        GameObject.Find("Menu").transform.Find("Buttons").gameObject.SetActive(true);
    }

}
