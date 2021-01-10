using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ErrorResponseCode
{
    NotAllPlayersReady = 101,
    PlayerLeftDuringGameStart = 102,
    PlayerLeftDuringGamePlay = 103,
}

public class ErrorResponseHandler
{
    public static Dictionary<int, Action> errorResponseHandlers;

    public static void InitialiseErrorResponseData()
    {
        errorResponseHandlers = new Dictionary<int, Action>()
        {
            { (int)ErrorResponseCode.NotAllPlayersReady, NotAllPlayersReady },
            { (int)ErrorResponseCode.PlayerLeftDuringGameStart, PlayerLeftDuringGameStart }
        };
    }

    public static void HandleErrorResponse(ErrorResponseCode errorResponseCode)
    {
        errorResponseHandlers[(int)errorResponseCode]?.Invoke();
    }

    private static void NotAllPlayersReady()
    {

    }

    private static void PlayerLeftDuringGameStart()
    {

    }
}
