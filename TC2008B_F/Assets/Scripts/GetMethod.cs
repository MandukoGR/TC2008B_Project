using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class GetMethod : MonoBehaviour
{
    // Start is called before the first frame update
   public TextMeshProUGUI text;
   public int id;
    void Start()
    {
        //se actualizan los datos obtenidos del servidor
        //cada segundo 
        Invoke("InitialRequest",1f);
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
    // con get data couroutie se obtiene los datos del servidor de python mesa
    // con unity web request podemos acceder a la informacion mediante URL
    IEnumerator GetDataCoroutine(){
        // Wait 5 seconds before doing a request
        string uri = "http://localhost:8585/step";
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            // Wait 1 second
            yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                // text.text = www.error;
            }
            else
            {
                // Debug.Log(www.downloadHandler.text);
                // text.text = www.downloadHandler.text;
            }
        }
    }
    // esta es la request inicial del servidor
    IEnumerator InitialRequest(){
        string uri = "http://localhost:8585/step";
        // wait 1 second
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

