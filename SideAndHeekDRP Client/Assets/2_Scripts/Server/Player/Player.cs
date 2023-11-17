﻿using System.Collections;
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

        public override void Init(int _id, string _username, bool _isReady, bool _hasAuthority, bool _isHost)
        {
            base.Init(_id, _username, _isReady, _hasAuthority, _isHost);

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

        public override void PickupPickedUp(PickupSO _pickupSO)
        {
            activePickup = NetworkObjectsManager.instance.pickupHandler.HandlePickup(_pickupSO, this);

            UIManager.instance.gameplayPanel.SetItemDetails(_pickupSO);
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

        public override void UsePickup()
        {
            if (activePickup != null)
            {
                Debug.Log("Item Used");
                activePickup.PickupUsed();
                activePickup = null;
            }
        }
    }
}