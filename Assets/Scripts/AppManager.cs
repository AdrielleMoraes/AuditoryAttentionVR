using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This file contains the experiment protocol with the following features:
 *  1. Start recording Eye Gaze
 *  2. Starts the Tutorial Phase
 */


[RequireComponent(typeof(ViveSR.anipal.Eye.SRanipal_Eye_Framework))]
[RequireComponent(typeof(PerformanceManager))]
public class AppManager : MonoBehaviour
{
    [Header("Data collection Settings")]
    [SerializeField] private bool saveData;
    public string filename;
    public int participantID;

    [Header("Protocol Settings")]
    [SerializeField] private int nTrials;
    [SerializeField] private GameObject singleTrial;
    [SerializeField] private int nSeries;


    void Awake()
    {
        
        if (saveData)
        {
            // enable eye tracker data collection
            StartEyeGazeData();

            // enable performance data
            gameObject.GetComponent<PerformanceManager>().StartRecording();
        }
        else
        {
            Debug.LogWarning("No data is being collected");
        }
    }


    void Start()
    {
        //InvokeRepeating("StartTrial", 2.0f, 5.0f);
    }


    void StartEyeGazeData()
    {
        // enable eye gaze data
        GetComponent<ViveSR.anipal.Eye.SRanipal_Eye_Framework>().EnableEyeDataCallback = true;
        GetComponent<ViveSR.anipal.Eye.SRanipal_Eye_Framework>().EnableEye = true;
        gameObject.AddComponent<EyeTracker_DataCollection>();           
    }


    public void StartTrial()
    {
        if (singleTrial == null)
        {
            Debug.Log("No trial gameobject assigned");
            return;
        }
        Instantiate(singleTrial);
    }
}
