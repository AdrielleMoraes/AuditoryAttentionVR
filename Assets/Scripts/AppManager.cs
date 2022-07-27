using System;
using System.Linq;
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
    [SerializeField] private int nRep; // total number of trials per category
    [SerializeField] private List<GameObject> AuditoryStimuli; // list of auditory stimuli
    [SerializeField] private List<GameObject> AudioVisualStimuli; // list of AV stimuli
    public GameObject[] experimentTrials;

    // interval in between stimuli in seconds
    [SerializeField] [Range(0, 5)] private int intervalMin;
    [SerializeField] [Range(5, 10)] private int intervalMax;

    int currentIndex;
    bool playNext = true;

    void Awake()
    {        
        if (saveData)
        {
            // enable eye tracker data collection
            StartEyeGazeData();
            StartControllerData();

            // enable performance data
            gameObject.GetComponent<PerformanceManager>().StartRecording(filename, participantID);
        }
        else
        {
            Debug.LogWarning("No data is being collected");
        }
    }


    void Start()
    {       
        FillTrialsArray();
        StartTrial();
    }

    private void Update()
    {
        if (playNext)
        {
            StartCoroutine(PlayTrial());
        }
    }

    void FillTrialsArray()
    {
        System.Random rnd = new System.Random(); // randomise

        // first concatenate all types
        GameObject[] groupedTrials = AudioVisualStimuli.Concat(AuditoryStimuli).ToArray();

        // create null elements
        GameObject[] nullElements = new GameObject[AuditoryStimuli.Count + groupedTrials.Length];

        // merge all together
        groupedTrials.CopyTo(nullElements, AuditoryStimuli.Count);

        // now create a new array to accommodate copied stimuli
        experimentTrials = new GameObject[nRep*nullElements.Length];

        for (int i = 0; i <nRep; i++)
        {
            //Shuffle items in original array           
            nullElements = nullElements.OrderBy(x => rnd.Next()).ToArray();

            // merge all together
            nullElements.CopyTo(experimentTrials, i* nullElements.Length);
        }

    }


    void StartEyeGazeData()
    {
        // enable eye gaze data
        GetComponent<ViveSR.anipal.Eye.SRanipal_Eye_Framework>().EnableEyeDataCallback = true;
        GetComponent<ViveSR.anipal.Eye.SRanipal_Eye_Framework>().EnableEye = true;
        gameObject.AddComponent<EyeTracker_DataCollection>();           
    }

    void StartControllerData()
    {
        gameObject.AddComponent<ControllerData>();
    }


    public void StartTrial()
    {


        StartCoroutine(PlayTrial());
    }

    IEnumerator PlayTrial()
    {
        playNext = false;
        if (playNext)
        {

            if (experimentTrials.Length <= 0 || currentIndex > experimentTrials.Length)
            {
                Debug.Log("No trial gameobjects in the list");
            }
            else
            {
                if (experimentTrials[currentIndex] != null)
                {
                    Instantiate(experimentTrials[currentIndex]);
                }

                currentIndex++;
            }
        }
        yield return new WaitForSeconds(2);
        playNext = true;
    }
}
