using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointedSound : MonoBehaviour
{
    [SerializeField] AudioClip audioClip;
    [SerializeField] AudioClip audioClipSwing;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void PlayOneTime()
    {
        audioSource.PlayOneShot(audioClip);
    } 

    private void PlayOneTimeSwing()
    {
        audioSource.PlayOneShot(audioClipSwing);
    } 
}
