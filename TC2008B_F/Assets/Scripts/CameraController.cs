using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controles para las 5 camaras reparitidas en el juego

public class CameraController : MonoBehaviour
{
    // se definen las camaras 
    public GameObject cam1;
    public GameObject cam2;
    public GameObject cam3;
    public GameObject cam4;
    public GameObject cam5;


    
    void Start()
    {
        //al start se inicia la camara principal
        cam1.SetActive(true);
        cam2.SetActive(false);
        cam3.SetActive(false);
        cam4.SetActive(false);
        cam5.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //con cada uno de los switches se activa las camaras 
        // los numeros son relativos a las camaras

        if (Input.GetButtonDown("Switch1"))
        {
            cam1.SetActive(true);
            cam2.SetActive(false);
            cam3.SetActive(false);
            cam4.SetActive(false);
            cam5.SetActive(false);
        }
        if (Input.GetButtonDown("Switch2"))
        {
            cam1.SetActive(false);
            cam2.SetActive(true);
            cam3.SetActive(false);
            cam4.SetActive(false);
            cam5.SetActive(false);
        }
        if (Input.GetButtonDown("Switch3"))
        {
            cam1.SetActive(false);
            cam2.SetActive(false);
            cam3.SetActive(true);
            cam4.SetActive(false);
            cam5.SetActive(false);
        }
        if (Input.GetButtonDown("Switch4"))
        {
            cam1.SetActive(false);
            cam2.SetActive(false);
            cam3.SetActive(false);
            cam4.SetActive(true);
            cam5.SetActive(false);
        }
        if (Input.GetButtonDown("Switch5"))
        {
            cam1.SetActive(false);
            cam2.SetActive(false);
            cam3.SetActive(false);
            cam4.SetActive(false);
            cam5.SetActive(true);
        }
    }
}
