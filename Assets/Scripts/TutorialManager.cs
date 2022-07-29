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
    public GameObject controllerTrackpad;
    public Material highlightMaterial;
    public Material regularMaterial;
    private bool isTriggerEnabled = false;
    private bool isTrackpadEnabled = false;

    [Header("GUI Settings")]
    public TextMeshProUGUI textDisplay;
    public Button startButton;
    public string[] displayMessages;
    // duration of the first message
    public float targetTime = 10.0f;
    private int currentMessage = 0;




    private IEnumerator coroutine;
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
    }

    private void OnEnable()
    {
        XRPointer.OnTriggerClicked += onClickTrigger;
        XRPointer.OnTrackpadClicked += onClickTrackpad;
    }

    private void OnDisable()
    {
        XRPointer.OnTriggerClicked -= onClickTrigger;
        XRPointer.OnTrackpadClicked -= onClickTrackpad;
    }

    void Start()
    {
        // set first message
        PlayNextMessage();

        //hide controller for now
        controller.SetActive(false);
        controller.transform.parent.gameObject.GetComponent<XRPointer>().animate = true;
        controller.transform.parent.gameObject.GetComponent<XRPointer>().showBeam = false;

        // disable start button. It will be enabled after the tutorial finishes
        startButton.enabled = false;
        startButton.onClick.AddListener(StartButtonOnClick);

        // Start coroutine.
        Start_Coroutine();
    }

    void Start_Coroutine()
    {
        // Start coroutine.
        coroutine = WaitAndPrint(targetTime);
        StartCoroutine(coroutine);
    }

    void SetupDataCollection()
    {
        // enable eye gaze data
        GetComponent<ViveSR.anipal.Eye.SRanipal_Eye_Framework>().EnableEyeDataCallback = true;
        GetComponent<ViveSR.anipal.Eye.SRanipal_Eye_Framework>().EnableEye = true;
        gameObject.AddComponent<EyeTracker_DataCollection>();
        gameObject.AddComponent<ControllerData>();
    }


    private IEnumerator WaitAndPrint(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        // enable controller after first message shows on screen
        if (currentMessage == 1)
        {
            controller.SetActive(true);
            controllerTrigger.GetComponent<Renderer>().material = highlightMaterial;
            isTriggerEnabled = true;
        }
        else if (currentMessage == 2)
        {
            controllerTrigger.GetComponent<Renderer>().material = regularMaterial;
            // enable selection with trackpad
            isTrackpadEnabled = true;
            controllerTrackpad.GetComponent<Renderer>().material = highlightMaterial;
        }

        else if (currentMessage == 3)
        {
            controllerTrackpad.GetComponent<Renderer>().material = regularMaterial;
        }

        PlayNextMessage();
    }

    void PlayNextMessage()
    {
        //jump to next message if available
        if (currentMessage < displayMessages.Length)
        {
            textDisplay.text = displayMessages[currentMessage];           
            currentMessage++;
        }
    }

    void onClickTrigger()
    {
        // check if trigger can be enabled now
        if (isTriggerEnabled)
        {
            // enable laser beam
            controller.transform.parent.gameObject.GetComponent<XRPointer>().showBeam = true;
            controller.transform.parent.gameObject.GetComponent<XRPointer>().SetVisibility(true);

            // move on to next message
            Start_Coroutine();

            isTriggerEnabled = false;
        }
    }

    void onClickTrackpad()
    {
        if (isTrackpadEnabled)
        {
            // move on to next message
            Start_Coroutine();

            isTrackpadEnabled = false;
        }
    }

    void DisplayFinalMessage()
    {
        // display last message in array
        textDisplay.text = displayMessages[displayMessages.Length -1];

        // enable start button
        startButton.enabled = true;
    }

    void StartButtonOnClick()
    {
        SceneManager.LoadScene("MainScene");
    }

}
