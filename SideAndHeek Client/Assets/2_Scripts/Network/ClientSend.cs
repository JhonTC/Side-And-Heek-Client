using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region Packets
    public static void WelcomeReceived()
    {
        using(Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(UIManager.instance.usernameField.text);

            SendTCPData(_packet);
        }
    }

    public static void PlayerMovement(float inputSpeed, bool[] _otherInputs, Quaternion _rotation)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(inputSpeed);

            _packet.Write(_otherInputs.Length);
            foreach (bool _input in _otherInputs)
            {
                _packet.Write(_input);
            }

            _packet.Write(_rotation);

            SendUDPData(_packet);
        }
    }

    public static void PlayerReady(bool _isReady)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerReady))
        {
            _packet.Write(_isReady);

            SendTCPData(_packet);
        }
    }

    public static void TryStartGame()
    {
        using (Packet _packet = new Packet((int)ClientPackets.tryStartGame))
        {
            SendTCPData(_packet);
        }
    }

    public static void SetPlayerColour(Color _colour, bool _isSeekerColour)
    {
        using (Packet _packet = new Packet((int)ClientPackets.setPlayerColour))
        {
            _packet.Write(_colour);
            _packet.Write(_isSeekerColour);

            SendTCPData(_packet);
        }
    }

    #endregion
}
