using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkSetup : MonoBehaviour
{
    [SerializeField] private FollowPlayer cameraPrefab;

    [SerializeField] private GameObject[] layersToChange;
    [SerializeField] private GameObject[] ObjectsToDisable;

    private void Start()
    {
        //SpawnPlayer(isServer);
        //SpawnPlayer(hasAuthority);
    }

    private void SpawnPlayer(bool isHunter)
    {
        /*if (hasAuthority)
        {
            FollowPlayer camera = Instantiate(cameraPrefab);
            camera.target = transform;
            DontDestroyOnLoad(camera);
        }*/

        if (!isHunter)
        {
            ChangeLayers("Player");
            DisableObjects();
        }
        else
            ChangeLayers("LocalPlayer");
    }

    private void ChangeLayers(string layer)
    {
        foreach (GameObject layerGO in layersToChange)
        {
            layerGO.layer = LayerMask.NameToLayer(layer);
        }
    }

    private void DisableObjects()
    {
        foreach (GameObject GO in ObjectsToDisable)
        {
            GO.SetActive(false);
        }
    }
}
