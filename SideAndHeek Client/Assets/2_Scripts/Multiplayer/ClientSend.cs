using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    /*public static void WelcomeReceived()
    {
        using(Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(UIManager.instance.usernameField.text);
            _packet.Write(Client.instance.uniqueUserCode);

            SendTCPData(_packet);
        }

        Message message = Message.Create(MessageSendMode.Unreliable, ClientToServerId.welcomeReceived);
        message.AddUShort(_player.Id);
        message.AddQuaternion(_player.movementController.root.rotation);

        NetworkManager.Instance.Client.Send(message);
    }*/

    public static void SetInputs(float inputSpeed, bool[] _otherInputs, Quaternion _rotation)
    {
        Message message = Message.Create(MessageSendMode.Unreliable, ClientToServerId.playerMovement);
        message.AddFloat(inputSpeed);
        message.AddBools(_otherInputs, false);
        message.AddQuaternion(_rotation);

        NetworkManager.Instance.Client.Send(message);
    }

    public static void PlayerReady(bool _isReady)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.playerReady);
        message.AddBool(_isReady);

        NetworkManager.Instance.Client.Send(message);
    }

    public static void TryStartGame()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.tryStartGame);

        NetworkManager.Instance.Client.Send(message);
    }

    public static void SetPlayerColour(Color _colour, bool _isSeekerColour)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.setPlayerColour);
        message.AddColour(_colour);
        message.AddBool(_isSeekerColour);

        NetworkManager.Instance.Client.Send(message);
    }

    public static void PickupSelected(ushort _pickupId)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.pickupSelected);
        message.AddUShort(_pickupId);

        NetworkManager.Instance.Client.Send(message);
    }

    public static void ItemUsed(bool sendDirection, Vector3 direction)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.itemUsed);
        message.Add(sendDirection);
        message.Add(direction);

        NetworkManager.Instance.Client.Send(message);
    }
    public static void ItemUsed()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.itemUsed);
        message.Add(false);

        NetworkManager.Instance.Client.Send(message);
    }

    public static void GameRulesChanged(GameRules gameRules)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.gameRulesChanged);
        message.AddGameRules(gameRules);

        NetworkManager.Instance.Client.Send(message);
    }

    public static void Command(string command)
    {
        Debug.Log("Command");
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.command);
        message.AddString(command);

        NetworkManager.Instance.Client.Send(message);
    }
}
