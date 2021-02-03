using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneSwitcher : MonoBehaviour
{

    public void loadMenuScene(){
        SceneManager.LoadScene("menu");
    }

    public void loadSingleplayerScene(){
        SceneManager.LoadScene("singleplayer");
    }

    public void loadMultiplayerScene(){
        SceneManager.LoadScene("multiplayer");
    }

    public void loadWorkshopScene(){
        SceneManager.LoadScene("workshop");  
    }

    public void exitGame(){
        Application.Quit();
    }
}
