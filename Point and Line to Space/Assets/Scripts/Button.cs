using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {

    //bool isMenuDisplay =true;
    public  string SelectMenuName;

    public void BackTitle(){
        GameObject.Find("Canvas").transform.Find("Menu").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("Title").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("TapScreenToStart").gameObject.SetActive(true);
    }

    public void MenuDisplay(){

        GameObject.Find("MenuPanel").transform.Find(SelectMenuName).gameObject.SetActive(true);
        /*if(isMenuDisplay){
            print("Success");
            isMenuDisplay = false;
            GameObject.Find("MenuPanel").transform.Find(SelectMenuName).gameObject.SetActive(true);
        }
        else{
            print("Error");
        }*/

    }


    public void Return(){
        //isMenuDisplay = true;
        this.gameObject.SetActive(false);
        /*if(isMenuDisplay){
            print("isMenueDispalay is true");
        }
        else{
            print("isMenueDispalay is false");
        }*/
    }

}
