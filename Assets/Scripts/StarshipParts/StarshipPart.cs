using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarshipPart : MonoBehaviour
{

    public int healthPoints;                                // Puntos de vida del bloque
    public string PART_KIND;                                // Tipo de pieza
    public string host;                                     // Nombre del objeto al que pertenece el bloque

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
        Sirve para recibir daño , recibe la cantidad de daño recibida, se llama desde "Projectile.cs"
    */
    public void takeDamage(int damage){
        healthPoints -= damage;                         // Quita vida

        if(healthPoints <= 0){                          // Si no queda vida destruimos el bloque

            if(transform.name == "StarshipCore"){       // Si el bloque era "StarshipCore" el padre (alien/nave) muere
                transform.parent.transform.parent.transform.SendMessage("die");
            }

            Destroy(this.gameObject);
        }
    }
}
