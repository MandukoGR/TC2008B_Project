using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Globalization;

public class AgentController : MonoBehaviour
{
    // Start is called before the first frame update
    public int id;
    public string position;
    //public GameObject carAgent;

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
        string uri = "http://localhost:5000/position?id=" + id;
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
                position = www.downloadHandler.text;

                var charsToRemove = new string[] {"[","{",":","x","z","}","]"," "};
                foreach (var c in charsToRemove)
                {
                    position = position.Replace(c, string.Empty);
                }
                string[] coordinatesString = position.Split(",");
                string tempVar;
                float numVal;
                List<float> coordinatesInt = new List<float>();
                for (int i = 0; i < 2; i++)
                {
                    tempVar = coordinatesString[i].Remove(0,2);
                    numVal = float.Parse(tempVar, CultureInfo.InvariantCulture.NumberFormat);
                    coordinatesInt.Add(numVal);
                }

                /*Debug.Log("Elementos de la lista");
                Debug.Log(coordinatesInt[0]);
                Debug.Log(coordinatesInt[1]);*/

                //Desparecer carrito - no funcional por el momento
                /*if (coordinatesInt[1] >= 180)
                {
                    carAgent.SetActive(false);
                }*/

                Vector3 currentPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                Vector3 targetPos = new Vector3(coordinatesInt[0],5.81f,coordinatesInt[1]);
                float timeElapsed = 0;
                float timeToMove = 1;
                while (timeElapsed < timeToMove)
                {
                    transform.position = Vector3.Lerp(currentPos, targetPos, timeElapsed / timeToMove);
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }


            }
        }
    }

}