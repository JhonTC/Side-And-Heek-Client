using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Net.NetworkInformation;
using Steamworks;

public class Client : MonoBehaviour
{
    public static Client instance;
    public static int dataBufferSize = 4096;

    public string ip = "127.0.0.1";
    public int port = 42069;
    public int myId = 0;
    public TCP tcp;
    public UDP udp;

    public bool isConnected = false;
    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    public string uniqueUserCode;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("UniqueUserCode"))
        {
            uniqueUserCode = PlayerPrefs.GetString("UniqueUserCode");
        } else
        {
            uniqueUserCode = GetMacAddress();
            PlayerPrefs.SetString("UniqueUserCode", uniqueUserCode);
        }
    }

    private string GetMacAddress() {
        foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
        {
            string macAddress = adapter.GetPhysicalAddress().ToString();
            if (macAddress != "")
            {
                return macAddress;
            }
        }

        throw new Exception("ERROR: No mac address found");
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    public void ConnectToServer(string _ip)
    {
        if (_ip != "")
        {
            IPHostEntry hostInfo = Dns.GetHostEntry(_ip);
            /*for (int i = 0; i < hostInfo.AddressList.Length; i++)
            {
                Debug.Log(hostInfo.AddressList[i]);
            }*/
            if (hostInfo.AddressList.Length > 0)
            {
                //ip = hostInfo.AddressList[0].ToString();
                Debug.Log($"Ip set: {ip}.");
            }

            ip = _ip;
        }

        tcp = new TCP();
        udp = new UDP();

        InitialiseClientData();
        ErrorResponseHandler.InitialiseErrorResponseData();

        tcp.Connect();

        //CSteamID id = new CSteamID(UInt64.Parse(ip));
        //SNetSocket_t socket = SteamNetworking.CreateP2PConnectionSocket(CSteamID.NonSteamGS, 42069, 10, true);

        StartCoroutine(ConnectionTimeoutTimer());
    }

    IEnumerator ConnectionTimeoutTimer(int waitTime = 15)
    {
        int currentTime = 0;
        while (currentTime < waitTime && !isConnected)
        {
            yield return new WaitForSeconds(1.0f);

            currentTime++;
        }

        if (!isConnected)
        {
            UIManager.instance.OnConnectionFailed();
            Debug.Log("Connection timeout: Failed to receive response from server.");
        }
    }

    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
        }

        private void ConnectCallback(IAsyncResult _result)
        {
            socket.EndConnect(_result);

            if (!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream();

            receivedData = new Packet();

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception _e)
            {
                Debug.Log($"Error sending data to server via TCP: {_e}");
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    instance.Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                receivedData.Reset(HandleData(_data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception _e)
            {
                Console.WriteLine($"Error receiving TCP data: {_e}");
                Disconnect();
            }
        }

        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;

            receivedData.SetBytes(_data);

            if (receivedData.UnreadLength() >= 4)
            {
                _packetLength = receivedData.ReadInt();
                if (_packetLength <= 0)
                {
                    return true;
                }
            }

            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {
                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        packetHandlers[_packetId](_packet);
                    }
                });

                _packetLength = 0;
                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }
            }

            if (_packetLength <= 1)
            {
                return true;
            }

            return false;
        }

        private void Disconnect()
        {
            Debug.Log("TCP disconnection.");
            instance.Disconnect();

            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
        }

        public void Connect(int _localPort)
        {
            socket = new UdpClient(_localPort);

            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            using (Packet _packet = new Packet())
            {
                SendData(_packet);
            }
        }

        public void SendData(Packet _packet)
        {
            try
            {
                _packet.InsertInt(instance.myId);
                if (socket != null)
                {
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }
            }
            catch (Exception _e)
            {
                Debug.Log($"Error sending data to server via UDP: {_e}");
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                byte[] _data = socket.EndReceive(_result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                if (_data.Length < 4)
                {
                    instance.Disconnect();
                    return;
                }

                HandleData(_data);
            }
            catch (Exception _e)
            {
                Console.WriteLine($"Error receiving UDP data: {_e}");
                Disconnect();
            }
        }

        private void HandleData(byte[] _data)
        {
            using (Packet _packet = new Packet(_data))
            {
                int _packetLength = _packet.ReadInt();
                _data = _packet.ReadBytes(_packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_data))
                {
                    int _packetId = _packet.ReadInt();
                    packetHandlers[_packetId](_packet);
                }
            });
        }

        private void Disconnect()
        {
            Debug.Log("UDP disconnection.");
            instance.Disconnect();

            endPoint = null;
            socket = null;
        }
    }
    
    private void InitialiseClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.Welcome },
            { (int)ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer },
            { (int)ServerPackets.playerPosition, ClientHandle.PlayerPositions },
            { (int)ServerPackets.playerRotation, ClientHandle.PlayerRotation },
            { (int)ServerPackets.playerState, ClientHandle.PlayerState },
            { (int)ServerPackets.playerDisconnected, ClientHandle.PlayerDisconnected },
            { (int)ServerPackets.createItemSpawner, ClientHandle.CreatePickupSpawner },
            { (int)ServerPackets.pickupSpawned, ClientHandle.PickupSpawned },
            { (int)ServerPackets.pickupPickedUp, ClientHandle.PickupPickedUp },
            { (int)ServerPackets.itemSpawned, ClientHandle.ItemSpawned },
            { (int)ServerPackets.itemTransform, ClientHandle.ItemTransform },
            { (int)ServerPackets.itemUseComplete, ClientHandle.ItemUseComplete },
            { (int)ServerPackets.playerReadyToggled, ClientHandle.PlayerReadyToggled },
            { (int)ServerPackets.changeScene, ClientHandle.ChangeScene },
            { (int)ServerPackets.unloadScene, ClientHandle.UnloadScene },
            { (int)ServerPackets.setPlayerType, ClientHandle.SetPlayerType },
            { (int)ServerPackets.setSpecialCountdown, ClientHandle.SetSpecialCountdown },
            { (int)ServerPackets.setPlayerColour, ClientHandle.SetPlayerColour },
            { (int)ServerPackets.setPlayerMaterialType, ClientHandle.SetPlayerMaterialType },
            { (int)ServerPackets.sendErrorResponseCode, ClientHandle.RecieveErrorResponse },
            { (int)ServerPackets.gameStart, ClientHandle.GameStart },
            { (int)ServerPackets.gameOver, ClientHandle.GameOver },
            { (int)ServerPackets.resetGame, ClientHandle.ResetGameAndReturnToLobby },
            { (int)ServerPackets.playerTeleportStart, ClientHandle.PlayerTeleportStart },
            { (int)ServerPackets.playerTeleportComplete, ClientHandle.PlayerTeleportComplete },
            { (int)ServerPackets.gameRulesChanged, ClientHandle.GameRulesChanged }
        };
        Debug.Log("Initialised packets.");
    }

    public void Disconnect()
    {
        if (isConnected)
        {
            isConnected = false;
            tcp.socket.Close();
            udp.socket.Close();

            UIManager.instance.gameplayPanel.UpdatePlayerTypeViews();
            UIManager.instance.customisationPanel.hiderColourSelector.ClearAll();

            GameManager.instance.FadeMusic(false);

            foreach (Pickup pickup in PickupHandler.pickups.Values)
            {
                Destroy(pickup.gameObject);
            }
            PickupHandler.pickups.Clear();
            PickupHandler.pickupLog.Clear();

            foreach (SpawnableObject item in ItemHandler.items.Values)
            {
                Destroy(item.gameObject);
            }
            ItemHandler.items.Clear();
            ItemHandler.itemLog.Clear();

            //foreach (PlayerManager player in GameManager.players.Values)
            //{
            //    Destroy(player.gameObject);
            //}
            //GameManager.players.Clear();
            //UIManager.instance.playerReadyGems.Clear();

            //GameManager.instance.LoadScene("Lobby", UnityEngine.SceneManagement.LoadSceneMode.Single);

            //UIManager.instance.DisplayStartPanel();
            //GameManager.instance.sceneCamera.SetActive(true);

            Debug.Log("Disconnected from server.");
        }
    }
}
