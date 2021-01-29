using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ErrorResponseCode
{
    NotAllPlayersReady = 101
}

public class ErrorResponseHandler
{
    public static Dictionary<int, Action> errorResponseHandlers;

    public static void InitialiseErrorResponseData()
    {
        errorResponseHandlers = new Dictionary<int, Action>()
        {
            { (int)ErrorResponseCode.NotAllPlayersReady, NotAllPlayersReady }
        };
    }

    public static void HandleErrorResponse(ErrorResponseCode errorResponseCode)
    {
        errorResponseHandlers[(int)errorResponseCode]?.Invoke();
    }

    private static void NotAllPlayersReady()
    {
        Debug.Log("Game start failed: Not all players ready!");
        // do something to show this
    }
}
