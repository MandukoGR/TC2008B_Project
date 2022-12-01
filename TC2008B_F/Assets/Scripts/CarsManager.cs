using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class CarsManager : MonoBehaviour
{
    private int id = 1;
    public GameObject carPrefab;
    public TextMeshProUGUI text;
    public int agentCounter = 0;
   
    void Start()
    {
        // cada segundo se instancia un carro con invoke repeating

        InvokeRepeating("InstantiateCar", 2f, 1f);
    }

    void Update()
    {
        // se modifica el texto en pantalla con la aparicion de cada carro
        string agentCounterString = string.Format("Agent Counter: {0}", agentCounter);
        text.text = agentCounterString;

       
    }

    void InstantiateCar(){
        //a cada auto se le identifica con un id 
        carPrefab.GetComponent<AgentController>().id = id;
        id++;
        // se le agrega un contador
        // para el text  mesh
        agentCounter++;
        // se le asigna un nombre en su creacion.
        carPrefab.name = "Car" + id;
        Instantiate(carPrefab, new Vector3(1, 5.81f, 0), Quaternion.identity);
        
    }

}
