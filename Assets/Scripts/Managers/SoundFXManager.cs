using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        //aparecer en gameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //asignar el audiClip
        audioSource.clip = audioClip;

        //asignar volumen
        audioSource.volume = volume;

        //darle play
        audioSource.Play();

        //obtener la longiutud del clip de sonido FX
        Debug.Log(audioSource);
        Debug.Log(audioSource.clip);
        float clipLength = audioSource.clip.length;

        //Destruir el clip despues de haber reproducido
        Destroy(audioSource.gameObject, clipLength);

    }

}
