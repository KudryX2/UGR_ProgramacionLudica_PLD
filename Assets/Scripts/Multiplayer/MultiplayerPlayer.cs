using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using Mirror;

using System.IO;                        // StreamReader
using UnityEditor;                      // AssetDatabase


public class MultiplayerPlayer : NetworkBehaviour
{
    private GameObject playerCamera;             // Camara del jugador
    private GameObject menuCamera;               // Camara del menu

    float starshipSpeed = 5f;                       // Velocidad de la nave, mas lento en el cliente

    public GameObject[] starshipPartsPrefabs;

    bool W, A, S, D;

    float x,y;

    // Start is called before the first frame update
    [Client]
    void Start()
    {
        // menuCamera = GameObject.Find("MenuCamera");
        // menuCamera.SetActive(false);                            // Desactivamos la camara del menu

        // playerCamera = transform.Find("Camera").gameObject;
        // playerCamera.SetActive(true);                       // Activamos la camara del jugador
        W = A = S = D = false;

        x = transform.position.x;
        y = transform.position.y;

        loadPlayerShip("goli");                             // Carga la nave del jugador

    }

    // Update is called once per frame
    [Client]
    void Update()
    {

        if(!hasAuthority){ return; }    // Si es el cliente adecuado seguimos

        checkKeyboard();                // Comprobamos las teclas pulsadas
    }

    [Client]
    void LateUpdate(){

        if(!hasAuthority){ return; }    // Si es el cliente adecuado seguimos

         if(W)
            y += Time.deltaTime * starshipSpeed;

        if(A)    
            x -= Time.deltaTime * starshipSpeed;

        if(S)    
            y -= Time.deltaTime * starshipSpeed;
        
        if(D)
            x += Time.deltaTime * starshipSpeed;

        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);   // Posicion del raton 


        CmdMove(x, y, worldPoint);
    }

    /*
        Este código no corre en el mismo ambito que la clase por lo tanto las variables 
        de las que depende el movimiento se pasan por parámetro 
    */
    [Command]
    private void CmdMove(float x, float y, Vector3 worldPoint){
        // Add Validate logic
    
        RpcMove(x, y, worldPoint);
    }

    [ClientRpc]
    private void RpcMove(float x, float y, Vector3 worldPoint){
        // transform.Translate(new Vector2(x,y));
        transform.position = new Vector2(x,y);
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(worldPoint.y - transform.position.y, worldPoint.x - transform.position.x) * Mathf.Rad2Deg - 0);
   
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
        Realiza el movimiento de la nave segun las teclas pulsadas
    */
    // void updateMovement(){

    //     // float x = transform.position.x;
    //     // float y = transform.position.y;
    //     // float z = transform.position.z;

    //     float x = 0;
    //     float y = 0;

    //     if(W)
    //         y += Time.deltaTime * starshipSpeed;

    //     if(A)    
    //         x -= Time.deltaTime * starshipSpeed;

    //     if(S)    
    //         y -= Time.deltaTime * starshipSpeed;
        
    //     if(D)
    //         x += Time.deltaTime * starshipSpeed;

    //     // transform.position = new Vector3(x,y,z);
    //     transform.Translate(new Vector2(x,y));
    // }



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
            GameObject originalPart = null;                                         // Pieza a partir de la que hacer la copia
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

}
