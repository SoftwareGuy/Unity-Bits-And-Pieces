// Server Optimizer: This script is best used on a empty GameObject that will persist through the lifecycle of your
// game. Every time a new scene loads on a headless server instance, it will seek out for any cameras inside the scene
// and audio sources (ie. ambience, etc), disabling and muting them (if they're audio).
// Written by Matt Coburn (Coburn64/SoftwareGuy). Version 1.0 (24th December 2021).
// Licensed under MIT license. See LICENSE for more information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Coburn.Toolbox
{
    // This script is only properly usable in Unity Server environments, unless you're using the Unity Editor to test
    // a headless server instance inside the editor environment.
    public class ServerOptimizations : MonoBehaviour
    {   
        // No need to compile this in for Unity Standalone instances.
#if UNITY_EDITOR || UNITY_SERVER
        private Camera[] cameras;
        private AudioSource[] audioSources;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        // Callback function that is fired when the Unity Scene Manager loads scenes.
        private void OnSceneLoaded(Scene newScene, LoadSceneMode mode)
        {
            Debug.Log("Server Optimizations: Detected scene load, optimizing scene...");

            // Find cameras to disable them, since servers don't need rendering.
            cameras = FindObjectsOfType<Camera>();
            // Find audio sources in the scene to disable them too, since servers should be silent...
            audioSources = FindObjectsOfType<AudioSource>();
            
            // Run through the camera list...
            for (int i = 0; i < cameras.Length; i++)
            {
                if (cameras[i] == null) continue;

                cameras[i].enabled = false;
            }

            // Run through the audio sources list...
            for (int i = 0; i < audioSources.Length; i++)
            {
                if (audioSources[i] == null) continue;

                // Mute it just to be safe.
                audioSources[i].mute = true;

                // Stop the annoyance.
                if (audioSources[i].isPlaying)
                    audioSources[i].Stop();

                // Finally disable it so we can free up some resources.
                audioSources[i].enabled = false;
            }
        }
        
        // That's it! Did you expect more? Sorry to disappoint.
    }
#endif
}
