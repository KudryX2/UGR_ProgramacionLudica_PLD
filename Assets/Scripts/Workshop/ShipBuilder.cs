using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;                                            // StreamReader
using UnityEngine.UI;
using UnityEngine.EventSystems;


/*
    Clase para el taller de naves
*/
public class ShipBuilder : MonoBehaviour
{
    public GameObject[] starshipPartsPrefabs;               // Prefabs de los bloques de las naves

    const float SIZE = 1.6f;                                // Tamaño del bloque de pieza

    GameObject StarshipBlocks;                              // Almacena los bloques de la nave en la escena
    GameObject StarshipGadgets;                             // Almacena los componentes de la nave en la escena

    Button buttonSave, buttonLoad;                          // Botones 
    bool leftMouseIsDown, rightMouseIsDown;                 // Boton izquierdo/derecho esta pulsado
    bool hoveringButton;                                    // Booleano si el raton esta encima de algun boton

    public GameObject inventorySelectedPart;                // Pieza con la que se va a construir
    GameObject blockPointer;                                // Puntero para construir

    GameObject gadgetToRotate;                              // Gadget cuya rotacion vamos a modificar 
    bool addingGadget = false;                              // Proceso de añadir un gadget (para añadir direccion)
    bool nextClickSetsRotationOfgadget = false;             // Siguiente click deja fija la rotacion del gadget

    void Start()
    {
        StarshipBlocks = GameObject.Find("StarshipBlocks");
        StarshipGadgets = GameObject.Find("StarshipGadgets");

        blockPointer = GameObject.Find("BlockPointer");

        buttonSave = GameObject.Find("buttonSave").GetComponent<Button>();      // Evento pulsar boton guardar
        buttonSave.onClick.AddListener( () => save() ); 

        buttonLoad = GameObject.Find("buttonLoad").GetComponent<Button>();      // Evento pulsar boton cargar
        buttonLoad.onClick.AddListener( () => load() );

        leftMouseIsDown = rightMouseIsDown = false;
    }

    void Update()
    {
        /*
            Leemos la posicion del raton y redondeamos la posicion para obtener la posicion en cuadricula
        */
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);   // Posicion del raton 
        float x = Mathf.Round(worldPoint.x*SIZE)/SIZE;                     			// Redondeo de la posicion (obtener cuadricula)
        float y = Mathf.Round(worldPoint.y*SIZE)/SIZE;
        Vector2 position = new Vector2(x, y);                   					// Posicion 2D del raton

        /*
            Detectamos los botones del raton
        */
        checkMouseButtons();                                                        

        /*
            Comprobamos si hay un bloque colindante
        */        
        bool nextToBlock = getNextToBlock(position);                            

        /*
            Podemos construir si :
                - Tenemos una pieza seleccionada
                - Hay un bloque colindante OR Tenemos seleccionado un nucleo OR estamos añadiendo un gadget
                - No es un gadget (necesita un bloque debajo) y no estamos estableciendo su rotacion
            
            Habilitar/Deshabilitar puntero
        */
        if(inventorySelectedPart != null && 
            (nextToBlock || inventorySelectedPart.name == "StarshipCore") &&
            (inventorySelectedPart.GetComponent<StarshipPart>().PART_KIND != "LASER" && !nextClickSetsRotationOfgadget)
        ){
            blockPointer.GetComponent<SpriteRenderer>().enabled = true;
        }else{
            blockPointer.GetComponent<SpriteRenderer>().enabled = false;
        }
        blockPointer.transform.position = position;                                		// Puntero sigue el raton

        /*
            Acciones boton IZQUIERDO del RATON [ CONSTRUIR BLOQUES Y MODIFICAR GADGETS ]
        */
        if(leftMouseIsDown && !hoveringButton && inventorySelectedPart != null){
            /*
                Añadir pieza
                Si no estamos en proceso de añadir un bloque AND ( hay un bloque colindante OR pieza seleccionada es un "StarshipCore" ) 
                podemos añadir una pieza en dicha posicion
            */
            if(!addingGadget && ( nextToBlock || inventorySelectedPart.name == "StarshipCore")){
                addPart(position);                                                 		
            }
            /*
                Si el siguiente click deja fija la rotacion del gadget salimos del proceso de añadir un gadget
            */
            if(nextClickSetsRotationOfgadget){
                nextClickSetsRotationOfgadget = false;
                addingGadget = false;
            }

        }

        // Si estamos en el proceso de añadir un gadget actualizamos la rotacion de este segun la posicion del raton
        if(addingGadget){
            rotateNewGadget(position);
        }

        // Si estamos rotando el gadget añadido y queremos cancelar la accion pulsamos ESC
        if(Input.GetKeyDown("escape") && nextClickSetsRotationOfgadget){
            Destroy(gadgetToRotate.gameObject);
            addingGadget = false;
            nextClickSetsRotationOfgadget = false;
        }

