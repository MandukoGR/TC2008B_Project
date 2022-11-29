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
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("InstantiateCar", 2f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
        string agentCounterString = string.Format("Agent Counter: {0}", agentCounter);
        text.text = agentCounterString;

       
    }

    void InstantiateCar(){
        carPrefab.GetComponent<AgentController>().id = id;
        id++;
        agentCounter++;
        carPrefab.name = "Car" + id;
        Instantiate(carPrefab, new Vector3(1, 5.81f, 0), Quaternion.identity);
        
    }

}
