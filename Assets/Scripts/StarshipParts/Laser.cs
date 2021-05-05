using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;  // Light2D
using UnityEditor;                              // AssetDatabase


public class Laser : MonoBehaviour
{
    public static string PART_KIND = "LASER";

    public GameObject laserShoot;           // Proyectil (prefab)

    double  fireRate,                       // Intervalos entre disparo y disparo 
            lastShoot;                      // Ultimo disparo

    public GameObject shootLight;           // Luz que parpadea al disparar
    double  effectsTime,                    // Tiempo que dura el efecto de la luz
            effectsStart;                   // Momento en el que empezo el efecto 


    public GameObject pointer;              // Puntero hacia donde apunta el laser

    // Start is called before the first frame update
    void Start()
    {
        fireRate = 0.1;
        
        shootLight.SetActive(false);
        effectsTime = 0.02;
    }

    // Update is called once per frame
    void Update()
    {
        /*
            Si ha pasado el "effectsTime" desactivamos la luz
        */
        if(Time.time - effectsStart >= effectsTime){
            shootLight.SetActive(false);
        }
    }

    /*
        Disparar , se llama desde el script "Player.cs" , "Alien.cs"
    */
    public void shoot(string origin){

        // Si puede disparar dispara
        if(Time.time - lastShoot >= fireRate){
            lastShoot = Time.time;                  // Cronometramos el momento del disparo 

            effectsStart = Time.time;               // Cronometramos el momento de empezar el efecto de luz del laser
            shootLight.SetActive(true);             // Activamos laser
            
            // Cargamos el prefab del proyectil
            GameObject newLaserShoot = Instantiate(laserShoot, transform.position , Quaternion.Euler(0, 0, transform.eulerAngles.z));   // Instanciamos
            newLaserShoot.name = laserShoot.name;                                               // Definir nombre
            newLaserShoot.transform.SetParent(GameObject.Find("Projectiles").transform);        // Definir padre
            newLaserShoot.SendMessage("setOrigin",origin);                                      // Definir al creador del proyectil (Jugador/Alien)
        }
        
    }

    /*
        Desactiva el puntero laser, se llama desde "Player.cs" y "Alien.cs"
    */
    public void disablePointer(){
        pointer.SetActive(false);
    }
}
