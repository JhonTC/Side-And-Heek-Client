using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Server
{
    public class Player : PlayerManager
    {
        private float inputSpeed = 0;
        private bool[] otherInputs = { false, false };
        private Quaternion rotation = Quaternion.identity;

        public PlayerController movementController;

        public int itemAmount = 0;
        public int maxItemCount = 100;

        //[HideInInspector]
        public List<int> activePlayerCollisionIds = new List<int>();

        public bool isBodyActive = false;

        [SerializeField] private PlayerController bodyPrefab;
        [SerializeField] private FootCollisionHandler largeGroundColliderPrefab;
        [SerializeField] private Transform feetMidpoint;
        protected override void Update()
        {
            base.Update();

            for (int i = 0; i < activeTasks.Count; i++)
            {
                activeTasks[i].UpdateTask();
            }
        }

        protected override void FixedUpdate()
        {
            //base.FixedUpdate();

            if (isBodyActive)
            {
                if (otherInputs[0])
                {
                    movementController.OnJump();
                }
                if (otherInputs[1])
                {
                    movementController.OnFlop();
                }
                movementController.CustomFixedUpdate(inputSpeed);
                movementController.SetRotation(rotation);
            }
        }

        public override void Init(int _id, string _username, bool _isReady, bool _hasAuthority, bool _isHunter)
        {
            base.Init(_id, _username, _isReady, _hasAuthority, _isHunter);

            SpawnPlayer();
        }

        public void SetInput(float _inputSpeed, bool[] _otherInputs, Quaternion _rotation)
        {
            inputSpeed = _inputSpeed;
            otherInputs = _otherInputs;
            rotation = _rotation;
        }

        public bool AttemptPickupItem()
        {
            if (itemAmount >= maxItemCount)
            {
                return false;
            }

            itemAmount++;
            return true;
        }

        public void TeleportPlayer(Transform _spawnpoint)
        {
            DespawnPlayer();
            transform.position = _spawnpoint.position;

            SpawnPlayer();
        }

        public void OnCollisionWithOther(float flopTime, bool turnToHunter)
        {
            if (isBodyActive)
            {
                movementController.OnCollisionWithOther(flopTime);
                if (turnToHunter)
                {
                    SetPlayerType(PlayerType.Hunter);
                    LocalGameManager localGameManager = GameManager.instance as LocalGameManager;
                    localGameManager.CheckForGameOver();
                }
            }
        }

        public override void SetPlayerType(PlayerType type)
        {
            base.SetPlayerType(type);

            if (playerType == PlayerType.Hunter)
            {
                LocalGameManager localGameManager = GameManager.instance as LocalGameManager;
                movementController.forwardForceMultipler = localGameManager.hunterSpeedMultiplier;
            }
            else
            {
                movementController.forwardForceMultipler = 1;
            }
        }

        public override void PickupPickedUp(BasePickup pickup)
        {
            if (pickup.pickupType == PickupType.Task)
            {
                activeTasks.Add(PickupManager.instance.HandleTask(pickup as TaskPickup, this));
            }
            else if (pickup.pickupType == PickupType.Item)
            {
                activeItem = PickupManager.instance.HandleItem(pickup as ItemPickup, this);
                UIManager.instance.gameplayPanel.SetItemDetails(pickup as ItemPickup);
            }
        }

        public void SpawnPlayer()
        {
            if (!isBodyActive)
            {
                movementController = Instantiate(bodyPrefab, transform);
                movementController.largeGroundCollider = Instantiate(largeGroundColliderPrefab, transform);
                movementController.feetMidpoint = feetMidpoint;
                movementController.SetupBodyCollisionHandlers(this);

                playerMotor = movementController.GetComponent<PlayerMotor>();
                playerMotor.playerManager = this;

                thirdPersonCamera.GetComponent<FollowPlayer>().target = movementController.transform;

                isBodyActive = true;
            }
        }

        public void DespawnPlayer()
        {
            if (isBodyActive)
            {
                isBodyActive = false;
                Destroy(movementController.largeGroundCollider.gameObject);
                Destroy(movementController.gameObject);
            }
        }

        public override void UseItem()
        {
            if (activeItem != null)
            {
                Debug.Log("Item Used");
                activeItem.ItemUsed();
                activeItem = null;
            }
        }
    }
}
