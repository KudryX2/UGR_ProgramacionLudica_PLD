using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;                        // StreamReader
using UnityEditor;                      // AssetDatabase


using UnityEngine.SceneManagement;      // Load Scene


/*
    Clase que gestiona la nave del jugador [SINGLEPLAYER]
*/
public class Player : MonoBehaviour
{
    public string starshipName;                             // Nombre de la nave (archivo que la almacena)
    
    float starshipSpeed = 30.0f;                            // Velocidad de la nave

    public GameObject[] starshipPartsPrefabs;               // Bloques de la nave (prefab)

    bool W,A,S,D;                                           // Teclas de movimiento
    bool leftMouseIsDown, rightMouseIsDown;                 // Boton izquierdo/derecho esta pulsado

    public Camera playerCamera;                             // Camara que sigue a la nave

    public bool isPaused;                                   // El juego esta o no pausado



    void Start()
    {
        starshipName = "goli";

        W = A = S = D = leftMouseIsDown = rightMouseIsDown = false;

        loadPlayerShip(starshipName);                       // Carga la nave del jugador
    
        isPaused = false;                                   // Por defecto el jugador no esta pausado
    }

    void Update()
    {
        checkMouse();                                   // Comprobamos los botones del raton 
        checkKeyboard();                                // Comprobamos las teclas pulsadas

        if(!isPaused){                                           

            if(leftMouseIsDown){
                shoot();
            }

            if(rightMouseIsDown){
                shoot();
            }
        }

    }

    void LateUpdate(){

        if(!isPaused){
            updateMovement();                               // Actualizamos la posicion de la nave (teclado)
            updateRotation();                               // Actualizamos la rotacion de la nave (raton)
            updateCameraPosition();                         // Actualizamos la posicion de la camara cobre la nave
        }
    }


    /*
        La nave apunta a donde mire el raton
    */
    void updateRotation(){
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);   // Posicion del raton 
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(worldPoint.y - transform.position.y, worldPoint.x - transform.position.x) * Mathf.Rad2Deg - 0);
    }

    /*
        Realiza el movimiento de la nave segun las teclas pulsadas
    */
    void updateMovement(){

        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;

        if(W)
            y += Time.deltaTime * starshipSpeed;

        if(A)    
            x -= Time.deltaTime * starshipSpeed;

        if(S)    
            y -= Time.deltaTime * starshipSpeed;
        
        if(D)
            x += Time.deltaTime * starshipSpeed;

        transform.position = new Vector3(x,y,z);

    }

    /*
        Detecta cuando se pulsa y cuando se suelta una tecla
    */
    void checkKeyboard(){
        // W
        if(Input.GetKeyDown("w")){
            W = true;
        }

        if(Input.GetKeyUp("w")){
            W = false;
        }

        // A
        if(Input.GetKeyDown("a")){
            A = true;
        }

        if(Input.GetKeyUp("a")){
            A = false;
        }

        // S
        if(Input.GetKeyDown("s")){
            S = true;
        }

        if(Input.GetKeyUp("s")){
            S = false;
        }

        // D
        if(Input.GetKeyDown("d")){
            D = true;
        }

        if(Input.GetKeyUp("d")){
            D = false;
        }
    }

    /*
        Comprueba si los botones del raton estan pulsados o no
    */
    void checkMouse(){
        
        // Boton izquierdo
        if(Input.GetMouseButtonDown(0)){
            leftMouseIsDown = true;
        }

        if(Input.GetMouseButtonUp(0)){
            leftMouseIsDown = false;
        }

        // Boton derecho 
        if(Input.GetMouseButtonDown(1)){
            rightMouseIsDown = true;
        }

        if(Input.GetMouseButtonUp(1)){
            rightMouseIsDown = false;
        }
    }

    /*
        Acualiza la posicion de la camara que sigue a la nave
    */
    void updateCameraPosition(){
        playerCamera.transform.position = transform.position;
    }

    /*
        Carga en la escena la nave del jugador, recibe el nombre de la nave como parametro
    */
    void loadPlayerShip(string shipName){

        string path = "Assets/Saves/" + shipName + ".txt";
        string  line;                                                               // Linea leida
        string  id,                                                                 // Identificador de la pieza
                partKind;                                                           // Tipo de pieza
        float   x, y,                                                               // Posicion de la pieza
                rotation = 0;                                                       // Rotacion de la pieza

        // Leer fichero y crear las partes
		StreamReader reader = new StreamReader(path); 
	
		while((line = reader.ReadLine()) != null){
			// Obtener los datos y convertirlos 
			string[] parsedLine = line.Split(' ');
            partKind = parsedLine[0];
			id = parsedLine[1];
			x = float.Parse(parsedLine[2]);
			y = float.Parse(parsedLine[3]);

            if(partKind == "LASER")
                rotation = float.Parse(parsedLine[4]);                              // Si es un laser captamos la rotacion

            // Busca el prefab de la pieza original   
            GameObject originalPart = null;                                             // Pieza a partir de la que hacer la copia
            foreach(GameObject part in starshipPartsPrefabs){
                if(part.name == id){
                    originalPart = part;
                    break;
                }
            }

			// Crear instancia del prefab original 
			var newPart = Instantiate(originalPart, new Vector2(x,y), Quaternion.Euler(0, 0, rotation));    // Crear copia
            newPart.name = originalPart.name;                                       // Poner nombre al objeto (quita el (Clone))
            newPart.layer = 8;                                                      // Capa de bloques de nave de jugador

            // Establecer padre
            if(partKind == "LASER"){
                newPart.SendMessage("disablePointer");                              // Deshabilita el puntero laser
                newPart.transform.SetParent(transform.GetChild(0));                 // Si es un laser -> gadgets
            }else if(partKind == "BLOCK"){
			    newPart.transform.SetParent(transform.GetChild(1));                 // Si es un bloque -> bloques
            }
		}

        reader.Close();
    }

    /*
        Disparar 
    */
    void shoot(){
        foreach (Transform weapon in transform.GetChild(0).transform)
        {
            weapon.transform.GetComponent<Laser>().shoot("player");                  // Cada arma va a disparar;
        }
    }


    /*  Pausa al jugador
        Se llama desde GameController.cs , pauseGame()
    */
    public void pause(){
        if(isPaused)
            isPaused = false;
        else
            isPaused = true;
    }


    /*  Jugador muere
        Se envia por mensaje desde StarshipPart.cs cuando la parte es destruida 
        La nave reaparece en el spawn 
    */
    public void die(){

        // Destruimos los gadgets de la nave
        // foreach(Transform part in transform.GetChild(0).transform){
        //     Destroy(part.gameObject);
        // }

        // // Destruimos los bloques de la nave
        // foreach(Transform part in transform.GetChild(1).transform){
        //     Destroy(part.gameObject);
        // }

        // Reiniciamos la posición
        // transform.position = new Vector3(0,0,0);
        // updateCameraPosition();

        // Volvemos a cargar la nave del jugador
        // loadPlayerShip(starshipName);

        // Recarga el nivel tmp
        SceneManager.LoadScene(Application.loadedLevel);
    }

}
