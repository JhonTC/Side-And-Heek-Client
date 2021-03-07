using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Server
{
    public class S_PickupSpawner : PickupSpawner
    {
        private static int nextSpawnerId = 1;

        public int maxSpawnCount = 0;

        public PickupType pickupType;
        public int code;

        private void Start()
        {
            hasPickup = false;
            spawnerId = nextSpawnerId;
            nextSpawnerId++;
            GameManager.pickupSpawners.Add(spawnerId, this);

            Init(spawnerId, PickupType.NULL);

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

            if (!PickupManager.instance.HaveAllPickupsOfTypeBeenSpawned(pickupType))
            {
                if (pickupType == PickupType.Task)
                {
                    TaskDetails newTaskDetails = GameManager.instance.collection.GetRandomTask();

                    while (!PickupManager.instance.CanTaskCodeBeUsed(newTaskDetails))
                    {
                        newTaskDetails = GameManager.instance.collection.GetRandomTask();
                    }

                    if (PickupManager.tasksLog.ContainsKey(newTaskDetails.task.taskCode))
                    {
                        PickupManager.tasksLog[newTaskDetails.task.taskCode]++;
                    }
                    else
                    {
                        PickupManager.tasksLog.Add(newTaskDetails.task.taskCode, 1);
                    }

                    hasPickup = true;

                    PickupSpawned(PickupManager.pickups.Count, pickupType, (int)newTaskDetails.task.taskCode, transform.position, transform.rotation);
                }
                else if (pickupType == PickupType.Item)
                {
                    ItemDetails newItemDetails = GameManager.instance.collection.GetRandomItem();

                    while (!PickupManager.instance.CanItemCodeBeUsed(newItemDetails))
                    {
                        newItemDetails = GameManager.instance.collection.GetRandomItem();
                    }

                    if (PickupManager.itemsLog.ContainsKey(newItemDetails.item.itemCode))
                    {
                        PickupManager.itemsLog[newItemDetails.item.itemCode]++;
                    }
                    else
                    {
                        PickupManager.itemsLog.Add(newItemDetails.item.itemCode, 1);
                    }

                    hasPickup = true;

                    PickupSpawned(PickupManager.pickups.Count, pickupType, (int)newItemDetails.item.itemCode, transform.position, transform.rotation);
                }
            }
        }

        public void PickupPickedUp(int _byPlayer)
        {
            hasPickup = false;

            int code = 0;
            BasePickup pickup = null;
            if (pickupType == PickupType.Task)
            {
                if (activePickup.activeTaskDetails != null)
                {
                    pickup = activePickup.activeTaskDetails.task;
                    code = (int)activePickup.activeTaskDetails.task.taskCode;
                }
            }
            else if (pickupType == PickupType.Item)
            {
                if (activePickup.activeItemDetails != null)
                {
                    pickup = activePickup.activeItemDetails.item;
                    code = (int)activePickup.activeItemDetails.item.itemCode;
                }
            }

            LobbyManager.players[_byPlayer].PickupPickedUp(pickup);
            activePickup = null;


            StartCoroutine(SpawnPickup());
        }
    }
}
