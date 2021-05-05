using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    int damage;                             // Daño que inflinge el proyectil
    static float SPEED = 200;               // Velocidad del proyectil
    Vector2 startPosition;                  // Posicion en la que se crea el proyectil
    string origin;                          // Objeto creador del disparo (Player/Alien)

    bool isPaused;                          // Indica si el juego esta o no pausado

    // Start is called before the first frame update
    void Start()
    {
        damage = 1;
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {   
        if(!isPaused){
            // Si se aleja en 50 unidades el proyectil se destruye
            if(Mathf.Sqrt(Mathf.Pow(startPosition.x-transform.position.x,2) + Mathf.Pow(startPosition.y-transform.position.y,2)) > 150){
                Destroy(this.gameObject);
            }

            transform.position += transform.right * Time.deltaTime * SPEED;   // Avanza
        }
    }

    /*
        Establace el origen del disparo , se llama desde "Laser.cs"
    */
    public void setOrigin(string origin){
        this.origin = origin;
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        // Si el origen es el jugador y le da a un alien
        if(origin == "player" && collider2D.gameObject.layer == 9){
            collider2D.SendMessage("takeDamage", damage);               // Mandamos mensaje al bloque impactado para quitarle vida
            Destroy(this.gameObject);                                   // Destruimos el proyectil
        }

        // Si el origen es un alien y le da al jugador
        if(origin == "alien" && collider2D.gameObject.layer == 8){
            collider2D.SendMessage("takeDamage", damage);               // Mandamos mensaje al bloque impactado para quitarle vida
            Destroy(this.gameObject);                                   // Destruimos el proyectil
        }

    }

    /*
        Se llama al pausar el juego , se llama desde GameController.cs
    */
    public void pause(){
        if(isPaused)
            isPaused = false;
        else
            isPaused = true;
    }

}
