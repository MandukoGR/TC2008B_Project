using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEditor;
using Newtonsoft.Json;

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


    void GetData()
    {
        StartCoroutine(GetDataCoroutine());
    }


    IEnumerator GetDataCoroutine()
    {
        // Wait 5 seconds before doing a request
        yield return new WaitForSeconds(5f);
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:5000/step"))
        {
            // Wait 1 second
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                text.text = www.error;
            }
            else
            {

                //Lista con todas las posiciones de los agentes
                string txt = www.downloadHandler.text.Replace('\'', '\"');
                txt = txt.TrimStart('"', '{', 'd', 'a', 't', 'a', ':', '[');
                txt = "{\"" + txt;
                txt = txt.TrimEnd(']', '}');
                txt = txt + '}';
                string[] strs = txt.Split(new string[] { "}, {" }, StringSplitOptions.None);

                for (int i = 0; i < strs.Length; i++)
                {
                    strs[i] = strs[i].Trim();
                    if (i == 0)
                    {
                        strs[i] = strs[i] + '}';
                        //print(strs[i]);
                    }
                    else if (i == strs.Length - 1)
                    {
                        strs[i] = '{' + strs[i];
                        //print(strs[i]);
                    }
                    else
                    {
                        strs[i] = '{' + strs[i] + '}';
                        //print(strs[i]);
                    }

                    Vector3 test = JsonUtility.FromJson<Vector3>(strs[i]);

                    text.text = test.x + "," + test.y + "," + test.z;


                }

            }
        }
    }
}