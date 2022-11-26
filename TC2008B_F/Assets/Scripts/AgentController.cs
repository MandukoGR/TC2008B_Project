using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class AgentController : MonoBehaviour
{
    // Start is called before the first frame update
   public int id;
   public string position;
    void Start()
    {
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
    
    IEnumerator GetDataCoroutine(){
        // Wait 5 seconds before doing a request
        string uri = "http://localhost:8585/position?id=" + id;
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            // Wait 1 second
            yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                position = www.error;
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                position= www.downloadHandler.text;
            }
        }
    }
}
