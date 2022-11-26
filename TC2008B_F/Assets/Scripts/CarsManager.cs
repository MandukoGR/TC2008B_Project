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
        Invoke("InitialRequest", 1f);
        InvokeRepeating("InstantiateCar", 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InstantiateCar(){
        carPrefab.GetComponent<AgentController>().id = id;
        id++;
    }

    IEnumerator InitialRequest(){
        string uri = "http://localhost:8585/step";
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
    }
}
