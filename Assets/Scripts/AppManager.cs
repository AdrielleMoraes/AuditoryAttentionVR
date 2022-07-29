using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/*
 * This file contains the experiment protocol with the following features:
 *  1. Start recording Eye Gaze
 *  2. Starts the Tutorial Phase
 */


[RequireComponent(typeof(ViveSR.anipal.Eye.SRanipal_Eye_Framework))]
public class AppManager : MonoBehaviour
{
    [Header("Data collection Settings")]
    [SerializeField] private bool saveData;
    public string filename;
    public int participantID;
    private static StreamWriter writer;

    [Header("Protocol Settings")]
    [SerializeField] private float firstWaitTime;
    [SerializeField] private int nRep; // total number of trials per category
    [SerializeField] private List<GameObject> AuditoryStimuli; // list of auditory stimuli
    [SerializeField] private List<GameObject> AudioVisualStimuli; // list of AV stimuli
    GameObject[] experimentTrials;

    // interval in between stimuli in miliseconds
    [SerializeField] [Range(0, 3000)] private int intervalMin;
    [SerializeField] [Range(3000, 10000)] private int intervalMax;
    System.Random rnd;

    int currentIndex;
    bool playNext = true;

    // timer settings
    bool timerOn = true;
    float targetTime;


    void Start()
    {
        if (saveData)
        {
            StartEyeGazeData(); // enable eye tracker data collection
            StartControllerData(); // controller data
            CreateDataFile(); // stimuli data
        }
        else
        {
            Debug.LogWarning("No data is being collected");
        }

        rnd = new System.Random(); // use this to generate random values

        // generate array with stimuli
        FillTrialsArray();

        // put a timer here before the experiment begins
        playNext = true;
        targetTime = firstWaitTime;
    }

    void CreateDataFile()
    {
        // get current timestamp
        var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // create directory to store files
        Directory.CreateDirectory("Data/Trials");

        // initialize txt file
        writer = new StreamWriter(string.Format("Data/Trials/{0}{1}{2}_{3}.txt", participantID, "PERFORMANCE", filename, unixTimestamp));

        // write header to the file
        string header = "timestamp, id, stimulus_type, name, duration, intensity";
        writer.WriteLine(header);
    }
    void SaveData(string input)
    {
        writer.WriteLine(input); 
    }
    private void Update()
    {
        if (timerOn)
        {
            targetTime -= Time.deltaTime;

            if (targetTime <= 0.0f)
            {
                timerOn = false;
                //reset timer
                targetTime = firstWaitTime;

                // play trial after timer is finished
                StartTrial();
            }

            return; // always stop here if timer is on
        }

        if (playNext)
        {
            StartTrial();
        }
    }

    void FillTrialsArray()
    {
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

    void StartTrial()
    {
        playNext = false;
        StartCoroutine(PlayTrial());
    }
    IEnumerator PlayTrial()
    {
        GameObject currentObject = null;
        // get current timestamp
        var trial_start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        if (experimentTrials.Length <= 0 || currentIndex > experimentTrials.Length)
        {
            Debug.Log("No trial gameobjects in the list");
        }
        else
        {
            if (experimentTrials[currentIndex] != null)
            {
                currentObject = experimentTrials[currentIndex];
                Instantiate(currentObject);
            }
            currentIndex++; // move pointer forward
        }

        int interval = rnd.Next(intervalMin, intervalMax);
        yield return new WaitForSeconds(interval/1000);

        // enable next trial
        playNext = true;

        //save data
        var trial_duration = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - trial_start;
        string data_row = "";
        if (currentObject == null)
        {
            data_row = string.Format("{0},{1},{2},{3},{4}",
            trial_start, currentIndex, "null", "null", trial_duration, "null");
        }
        else
        {
            TrialInfo objectInfo = currentObject.GetComponent<TrialInfo>();
            string stimulus_type = objectInfo.Type.ToString();
            string stimulus_name = objectInfo.name;
            int stimulus_intensity = objectInfo.Intensity;
            data_row = string.Format("{0},{1},{2},{3},{4}",
            trial_start, currentIndex, stimulus_type, stimulus_name, trial_duration, stimulus_intensity);
        }

        if (saveData)
        {
            SaveData(data_row);
        }

    }

    void OnApplicationQuit()
    {
        try
        {
            // end writing to file
            writer.WriteLine(DateTimeOffset.Now.ToUnixTimeMilliseconds());
            writer.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
}
