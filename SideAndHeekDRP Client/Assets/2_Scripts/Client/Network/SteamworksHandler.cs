using Steamworks;
using System;
using System.IO;
using UnityEngine;

public class SteamworksHandler : MonoBehaviour
{
    private void Start()
    {
        FetchSteamID();
    }

    private void FetchSteamID()
    {
        if (SteamManager.Initialized)
        {
            ulong SteamUserID = SteamUser.GetSteamID().m_SteamID;

            Debug.Log(SteamUserID);
        }
    }
}
