using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;                      // AssetDatabase


public class GameTerrainGenerator : MonoBehaviour
{

    public GameObject spaceObject;

    // Start is called before the first frame update
    void Start()
    {
        int x,y;

        for(int i = 0 ;  i < 100 ; ++i){
            x = Random.Range(-100,100);
            y = Random.Range(-100,100);

            GameObject newSpaceObject = Instantiate(spaceObject, new Vector2(x,y), Quaternion.Euler(0, 0, 0));    // Crear copia
            newSpaceObject.transform.SetParent(transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}