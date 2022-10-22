using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Server
{
    public class S_PickupSpawner : PickupSpawner
    {
        public int maxSpawnCount = 0;

        public int code;

        private void Start()
        {
            hasPickup = false;
            GameManager.pickupSpawners.Add(spawnerId, this);

            Init(spawnerId);

            StartCoroutine(SpawnPickup());
        }

        private void OnDestroy()
        {
            if (GameManager.pickupSpawners.ContainsKey(spawnerId))
            {
                GameManager.pickupSpawners.Remove(spawnerId);
            }
        }

        private IEnumerator SpawnPickup(int spawnDelay = 10)
        {
            yield return new WaitForSeconds(spawnDelay);

            if (!PickupHandler.HaveAllPickupsBeenSpawned())
            {
                PickupDetails newItemDetails = GameManager.instance.collection.GetRandomItem();

                while (!PickupHandler.CanPickupCodeBeUsed(newItemDetails))
                {
                    newItemDetails = GameManager.instance.collection.GetRandomItem();
                }

                if (PickupHandler.pickupLog.ContainsKey(newItemDetails.pickupSO.pickupCode))
                {
                    PickupHandler.pickupLog[newItemDetails.pickupSO.pickupCode]++;
                }
                else
                {
                    PickupHandler.pickupLog.Add(newItemDetails.pickupSO.pickupCode, 1);
                }

                hasPickup = true;

                //todo: PickupSpawned(PickupHandler PickupHandler.pickups.Count, (int)newItemDetails.pickupSO.pickupCode, transform.position, transform.rotation);
            }
        }

        public void PickupPickedUp(ushort _byPlayer)
        {
            hasPickup = false;

            LobbyManager.players[_byPlayer].PickupPickedUp(activePickup.activeObjectDetails.pickupSO);
            activePickup = null;

            StartCoroutine(SpawnPickup());
        }
    }
}
