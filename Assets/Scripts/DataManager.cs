using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(ViveSR.anipal.Eye.SRanipal_Eye_Framework))]
public class DataManager : MonoBehaviour
{

    [Header("Data collection Settings")]
    [SerializeField] private bool saveData;
    public string filename;
    public int participantID;
    

    // Start is called before the first frame update
    void Start()
    {
        if (saveData)
        {
            StartEyeGazeData(); // enable eye tracker data collection
            StartControllerData(); // controller data
            
            // trigger this on app manager
            GetComponent<AppManager>().CreateDataFile(participantID, filename); // stimuli data
        }
        else
        {
            Debug.LogWarning("No data is being collected");
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
}
