using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public AudioSource Grass_footsteps_new;
    public float minPitch = 0.8f;
    public float maxPitch = 1.1f;
    float lastTime = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            Grass_footsteps_new.enabled = true;
            
        }
        else
        {
            Grass_footsteps_new.enabled = false;
            
        }
        if (Grass_footsteps_new.time < lastTime)
        {
            Grass_footsteps_new.pitch = Random.Range(minPitch, maxPitch);
            Debug.Log("Pitch change");
        } 
        lastTime = Grass_footsteps_new.time;
        
    }
}
