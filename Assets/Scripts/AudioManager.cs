using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource sonidoRespuestaCorrecta;
    public AudioSource sonidoRespuestaIncorrecta;
    public AudioSource sonidoPresionarBoton;
 
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    
    }
}
