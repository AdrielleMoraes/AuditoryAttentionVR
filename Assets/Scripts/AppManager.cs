using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;



public class AppManager : MonoBehaviour
{
    [Header("Data collection Settings")]
    private static StreamWriter writer;

    [Header("Protocol Settings")]
    [SerializeField] private string pathToKeywords1;
    [SerializeField] private string pathToKeywords2;
    [SerializeField] private int nRep; // total number of trials per category
    [SerializeField] private List<GameObject> AuditoryStimuli; // list of auditory stimuli
    [SerializeField] private List<GameObject> AudioVisualStimuli; // list of AV stimuli
    [SerializeField] [Range(0, 2500)] private int timeBeforeStimuli = 0; // time before stimuli in miliseconds
    GameObject[] experimentTrials;

    [Header("Target")]
    [SerializeField] GameObject targetSpeaker;
    public AudioClip secondClip;


    float targetTime = 0;
    List<float> keywordTimestamps;
    System.Random rnd;
    private bool timerOn = false;

    int currentIndex;

    void Start(){
        
        rnd = new System.Random(); // use this to generate random values

        // generate array with stimuli
        FillTrialsArray();

        // list with timestamps for keywords
        readCSV(pathToKeywords1); //@"Assets\GameResources\Auditive\Stories\keywords_The House that Jack Built.txt"
    }

    void readCSV(string filePath)
    {
        float previousPos = 0;
        using (var reader = new StreamReader(filePath))
        {
            keywordTimestamps = new List<float>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split('\t');

                var seconds = values[0].Split(':');
                float keyword_pos = float.Parse(seconds[0]) * 60 + float.Parse(seconds[1]);

                keywordTimestamps.Add(keyword_pos - previousPos);

                if (keywordTimestamps.Count > 0)
                {
                    previousPos = keyword_pos;
                }

                
            }
            targetTime = keywordTimestamps.ElementAt(0) - rnd.Next(timeBeforeStimuli) / 1000;
            keywordTimestamps.RemoveAt(0);
        }
    }

    public void CreateDataFile(int participantID, string filename)
    {
        // get current timestamp
        var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // create directory to store files
        Directory.CreateDirectory("Data/Trials");

        // initialize txt file
        writer = new StreamWriter(string.Format("Data/Trials/{0}{1}{2}_{3}.txt", participantID, "PERFORMANCE", filename, unixTimestamp));

        // write header to the file
        string header = "timestamp, id, stimulus_type, name, duration";
        writer.WriteLine(header);
    }

    void SaveData(string input){
        if (writer == null)
        {
            return;
        }
        writer.WriteLine(input); 
    }


    public void startPartOne()
    {
        targetSpeaker.SetActive(true);
        targetSpeaker.GetComponent<AudioSource>().Play();
        timerOn = true;
    }

    public void startPartTwo()
    {
        // generate array with stimuli
        FillTrialsArray();

        // list with timestamps for keywords
        readCSV(pathToKeywords2);

        targetSpeaker.SetActive(true);
        targetSpeaker.GetComponent<AudioSource>().clip = secondClip;
        targetSpeaker.GetComponent<AudioSource>().Play();
        timerOn = true;
    }
    private void Update(){
        if (timerOn)
        {
            targetTime -= Time.deltaTime;
            if (targetTime <= 0.0f)
            {
                timerOn = false;
                // play first trial after timer is finished
                StartTrial();  
            }

            return; // always stop here if timer is on
        }

        // reaching the end of experiment
        if(currentIndex >= experimentTrials.Length)
        {
            // pause trials and return message and timer
            Debug.Log("Reached middle of experiment");

            Application.Quit();
        }          
    }

    void FillTrialsArray(){
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

    void StartTrial(){
        StartCoroutine(PlayTrial());
    }

    IEnumerator PlayTrial(){
        GameObject currentObject = null;
        // get current timestamp
        var trial_start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        if (experimentTrials.Length <= 0 || keywordTimestamps.Count <= 0)
        {
            Debug.Log("No trial gameobjects in the list");

            // move to mid phase
            GetComponent<TutorialManager>().StartMidTutorial();

            targetSpeaker.SetActive(false);
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

        // interval is based on the imported csv file
        targetTime = keywordTimestamps.ElementAt(0) - rnd.Next(timeBeforeStimuli) / 1000;
        keywordTimestamps.RemoveAt(0);

        yield return new WaitForSeconds(targetTime);

        
        //save data
        var trial_duration = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - trial_start;
        string data_row;
        if (currentObject == null)
        {
            data_row = string.Format("{0},{1},{2},{3}",
            trial_start, currentIndex, "null", "null", trial_duration);
        }
        else
        {
            TrialInfo objectInfo = currentObject.GetComponent<TrialInfo>();
            string stimulus_type = objectInfo.Type.ToString();
            string stimulus_name = objectInfo.name;
            data_row = string.Format("{0},{1},{2},{3}",
            trial_start, currentIndex, stimulus_type, stimulus_name, trial_duration);
        }

        SaveData(data_row);

        // enable next trial
        StartTrial();
    }

    void OnApplicationQuit(){
        if (writer == null)
        {
            return;
        }
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
