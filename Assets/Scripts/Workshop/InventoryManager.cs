using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventoryManager : MonoBehaviour
{
    public GameObject[] slots;      // Inventario
    private int selectedSlot;       // Celda seleccionada
    bool mustUpdate;                // Almacena si hay o no que actualizar la UI (cuando se ha cambiado de celda seleccionada)
    ShipBuilder shipBuilder;        // Referencia al script ShipBuilder

    void Start()
    {
        mustUpdate = true;
        shipBuilder = (ShipBuilder) GameObject.Find("ShipBuilder").GetComponent(typeof(ShipBuilder));

        for( int i = 0 ; i < 10 ; ++i){
            if(i < slots.Length){
                // Ponemos imagen del item en el slot 
                transform.GetChild(i).transform.GetChild(2).GetComponent<Image>().sprite = slots[i].GetComponent<SpriteRenderer>().sprite;
            }else{
                // Si no hay item en este slot desactivamos la imagen
                transform.GetChild(i).transform.GetChild(2).GetComponent<Image>().enabled = false;
            }
        }

    }

    void Update()
    {
        // Comprobamos las teclas 
         if (Input.GetKeyDown("1"))
        {
            selectedSlot = 0;
            mustUpdate = true;
        }
        if (Input.GetKeyDown("2"))
        {
            selectedSlot = 1;
            mustUpdate = true;
        }
        if (Input.GetKeyDown("3"))
        {
            selectedSlot = 2;
            mustUpdate = true;
        }
        if (Input.GetKeyDown("4"))
        {
            selectedSlot = 3;
            mustUpdate = true;
        }

        if (Input.GetKeyDown("5"))
        {
            selectedSlot = 4;
            mustUpdate = true;
        }

        // Comprobamos raton 
        if(Input.mouseScrollDelta.y > 0)
        {
            selectedSlot--;
            if(selectedSlot < 0)
            {
                selectedSlot = transform.childCount-1;
            }
            mustUpdate = true;
        }
        else if(Input.mouseScrollDelta.y < 0)
        {
            selectedSlot++;
            if(selectedSlot > transform.childCount-1)
            {
                selectedSlot = 0;
            }
            mustUpdate = true;
        }

        // Si se ha cambiado de celda seleccionada tenemos que actualizarlas
        if (mustUpdate){
            
            // Actualizamos la UI
            for (int i = 0 ;  i < transform.childCount ; ++i){
                if(i != selectedSlot)
                    transform.GetChild(i).transform.GetChild(1).gameObject.SetActive(false);    // Ocultar el prite "seleccionado"
                else
                    transform.GetChild(i).transform.GetChild(1).gameObject.SetActive(true);     // Mostrar el sprite "seleccionado"
            }

            // Actualizar la pieza con la que se va a construir
            if(selectedSlot < slots.Length && slots[selectedSlot] != null)
                shipBuilder.setInventorySelectedPart(slots[selectedSlot]);
            else
                shipBuilder.setInventorySelectedPart(null);
            
            mustUpdate = false;
        }

    }


}
