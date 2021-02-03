using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;                      // AssetDatabase


public class AlienManager : MonoBehaviour
{
    public GameObject alien;            // Alien (prefab)

    float   spawnRate,                  // Frecuencia de spawneo 
            lastSpawn;                  // Ultimo spawneo

    int     maxSpawned,                 // Cantidad maxima de aliens spawneados
            aliensAlive;                // Aliens vivos actualmente

    public GameObject gameController;   // Controlador del juego 


    // Start is called before the first frame update
    void Start()
    {

        spawnRate = 1;

        maxSpawned = 1;
        aliensAlive = 0;

        gameController = GameObject.Find("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        /*
            Spanear aliens cada "spawnRate" si hay menos que el limite            
        */
        if(aliensAlive < maxSpawned && Time.time - lastSpawn >= spawnRate){
            spawnAlien(new Vector2(Random.Range(-50,50),Random.Range(-50,50)), "Alien1");                           // Spawneamos un alien
            lastSpawn = Time.time;                                              // Actualizamos "lastSpawn"
            aliensAlive++;                                                      // Incrementamos contador de aliens vivos
        }
    }

    /*
        Crea un alien en una posicion dada con el nombre dado
    */
    void spawnAlien(Vector2 position, string name){
        GameObject newAlien = Instantiate(alien, position, Quaternion.Euler(0, 0, 0)) as GameObject;   // Crear copia
        newAlien.name = alien.name;                                                                    // Poner nombre al objeto (quita el (Clone))
        newAlien.transform.SetParent(transform);                                                       // Establecer padre
    }

    /*
        Alien avisa de que ha muerto, se llama desde "Alien.cs"
    */
    public void alienDied(){
        if(aliensAlive > 0){
            aliensAlive--;
            gameController.SendMessage("givePointsToPlayer", 10);
        }
    }

    /*
        Se llama desde GameController.cs al pausarse el juego
    */
    void pause(){

        foreach (Transform alien in transform)
        {
            alien.transform.SendMessage("pause");
        }
    }
}
