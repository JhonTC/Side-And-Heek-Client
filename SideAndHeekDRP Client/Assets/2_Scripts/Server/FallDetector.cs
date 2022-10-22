using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Server
{
    public class FallDetector : MonoBehaviour
    {
        /*private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "BodyCollider")
            {
                Player player = other.GetComponent<BodyCollisionDetection>().player;
                LocalGameManager localGameManager = GameManager.instance as LocalGameManager;
                player.TeleportPlayer(LevelManager.GetLevelManagerForScene(localGameManager.activeSceneName).GetNextSpawnpoint(true));

                Debug.Log(player.name + " fell out of the map.");
            }
        }*/
    }
}
