using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Globalization;

public class GetMethod : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI text;
    public int actualId=1;
    public GameObject carAgent;
    public Transform initialPosition;
    public AgentController car;

    void Start()
    {
        Invoke("InitialRequest",1f);
        InvokeRepeating("GetData", 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        // Invoke GetData every second
    }


    void GetData ()
    {
        StartCoroutine(GetDataCoroutine());
    }


    IEnumerator InitialRequest()
    {
        string uri = "http://localhost:5000/step";
        // wait 1 second
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log(www.downloadHandler.text);
            }
        }
    }

    IEnumerator GetDataCoroutine()
    {
        // Wait 5 seconds before doing a request
        string uri = "http://localhost:5000/step";
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            // Wait 1 second
            yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError)
            {
                //Debug.Log(www.error);
                // text.text = www.error;
            }
            else
            {
                //Debug.Log(www.downloadHandler.text);
                // text.text = www.downloadHandler.text;
                Debug.Log("Generando agente num: " + actualId);
                Instantiate(carAgent, initialPosition.position, initialPosition.rotation);
                car = carAgent.GetComponent<AgentController>();
                car.id = actualId;
                actualId++;
            }
        }
    }
}