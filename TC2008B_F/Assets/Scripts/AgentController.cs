using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class AgentController : MonoBehaviour
{
    // Start is called before the first frame update
    //cars id
   public int id;
   //posiciones de los carros
   public string position;

   public GameObject carsManager;

    void Start()
    {
        //se obtiene la informacion de las cordenadas y es
        //actualizado mediante invoke repeating
        carsManager = GameObject.Find("CarManager");
        InvokeRepeating("GetData", 0.9f, 1f);
    }


    void GetData ()
    {
        StartCoroutine(GetDataCoroutine());
    }
    
    IEnumerator GetDataCoroutine(){
        //se obtiene del servidor la posciicon exacta del carro mediante
        // su id
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
                // Debug.Log(www.downloadHandler.text);
                position= www.downloadHandler.text;
                position = position.TrimStart('[');
                position = position.TrimEnd(']');
                
                // Solo se obtiene x de la posicion 
                string[] positionArray = position.Split(',');
                string x = positionArray[0];
                x= x.TrimStart('{', '"', 'x', '"', ':');
                x= x.TrimEnd(' ');
                int positionX = int.Parse(x);
                Debug.Log(positionX);
                // Solo se obtiene y de la posicion
                string z = positionArray[1];
                z= z.TrimStart(' ','"', 'z', '"', ':');
                z= z.TrimEnd(' ');
                int positionZ = int.Parse(z);
                // Debug.Log(positionZ);

              
                if (positionZ == 180){
                    Destroy(gameObject);
                    carsManager.GetComponent<CarsManager>().agentCounter--;
                    
                }


                Vector3 currentPosition = transform.position;
                Vector3 targetPos = new Vector3(positionX , 5.81f, positionZ);

               
                float timeElapsed = 0;
                float timeToMove = 1f;
                while (timeElapsed < timeToMove)
                {
                    transform.position = Vector3.Lerp(currentPosition, targetPos, timeElapsed / timeToMove);
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }
                

            }
        }
    }
}
