using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This file contains the experiment protocol with the following features:
 *  1. Start recording Eye Gaze
 *  2. Starts the Tutorial Phase
 */

[RequireComponent(typeof(ViveSR.anipal.Eye.SRanipal_Eye_Framework))]
public class AppManager : MonoBehaviour
{
    [Header("Eye Tracking Settings")]
    [SerializeField] private bool saveData;
    [SerializeField] private string eyeFileName;

    [Header("Protocol Settings")]
    [SerializeField] private int nTrials;
    [SerializeField] private GameObject singleTrial;
    [SerializeField] private int nSeries;

    /*
     * Tasks:
     * 
     *  Generate file with performance data
     *  Create experiment protocol
     *  Start turorial phase
     *  Move to testing phase

     *  
     */
    // Start is called before the first frame update
    void Awake()
    {
        // enable eye tracker data collection
        if (saveData)
        {
            StartEyeGazeData(eyeFileName);
        }

    }



    // Update is called once per frame
    void Start()
    {
        //InvokeRepeating("StartTrial", 2.0f, 5.0f);
    }

    public void StartTrial()
    {
        if(singleTrial == null)
        {
            Debug.Log("No trial gameobject assigned");
            return;
        }    
        Instantiate(singleTrial);
    }

    void StartEyeGazeData(string filename)
    {
        // enable eye gaze data
        GetComponent<ViveSR.anipal.Eye.SRanipal_Eye_Framework>().EnableEyeDataCallback = true;
        GetComponent<ViveSR.anipal.Eye.SRanipal_Eye_Framework>().EnableEye = true;


        EyeTracker_DataCollection eyeTracker = gameObject.AddComponent<EyeTracker_DataCollection>();
        eyeTracker.filename = filename;
        
    }
}
