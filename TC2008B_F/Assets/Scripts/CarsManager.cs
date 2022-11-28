using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class CarsManager : MonoBehaviour
{
    private int id = 1;
    public GameObject carPrefab;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("InstantiateCar", 2f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InstantiateCar(){
        carPrefab.GetComponent<AgentController>().id = id;
        id++;
        Instantiate(carPrefab, new Vector3(0, 5.81f, 0), Quaternion.identity);
    }

}
