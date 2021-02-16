using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Server
{
    public class Pickup : PickupSpawner
    {
        public static Dictionary<int, Pickup> spawners = new Dictionary<int, Pickup>();
        public static Dictionary<TaskCode, int> tasksLog = new Dictionary<TaskCode, int>();
        public static Dictionary<ItemCode, int> itemsLog = new Dictionary<ItemCode, int>();
        private static int nextSpawnerId = 1;

        public int maxSpawnCount = 0;

        protected override void Start()
        {
            base.Start();

            hasPickup = false;
            spawnerId = nextSpawnerId;
            nextSpawnerId++;
            spawners.Add(spawnerId, this);

            Init(spawnerId, pickupType, false, 0);

            StartCoroutine(SpawnPickup());
        }

        private void OnDestroy()
        {
            if (spawners.ContainsKey(spawnerId))
            {
                spawners.Remove(spawnerId);
            }
        }

        /*private void OnTriggerEnter(Collider other)
        {
            if (hasItem && other.CompareTag("BodyCollider"))
            {
                Player _player = other.GetComponentInParent<Player>();
                if (_player.AttemptPickupItem())
                {
                    ItemPickedUp(_player.id);
                }
            }
        }*/

        private IEnumerator SpawnPickup(int spawnDelay = 1)
        {
            yield return new WaitForSeconds(spawnDelay);

            if (!HaveAllPickupsOfTypeBeenSpawned(pickupType))
            {
                if (pickupType == PickupType.Task)
                {
                    TaskDetails newTaskDetails = LocalGameManager.instance.collection.GetRandomTask();

                    while (!CanTaskCodeBeUsed(newTaskDetails))
                    {
                        newTaskDetails = LocalGameManager.instance.collection.GetRandomTask();
                    }

                    if (tasksLog.ContainsKey(newTaskDetails.task.taskCode))
                    {
                        tasksLog[newTaskDetails.task.taskCode]++;
                    }
                    else
                    {
                        tasksLog.Add(newTaskDetails.task.taskCode, 1);
                    }

                    hasPickup = true;

                    PickupSpawned(pickupType, (int)newTaskDetails.task.taskCode);
                }
                else if (pickupType == PickupType.Item)
                {
                    ItemDetails newItemDetails = LocalGameManager.instance.collection.GetRandomItem();

                    while (!CanItemCodeBeUsed(newItemDetails))
                    {
                        newItemDetails = LocalGameManager.instance.collection.GetRandomItem();
                    }

                    if (itemsLog.ContainsKey(newItemDetails.item.itemCode))
                    {
                        itemsLog[newItemDetails.item.itemCode]++;
                    }
                    else
                    {
                        itemsLog.Add(newItemDetails.item.itemCode, 1);
                    }

                    hasPickup = true;

                    PickupSpawned(pickupType, (int)newItemDetails.item.itemCode);
                }
            }
        }

        private bool CanTaskCodeBeUsed(TaskDetails taskDetails)
        {
            if (tasksLog.ContainsKey(taskDetails.task.taskCode))
            {
                if (tasksLog[taskDetails.task.taskCode] >= taskDetails.numberOfUses)
                {
                    return false;
                }
            }

            return true;
        }

        private bool CanItemCodeBeUsed(ItemDetails itemDetails)
        {
            if (itemsLog.ContainsKey(itemDetails.item.itemCode))
            {
                if (itemsLog[itemDetails.item.itemCode] >= itemDetails.numberOfUses)
                {
                    return false;
                }
            }

            return true;
        }

        private bool HaveAllPickupsOfTypeBeenSpawned(PickupType pickupType)
        {
            if (pickupType == PickupType.Task)
            {
                if (tasksLog.Count < LocalGameManager.instance.collection.taskDetails.Count)
                {
                    return false;
                }

                foreach (TaskCode taskCode in tasksLog.Keys)
                {
                    if (tasksLog.ContainsKey(taskCode))
                    {
                        if (tasksLog[taskCode] < LocalGameManager.instance.collection.GetTaskByCode(taskCode).numberOfUses)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (pickupType == PickupType.Item)
            {
                if (itemsLog.Count < LocalGameManager.instance.collection.itemDetails.Count)
                {
                    return false;
                }

                foreach (ItemCode itemCode in itemsLog.Keys)
                {
                    if (itemsLog.ContainsKey(itemCode))
                    {
                        if (itemsLog[itemCode] < LocalGameManager.instance.collection.GetItemByCode(itemCode).numberOfUses)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void PickupPickedUp(int _byPlayer)
        {
            hasPickup = false;

            int code = 0;
            BasePickup pickup = null;
            if (pickupType == PickupType.Task)
            {
                if (activeTaskDetails != null)
                {
                    pickup = activeTaskDetails.task;
                    code = (int)activeTaskDetails.task.taskCode;
                }
            }
            else if (pickupType == PickupType.Item)
            {
                if (activeItemDetails != null)
                {
                    pickup = activeItemDetails.item;
                    code = (int)activeItemDetails.item.itemCode;
                }
            }

            LobbyManager.players[_byPlayer].PickupPickedUp(pickup);

            StartCoroutine(SpawnPickup());
        }
    }
}
