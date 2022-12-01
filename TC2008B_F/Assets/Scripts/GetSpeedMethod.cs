using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class GetSpeedMethod : MonoBehaviour
{
    // Start is called before the first frame update
   public TextMeshProUGUI text;
   public int id;

   // a traves de la informacion que se involucra, en el servidor
   // se obtinene la velocidad de cada agenta para secuentemente
   //obtener un promedio
    void Start()
    {
        InvokeRepeating("GetData", 3f, 1.1f);
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
    // se obtiene la velocidad mediante el ingreso
    //del servidor con url en la funcion speed
    IEnumerator GetDataCoroutine(){
        // Wait 5 seconds before doing a request
        string uri = "http://localhost:8585/speed";

        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            // Wait 1 second
            yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            
                //Se obtiene el texto error
                text.text = www.error;
            }
            else
            {
                // se obtiene la velocidad
                // Debug.Log(www.downloadHandler.text);
                string speed = www.downloadHandler.text;
                speed = speed.TrimStart('{', '"', 'd', 'a', 't', 'a', '"', ':');
                speed = speed.TrimEnd('}');
                float speedFloat = float.Parse(speed);
                speedFloat = speedFloat * 5.5f;
                text.text = speedFloat.ToString() + " m/s";
            }
        }
    }

    

    
}

