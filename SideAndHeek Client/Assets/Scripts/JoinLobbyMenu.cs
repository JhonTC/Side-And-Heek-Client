using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
    //[SerializeField] private CarpNetworkManager networkManager = null;

    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private InputField ipAddressInputField = null;
    [SerializeField] private Button joinButton = null;

    private void OnEnable()
    {
        //CarpNetworkManager.OnClientConnected += HandleClientConnected;
        //CarpNetworkManager.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        //CarpNetworkManager.OnClientConnected -= HandleClientConnected;
        //CarpNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
    }

    public void JoinLobby()
    {
        //string ipAddress = ipAddressInputField.text;

        //networkManager.networkAddress = ipAddress;
        //networkManager.StartClient();

        joinButton.interactable = false;
    }

    private void HandleClientConnected ()
    {
        joinButton.interactable = true;
        landingPagePanel.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = false;
        landingPagePanel.SetActive(true);
    }
}
