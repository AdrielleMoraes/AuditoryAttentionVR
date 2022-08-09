using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



[RequireComponent(typeof(ViveSR.anipal.Eye.SRanipal_Eye_Framework))]
public class TutorialManager : MonoBehaviour
{

    [Header("Data collection Settings")]
    [SerializeField] private bool saveData;
    public string filename;
    public int participantID;

    [Header("Controller Settings")]
    public GameObject controller;
    public GameObject controllerTrigger;
    public Material highlightMaterial;
    public Material regularMaterial;
    private bool isTriggerEnabled = false;

    [Header("GUI Settings")]
    public TextMeshProUGUI textDisplay;
    public string[] displayMessages;
    // duration of the first message
    public float targetTime = 10.0f;
    private int currentMessage = 0;
    
    private AudioSource tutorialAudio;

    private IEnumerator coroutine;

    private int countdownTime = 5;
    void Awake()
    {
        Debug.Log("Tutorial Phase Started.");

        if (saveData)
        {
            // enable eye tracker and controller data collection
            SetupDataCollection();           
        }
        else
        {
            Debug.LogWarning("Tutorial Phase: No data is being collected");
        }

        tutorialAudio = GetComponent<AudioSource>();
    }

    void SetupDataCollection()
    {
        // enable eye gaze data
        GetComponent<ViveSR.anipal.Eye.SRanipal_Eye_Framework>().EnableEyeDataCallback = true;
        GetComponent<ViveSR.anipal.Eye.SRanipal_Eye_Framework>().EnableEye = true;
        gameObject.AddComponent<EyeTracker_DataCollection>();
        gameObject.AddComponent<ControllerData>();
    }


    private void OnEnable()
    {
        XRPointer.OnTriggerClicked += onClickTrigger;
    }

    private void OnDisable()
    {
        XRPointer.OnTriggerClicked -= onClickTrigger;
    }

    void Start()
    {
        // set first message
        PlayNextMessage();

        //hide controller for now
        controller.SetActive(false);
        controller.transform.parent.gameObject.GetComponent<XRPointer>().animate = true;
        controller.transform.parent.gameObject.GetComponent<XRPointer>().showBeam = false;

        // Start coroutine.
        Start_Coroutine();
    }

    public void Start_Coroutine()
    {
        // Start coroutine.
        coroutine = WaitAndPrint(targetTime);
        StartCoroutine(coroutine);
    }

    private IEnumerator WaitAndPrint(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (currentMessage <= 2)
        {
            // move on to next message
            Start_Coroutine();
        }
        else if (currentMessage == 3)
        {

            controller.SetActive(true);
            controllerTrigger.GetComponent<Renderer>().material = highlightMaterial;
            isTriggerEnabled = true;

            // play sound
            tutorialAudio.Play();


        }
        else if (currentMessage == 4)
        {
            controllerTrigger.GetComponent<Renderer>().material = regularMaterial;
        }

        PlayNextMessage();
    }

    public void PlayNextMessage()
    {
        //jump to next message if available
        if (currentMessage < displayMessages.Length)
        {
            textDisplay.text = displayMessages[currentMessage];           
            currentMessage++;
        }

        // run this if it is the last message
        if (currentMessage == displayMessages.Length)
        {
            StartExperiment();
        }      
    }

    void onClickTrigger()
    {
        // check if trigger can be enabled now
        if (isTriggerEnabled)
        {
            // move on to next message
            Start_Coroutine();

            isTriggerEnabled = false;
        }
    }

    void StartExperiment()
    {
        // Start coroutine.
        IEnumerator endCoroutine = CountdownRoutine(1);
        StartCoroutine(endCoroutine);
        
    }

    private IEnumerator CountdownRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        
        if (countdownTime <= 0)
        {
            SceneManager.LoadScene("MainScene");
        }
        textDisplay.text = displayMessages[currentMessage-1] + countdownTime + " seconds";
        countdownTime--;

        StartExperiment();
    }

}


