using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class GetMethod : MonoBehaviour
{
    // Start is called before the first frame update
   public TextMeshProUGUI text;
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
        yield return new WaitForSeconds(5f);
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:8585/step"))
        {
            // Wait 1 second
            yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                text.text = www.error;
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                text.text = www.downloadHandler.text;
            }
        }
    }

    
}

