using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;                        // StreamReader

/*
    Clase que gestiona la nave de un alien
*/
public class Alien : MonoBehaviour
{

    GameObject playerSpaceship;                         // Referencia a la nave del jugador
    public GameObject[] starshipPartsPrefabs;           

    static float SPEED = 3f;

    public bool isPaused;                               // El juego esta o no pausado

    // Start is called before the first frame update
    void Start()
    {

        playerSpaceship = GameObject.Find("PlayerSpaceship");

        isPaused = false;                                               // Por defecto no esta en pausa

        /*
            Cargamos la nave del alien en la posicion del objeto , con el nombre del objeto
            ambos parametros se definen en "AlienManager" al instanciar un objeto de esta clase.
        */                         
        loadAlienShip(transform.position, transform.name);     
    }

    // Update is called once per frame
    void Update()
    {

        if(!isPaused){
            var distanceToPlayer = distance(transform.position.x, transform.position.y, playerSpaceship.transform.position.x, playerSpaceship.transform.position.y );
            
            if(distanceToPlayer < 100 && distanceToPlayer > 15){    
                followPlayer();
            }

            if(distanceToPlayer < 100){
                shootPlayer();
            }
        }
        
                transform.position = new Vector3(transform.position.x, transform.position.y, 0);


    }

    /*
        Carga en la escena la nave del alien, recibe la posicion donde se va a vargar y el nombre del archivo como parametro
    */
    void loadAlienShip(Vector2 worldPosition, string shipName){

        string path = "Assets/Saves/" + shipName + ".txt";
        string  line;                                                               // Linea leida
        
        string  partKind,                                                           // Tipo de pieza
                id;                                                                 // Identificador de la pieza
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
            rotation = 0;

            if(partKind == "LASER"){
                rotation = float.Parse(parsedLine[4]);
            }

            // Busca el prefab de la pieza original
            GameObject originalPart = null;                                          // Pieza a partir de la que hacer la copia
            foreach(GameObject part in starshipPartsPrefabs){
                if(part.name == id){
                    originalPart = part;
                    break;
                }
            }

			// Crear instancia del prefab original 
			var newPart = Instantiate(originalPart, new Vector3(worldPosition.x + x, worldPosition.y + y, 0), Quaternion.Euler(0, 0, rotation));
            newPart.name = originalPart.name;                                       // Poner nombre al objeto (quita el (Clone))
            newPart.layer = 9;                                                      // Layer de parte de nave de un alien

            // Establecer padre
            if(partKind == "LASER"){
                newPart.SendMessage("disablePointer");                              // Deshabilita el puntero laser
                newPart.transform.SetParent(transform.GetChild(0));                 // Si es un laser -> Gadgets
            }else{
			    newPart.transform.SetParent(transform.GetChild(1));                 // Si es un bloque -> Blocks
            }
		}

        reader.Close();
    }

    /*
        Se llama cuando se destruye el nucleo del alien, se llama desde "Block.cs"
    */
    public void die(){
        GameObject.Find("AlienManager").transform.SendMessage("alienDied");     // Avisar a "AlienManager" de que un alien ha muerto
        Destroy(this.gameObject);
    }


    /*
        Accion de seguir al jugador
    */
    private void followPlayer(){
        var distanceToPlayer = distance(transform.position.x, transform.position.y, playerSpaceship.transform.position.x, playerSpaceship.transform.position.y );

        transform.Translate((playerSpaceship.transform.position - transform.position)/distanceToPlayer * SPEED * Time.smoothDeltaTime);
    }

    /*
        Accion de disparar al jugador
    */
    private void shootPlayer(){

        foreach (Transform laser in transform.GetChild(0).transform){
            // Calculamos la rotación a la que se tiene que girar el alien
            var finalRotation =  Mathf.Atan2(laser.position.y - playerSpaceship.transform.position.y, laser.position.x - playerSpaceship.transform.position.x) * Mathf.Rad2Deg;
            // Rotar el cañon
            laser.rotation = Quaternion.RotateTowards(laser.rotation, Quaternion.Euler(0,0,finalRotation - 180), 25 * Time.deltaTime);
            // Disparar
            laser.transform.GetComponent<Laser>().shoot("alien");
        }

    }

    /*
        Devuelve la distancia entre dos puntos 
    */
    public float distance(float x1, float y1, float x2, float y2){
        return Mathf.Sqrt(Mathf.Pow(x1-x2,2) + Mathf.Pow(y1-y2,2));
    }

    /*
        Pausa el juego , se llama desde AlienManager.cs
    */
    public void pause(){
        if(isPaused)
            isPaused = false;
        else    
            isPaused = true;
    }

}