        // Si estamos en el proceso de añadir un gadget y hemos soltado el raton en el sigueinte click la rotacion se hara fija
        if(!leftMouseIsDown && addingGadget){
            nextClickSetsRotationOfgadget = true;
        }


        /*
            Acciones boton DERECHO del RATON [ ELIMINAR ELEMENTOS ]
        */
        if(rightMouseIsDown){
            removePart(position);                                               	
        }

    
    }

    /*
        Añade una pieza a la nave , recibe la posición del raton 
    */
    void addPart(Vector2 position){

        // Comprobamos de que tipo es el item que tenemos seleccionado en el inventario
        string selectedPartKind = inventorySelectedPart.GetComponent<StarshipPart>().PART_KIND;

        // Laser
        if(selectedPartKind == "LASER"){
            /*
                Para construir un gadget necesitamos que haya un bloque en la posicion dada
                pero no haya ningun otro gadget en dicha posicion 
            */
            if(blockAtPosition(position) && !gadgetAtPosition(position)){
                var newPart = Instantiate(inventorySelectedPart, position, Quaternion.identity); // Crear instancia del prefab
                newPart.name = inventorySelectedPart.name;                                       // Establecer nombre
                newPart.transform.SetParent(StarshipGadgets.transform);                          // Esablecer padre

                // Entramos en el proceso de modificar la rotacion del gadget añadido
                addingGadget = true;
                gadgetToRotate = newPart;       
            }
        }
        // Bloques
        else if(selectedPartKind == "BLOCK"){
            /*
                Para construir un bloque necesitamos que no haya nada en la posicion dada
            */
            if(!blockAtPosition(position)){
                // Instanciamos la pieza nueva y la añadimos a la escena
                var newPart = Instantiate(inventorySelectedPart, position, Quaternion.identity);    // Crear instancia del prefab 
                newPart.name = inventorySelectedPart.name;                                          // Establecer nombre
                newPart.transform.SetParent(StarshipBlocks.transform);                              // Establecer padre 
            }
        }
 
    }

    /*
        Establece una direccion para un gadget cuando este es puesto, recibe la posicion del raton
    */
    void rotateNewGadget(Vector2 position){
        var rotation =  Mathf.Atan2(position.y - gadgetToRotate.transform.position.y, position.x - gadgetToRotate.transform.position.x) * Mathf.Rad2Deg;
        rotation = Mathf.Round(rotation*20)/20;                                         // Redondeamos la rotacion
        gadgetToRotate.transform.rotation = Quaternion.Euler(0, 0, rotation);    
    }

    /*
        Quita una pieza de la nave , recibe la posición del raton 
    */
    void removePart(Vector2 position){
        Transform closerPart = null;                                                    // Objeto mas cercano
        float minDist = -1;                                                             // Distancia minima

        foreach(Transform part in StarshipGadgets.transform){                           // Calculamos el bloque mas cercano 
            float auxDist = distance(part.transform.position.x , part.transform.position.y , position.x, position.y);
            if(auxDist < SIZE/4){                                                       // Se tiene en cuenta solo si esta cerca
                if(minDist == -1 || (minDist != -1 && auxDist < minDist)){              // Si no tenemos ninguna o si hay alguna y es mayor que la nueva
                    minDist = auxDist;
                    closerPart = part;
                }

            }
        }

        // Si no se ha encontrado ningun componente se busca en los bloques
        if(closerPart == null){
            foreach(Transform part in StarshipBlocks.transform){                        // Calculamos el bloque mas cercano 
                float auxDist = distance(part.transform.position.x , part.transform.position.y , position.x, position.y);
                if(auxDist < SIZE/4){                                                   // Se tiene en cuenta solo si esta cerca
                    if(minDist == -1 || (minDist != -1 && auxDist < minDist)){          // Si no tenemos ninguna o si hay alguna y es mayor que la nueva
                        minDist = auxDist;
                        closerPart = part;
                    }

                }
            }

        }
        
        if(closerPart != null){                                                         // Si hemos encontrado la pieza
            Destroy(closerPart.gameObject);                                             // Eliminamos el objeto de la escena
        }

    }

   

    /*
        Guarda 
    */
    public void save(){
        Text name = GameObject.Find("SaveFileName").GetComponent<Text>();
        string path = "Assets/Saves/" + name.text + ".txt";

        StreamWriter writer = new StreamWriter(path, false);    					    // Escribimos con truncate (false)

        /*
            Para cada pieza guardamos : tipo de pieza, nombre, posicion y rotacion (si es un gadget)
        */
        foreach ( Transform part in StarshipBlocks.transform ){
            writer.WriteLine(part.GetComponent<StarshipPart>().PART_KIND + " " + part.name + " " + part.transform.position.x + " " + part.transform.position.y);         
        }

        foreach ( Transform part in StarshipGadgets.transform ){
            writer.WriteLine(part.GetComponent<StarshipPart>().PART_KIND + " " + part.name + " " + part.transform.position.x + " " + part.transform.position.y + " " + part.transform.eulerAngles.z);
        }
        
        writer.Close();
    }

    /*
        Limpia actual y carga
    */
    public void load(){

        Text name = GameObject.Find("LoadFileName").GetComponent<Text>();
        string path = "Assets/Saves/" + name.text + ".txt";
        string  line;                                                               // Linea leida
        string  partKind,                                                           // Tipo de pieza
                id;                                                                 // Identificador de la pieza
        float   x, y,                                                               // Posicion de la pieza
                rotation;                                                           // Rotacion de la pieza

         // Limpiar actual
		foreach (Transform part in StarshipBlocks.transform) {
			GameObject.Destroy(part.gameObject);
		}

        foreach (Transform part in StarshipGadgets.transform) {
			GameObject.Destroy(part.gameObject);
		}

        // Leer fichero y crear las partes
		StreamReader reader = new StreamReader(path); 
	
		while((line = reader.ReadLine()) != null){
			// Obtener los datos y convertirlos 
			string[] parsedLine = line.Split(' ');
            partKind = parsedLine[0]; 
			id = parsedLine[1];
			x = float.Parse(parsedLine[2]);
			y = float.Parse(parsedLine[3]);
            rotation = 0;                                                           // Si es un bloque por defecto dejamos la rotacion a 0

            if(partKind == "LASER"){                                                // Si es un laser extraemos la rotacion
                rotation = float.Parse(parsedLine[4]);
            }

            // Buscamos el prefab de la pieza a construir
            GameObject originalPart = null;                                         // Prefab de la pieza a construir
            foreach(GameObject part in starshipPartsPrefabs){                      
                if(part.name == id){
                    originalPart = part;
                    break;
                }
            }

			// Añadir a la escena las piezas leidas
			Vector2 position = new Vector2(x,y);
			var newPart = Instantiate(originalPart, position, Quaternion.Euler(0, 0, rotation));    // Instanciar el objeto 
            newPart.name = originalPart.name;                                                       // Poner nombre al objeto (quita el (Clone))
            
            // Establecer padre
            if(partKind == "LASER"){
                newPart.transform.SetParent(StarshipGadgets.transform);                             // Si es un laser -> gadgets                
            }else if(partKind == "BLOCK"){  
                newPart.transform.SetParent(StarshipBlocks.transform);                              // Si es un bloque -> bloques        
            }
		}

        GameObject.Find("SaveNameImput").GetComponent<InputField>().text = name.text;               // Añadir el nombre de la nave en el campo de guardado 

        reader.Close();
    }

	/*
		Se llama cuando el puntero entra encima de un boton
	*/
    public void onHoveringButtonEnter(){
        hoveringButton = true;
		blockPointer.GetComponent<SpriteRenderer>().enabled = false;						// Oculta el puntero 
    }

	/*
		Se llama cuando el puntero sale de encima de un boton 
	*/
    public void onHoveringButtonExit(){
        hoveringButton = false;
		blockPointer.GetComponent<SpriteRenderer>().enabled = true;						// Muestra el puntero 
    }

	/*
		Se llama para cambiar la pieza seleccionada a construir
		Se llama desde InventoryManager (script), al seleccionar una celda distinta
		Recibe un GameObject que es un prefab del inventario
	*/
	public void setInventorySelectedPart(GameObject newInventorySelectedPart){
		inventorySelectedPart = newInventorySelectedPart;
	}


    /*
        Comprueba el estado de los botones del raton 
    */
    void checkMouseButtons(){
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
        Devuelve true si hay un bloque en la posicion dada, si no false
    */
    bool blockAtPosition(Vector2 position){
        foreach (Transform part in StarshipBlocks.transform){
            // Si se encuentra con un bloque en la posicion dada devuelve true
            if(part.transform.position.x == position.x && part.transform.position.y == position.y){ 
                return true;            
            }
        }
        return false;
    }

    /*
        Devuelve si hay un bloque colindante a la posicion dada
    */
    bool getNextToBlock(Vector2 position){
        foreach (Transform part in StarshipBlocks.transform){
            // Si encuentra un bloque a una distancia menor que SIZE/4 devuelve true
            if(distance(part.transform.position.x, part.transform.position.y , position.x, position.y) < SIZE/2){
                return true;            
            }
        }
        return false;
    }

    /*
        Devuelve true si hay un gadget en la posicion dada, si no false
    */
    bool gadgetAtPosition(Vector2 position){
        foreach (Transform part in StarshipGadgets.transform){
            // Si se encuentra con un gadget en la posicion dada devuelve true
            if(part.transform.position.x == position.x && part.transform.position.y == position.y){ 
                return true;            
            }
        }
        return false;
    }


    /*
        Devuelve la distancia entre dos puntos 
    */
    public float distance(float x1, float y1, float x2, float y2){
        return Mathf.Sqrt(Mathf.Pow(x1-x2,2) + Mathf.Pow(y1-y2,2));
    }

}



