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
            // move on to next message
            Start_Coroutine();

            isTriggerEnabled = false;
        }
    }

    void StartExperiment()
    {
        SceneManager.LoadScene("MainScene");
    }

}
