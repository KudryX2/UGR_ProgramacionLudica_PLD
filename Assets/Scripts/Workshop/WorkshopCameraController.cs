using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkshopCameraController : MonoBehaviour
{
    static int SPEED = 10;              // Velocidad de la camara
    bool W,A,S,D;                       // Movimiento
    bool Q,E;                           // Zoom 
    Camera camera;                      // Componente camara

    // Start is called before the first frame update
    void Start()
    {
        W = A = S = D = Q = E = false;
        camera = transform.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

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

        // Q
        if(Input.GetKeyDown("q")){
            Q = true;
        }

        if(Input.GetKeyUp("q")){
            Q = false;
        }

        // E
        if(Input.GetKeyDown("e")){
            E = true;
        }

        if(Input.GetKeyUp("e")){
            E = false;
        }

    }

    void LateUpdate(){
        // Mover camara
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;

        if(W)
            y += Time.deltaTime * SPEED;

        if(A)    
            x -= Time.deltaTime * SPEED;

        if(S)    
            y -= Time.deltaTime * SPEED;
        
        if(D)
            x += Time.deltaTime * SPEED;

        transform.position = new Vector3(x,y,z);

        // Zoom
        if(Q && camera.orthographicSize < 20){
            camera.orthographicSize += Time.deltaTime * 5;
        }

        if(E && camera.orthographicSize > 5){
            camera.orthographicSize -= Time.deltaTime * 5;
        }
    }
}
