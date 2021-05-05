using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;                                    // StreamReader
using UnityEngine.UI;

using System;                                       // Parse string to int



public class GameController : MonoBehaviour
{
    public bool gamePaused;                         // Indica si el juego esta pausado o no

    GameObject player;                              // Referencia al jugador
    GameObject alienManager;                        // Referencia al gestor de alienigenas
    GameObject projectiles;                         // Referencia al contenedor que almacena los proyectiles

    GameObject pauseButton;                         // Referencia al boton de pausa
    GameObject pauseMenu;                           // Referencia al menu de pausa

    public int points;                              // Puntos del jugador
    Text pointsText;                                // Texto UI que contiene los puntos del jugador

    // Start is called before the first frame update
    void Start()
    {
        gamePaused = false;
        
        player = GameObject.Find("PlayerSpaceship");
        alienManager = GameObject.Find("AlienManager");
        projectiles = GameObject.Find("Projectiles");

        pauseButton = GameObject.Find("buttonPause");
        
        pauseMenu = GameObject.Find("pauseMenu");
        pauseMenu.SetActive(false);                 // Por defecto el menu de pausa no se ve

        points = 0;
        pointsText = GameObject.Find("PlayerPoints").GetComponent<Text>();      

        loadGame();                                 // Cargamos el juego
    }

    // Update is called once per frame
    void Update()
    {
    
        if(Input.GetKeyDown("p")){                  // Si pulsamos la tecla P el juego entra o sale de la pausa
            pauseGame();
        }

    }

    /*  
        Se llama para poner poner la pausa o quitarla en SINGLEPLAYER
        Se llama desde el botón "Continue" y el botón "Pause"
    */
    public void pauseGame(){
        showHideMenu();

        player.transform.SendMessage("pause");                          // Pausamos el movimiento del jugador
        alienManager.transform.SendMessage("pause");                    // Pausamos el movimiento de los alienigenas
        foreach (Transform projectile in projectiles.transform)         // Pausamos el movimiento de los proyectiles
        {
            projectile.transform.SendMessage("pause");
        }
    }

    /*
        Se llama para mostrar o ocultar el menu en el MULTIPLAYER
        Se llama desde el botón "Continue" y el botón "Pause"
    */
    public void showMenu(){
        showHideMenu();
    }

    private void showHideMenu(){
        if(gamePaused){
            gamePaused = false;
            pauseButton.SetActive(true);
            pauseMenu.SetActive(false);
        }else{
            gamePaused = true;
            pauseButton.SetActive(false);
            pauseMenu.SetActive(true);
        }
    }


    /*
        Da puntos al jugador y actualiza el texto que aparece en la pantalla
        Se llama desde AlienManager.cs alienDied
    */
    public void givePointsToPlayer(int points){
        this.points += points;
        pointsText.text = "Points : " + this.points;
    }

    /*
        Guarda la partida
        Se llama al salir del singleplayer al pulsar "volver al menu principal" o "salir del juego"
    */
    public void saveGame(){
        Debug.Log("Saving ...");

        StreamWriter writer = new StreamWriter("Assets/Saves/GameSave/partida.txt", false);    	// Escribimos con truncate (false)

        writer.WriteLine(points);

        writer.Close();
    }


    /*
        Carga la partida
        Se llama desde el metodo Start de GameController
    */
    public void loadGame(){

        string line;

        // Puntos del jugador
        StreamReader reader = new StreamReader("Assets/Saves/GameSave/partida.txt"); 
        line = reader.ReadLine();
        
        pointsText.text = "Points : " + line;
        points = Int16.Parse(line);

    }

}
