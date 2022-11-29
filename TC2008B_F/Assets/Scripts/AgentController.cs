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
        InvokeRepeating("GetData", 1f, 0.99f);
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
                // Debug.Log(www.downloadHandler.text);
                position= www.downloadHandler.text;
                position = position.TrimStart('[');
                position = position.TrimEnd(']');
                
                // Only get x from position
                string[] positionArray = position.Split(',');
                string x = positionArray[0];
                x= x.TrimStart('{', '"', 'x', '"', ':');
                x= x.TrimEnd(' ');
                int positionX = int.Parse(x);
                Debug.Log(positionX);

                string z = positionArray[1];
                z= z.TrimStart(' ','"', 'z', '"', ':');
                z= z.TrimEnd(' ');
                int positionZ = int.Parse(z);
                // Debug.Log(positionZ);

              
                if (positionZ == 180){
                    Destroy(gameObject);
                }


                // Move car
                Vector3 currentPosition = transform.position;
                Vector3 targetPos = new Vector3(positionX , 5.81f, positionZ);
                 // Move using Lerp
                float timeElapsed = 0;
                float timeToMove = 1f;
                while (timeElapsed < timeToMove)
                {
                    transform.position = Vector3.Lerp(currentPosition, targetPos, timeElapsed / timeToMove);
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }
                

                // [{"x": 0, "z": 9, "y": 0, "val": 2.0}]
            }
        }
    }
}
