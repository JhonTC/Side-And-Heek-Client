using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    public static PickupManager instance;

    private delegate void TaskHandler();
    private static Dictionary<TaskCode, TaskHandler> taskHandlers;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        InitialiseTaskData();
    }
    private void InitialiseTaskData()
    {
        taskHandlers = new Dictionary<TaskCode, TaskHandler>()
        {
            { TaskCode.NULL_TASK,  NullTask },
            { TaskCode.TestTaskEasy, TestTaskEasy },
            { TaskCode.TestTaskNormal, TestTaskNormal },
            { TaskCode.TestTaskHard, TestTaskHard },
            { TaskCode.TestTaskExtreme, TestTaskExtreme }
        };
        Debug.Log("Initialised packets.");
    }

    public void HandleTask(TaskCode _code)
    {
        taskHandlers[_code]();
    }

    private void NullTask() 
    { 

    }
    private void TestTaskEasy()
    {

    }
    private void TestTaskNormal()
    {

    }
    private void TestTaskHard()
    {

    }
    private void TestTaskExtreme()
    {

    }


}
