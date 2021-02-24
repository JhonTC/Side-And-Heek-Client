using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Server
{
    public class LocalGameManager : GameManager
    {
        [SerializeField] private int specialSpawnDelay = 20;
        private int specialSpawnCount = 0;

        public string activeSceneName;

        public int maxPlayDuration = 240;

        public float hunterSpeedMultiplier = 1f;

        private bool tryStartGameActive = false;

        protected override void Start()
        {
            base.Start();
            activeSceneName = lobbyScene;
        }

        public void SpawnPlayer()
        {
            int newId = LobbyManager.players.Count + 1;
            Debug.Log($"Message from GameManager: Spawn Player with id: {newId}");

            Transform spawnpoint = LevelManager.GetLevelManagerForScene(activeSceneName).GetNextSpawnpoint(true);
            LobbyManager.instance.SpawnPlayer(newId, "", false, true, LobbyManager.players.Count == 0, spawnpoint.position, spawnpoint.rotation, LobbyManager.instance.unreadyColour);

            UIManager.instance.AddPlayerReady(newId);
        }

        protected override void OnLevelFinishedLoading(Scene _scene, LoadSceneMode _loadSceneMode)
        {
            base.OnLevelFinishedLoading(_scene, _loadSceneMode);

            activeSceneName = _scene.name;

            foreach (PlayerManager playerManager in LobbyManager.players.Values)
            {
                Player player = playerManager as Player;
                if (player.playerType != PlayerType.Hunter)
                {
                    player.TeleportPlayer(LevelManager.GetLevelManagerForScene(activeSceneName).GetNextSpawnpoint(!gameStarted));
                }
                else
                {
                    StartCoroutine(SpawnSpecial(player, specialSpawnDelay));
                }
            }
        }

        protected override void OnLevelFinishedUnloading(Scene _scene)
        {
            base.OnLevelFinishedUnloading(_scene);

            activeSceneName = lobbyScene;
            Pickup.itemsLog.Clear();
            Pickup.tasksLog.Clear();
        }

        private IEnumerator SpawnSpecial(Player _player, int _delay = 60)
        {
            specialSpawnCount = _delay;
            while (specialSpawnCount > 0)
            {
                yield return new WaitForSeconds(1.0f);

                UIManager.instance.SetCountdown(specialSpawnCount);

                specialSpawnCount--;
            }

            _player.TeleportPlayer(LevelManager.GetLevelManagerForScene(activeSceneName).GetNextSpawnpoint(true));

            tryStartGameActive = false;

            LobbyManager.instance.tryStartGameActive = false;
            UIManager.instance.DisableAllPanels();

            StartCoroutine(GameTimeCountdown(maxPlayDuration));
        }

        private IEnumerator GameTimeCountdown(int _delay = 240)
        {
            currentTime = _delay;
            while (currentTime > 0 && gameStarted)
            {
                yield return new WaitForSeconds(1.0f);

                UIManager.instance.SetCountdown(currentTime, currentTime <= 1);
                currentTime--;
            }

            UIManager.instance.SetMessage("Game Over", 2f, true);

            if (gameStarted)
            {
                GameOver(false);
            }
        }

        public void TryStartGame(int _fromClient)
        {
            if (!tryStartGameActive)
            {
                bool areAllPlayersReady = AreAllPlayersReady();
                if (areAllPlayersReady)
                {
                    int _randPlayerId = LobbyManager.players.ElementAt(Random.Range(0, LobbyManager.players.Count)).Value.id;

                    foreach (PlayerManager playerManager in LobbyManager.players.Values)
                    {
                        Player player = playerManager as Player;

                        PlayerType _playerType = PlayerType.Default;
                        if (player.id == _randPlayerId)
                        {
                            _playerType = PlayerType.Hunter;
                        }
                        else
                        {
                            _playerType = PlayerType.Hider;
                        }
                        player.SetPlayerType(_playerType);
                    }

                    tryStartGameActive = true;

                    LevelManager.GetLevelManagerForScene(activeSceneName).LoadScene(LobbyManager.instance.mapScene, LevelType.Map);
                }
                else
                {
                    ErrorResponseHandler.HandleErrorResponse(ErrorResponseCode.NotAllPlayersReady);
                    tryStartGameActive = false;
                }
            }
            else
            {
                ErrorResponseHandler.HandleErrorResponse(ErrorResponseCode.NotAllPlayersReady); //TODO:Change to different message(game already trying to start)
                tryStartGameActive = false;
            }
        }

        private bool AreAllPlayersReady()
        {
            foreach (PlayerManager playerManager in LobbyManager.players.Values)
            {
                if (!playerManager.isReady)
                {
                    return false;
                }
            }

            return true;
        }

        public void CheckForGameOver()
        {
            foreach (PlayerManager playerManager in LobbyManager.players.Values)
            {
                if (playerManager.playerType == PlayerType.Hider)
                {
                    return;
                }
            }

            GameOver(true);
        }

        public void GameOver(bool _isHunterVictory)
        {
            //ServerSend.GameOver(_isHunterVictory);

            gameStarted = false;

            foreach (PlayerManager playerManager in LobbyManager.players.Values)
            {
                Player player = playerManager as Player;

                player.GameOver();

                player.playerType = PlayerType.Default;
                player.isReady = false;

                player.TeleportPlayer(LevelManager.GetLevelManagerForScene(lobbyScene).GetNextSpawnpoint(!gameStarted));
            }


            SceneManager.UnloadSceneAsync(LobbyManager.instance.mapScene);
            //ServerSend.UnloadScene("Map_1");

            Debug.Log("Game Over, Hunters Win!");
        }
    }
}
