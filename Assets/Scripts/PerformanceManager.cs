using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Runtime.InteropServices;
using System.IO;
using System;


public static class SelectionType
{
    public const int LessSaliency = -1;
    public const int SameSaliency = 0;
    public const int MoreSaliency = 1;
}

public class PerformanceManager : MonoBehaviour
{

    private bool saveData = false;
    private static StreamWriter writer;
    
    public void StartRecording(string filename, int participantID)
    {
        saveData = true;

        // get current timestamp
        var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // create directory to store files
        Directory.CreateDirectory("Data/Performance");

        // initialize txt file
        writer = new StreamWriter(string.Format("Data/Performance/{0}{1}{2}_{3}.txt", participantID, "PERFORMANCE", filename, unixTimestamp));

        string header = "timestamp, time_complete, trial, trial_type, user_response";

        // first row indicates when data collection started
        writer.WriteLine(DateTimeOffset.Now.ToUnixTimeMilliseconds());
        writer.WriteLine(header);

    }


    public void onLessSaliency()
    {
        if(saveData)
            writePerformance(SelectionType.LessSaliency);
        Debug.Log("Less clicked!");
    }

    public void onSameSaliency()
    {
        if (saveData)
            writePerformance(SelectionType.SameSaliency);
        Debug.Log("Same clicked!");
    }

    public void onMoreSaliency()
    {
        if (saveData)
            writePerformance(SelectionType.MoreSaliency);
        Debug.Log("More clicked!");
    }

    private static void writePerformance(int user_response)
    {
        // timestamps
        long sample_timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        long time_complete = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        // Trial information
        int selectionType = 0;
        int userResponse = user_response;
        int trialNumber = 0;

        string data_row = string.Format("{0},{1},{2},{3},{4}",
            sample_timestamp, time_complete, selectionType, userResponse, trialNumber);

        writer.WriteLine(data_row);
    }
}
