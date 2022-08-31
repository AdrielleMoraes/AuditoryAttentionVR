using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{

    [Header("Controller Settings")]
    public GameObject controller;
    public GameObject controllerTrigger;
    public Material highlightMaterial;
    public Material regularMaterial;
    private bool isTriggerEnabled = false;

    [Header("GUI Settings")]
    public TextMeshProUGUI textDisplay;
    public TextMeshProUGUI textCount;
    public string[] displayTutorialMessages;
    public string[] displayMidMessages;

    // duration of the first message
    public float targetTime = 10.0f;
    private int currentMessage = 0;
    private int buttonCount = 0;

    private IEnumerator coroutine;

    private int countdownTime = 5;

    void Awake()
    {
        Debug.Log("Tutorial Phase Started");

        textCount.gameObject.SetActive(false);
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
        //hide controller for now
        controller.SetActive(false);
        controller.transform.parent.gameObject.GetComponent<XRPointer>().animate = true;
        controller.transform.parent.gameObject.GetComponent<XRPointer>().showBeam = false;

        // set first message
        PlayNextMessage(true);
    }

    public void PlayNextMessage(bool isTutorial)
    {
        // Start coroutine.
        coroutine = WaitAndPrint(targetTime, isTutorial);
        StartCoroutine(coroutine);
    }

    private IEnumerator WaitAndPrint(float waitTime, bool isTutorial)
    {
        yield return new WaitForSeconds(waitTime);

        if (isTutorial)
        {
            PrintNextTutorialMessage();
        }
        else
        {
            PrintNextMidMessage();
        }
        
    }

    void PrintNextTutorialMessage() { 
        //jump to next message if available
        if (currentMessage < displayTutorialMessages.Length)
        {
            textDisplay.text = displayTutorialMessages[currentMessage];           
            currentMessage++;

            // last message
            if (currentMessage == displayTutorialMessages.Length)
            {
                isTriggerEnabled = false;
                // Start coroutine.
                IEnumerator endCoroutine = CountdownRoutine(1, displayTutorialMessages[currentMessage - 1],1);
                StartCoroutine(endCoroutine);
                return;
            }
            switch (currentMessage)
            {           
                case 4:           
                    controller.SetActive(true);
                    controllerTrigger.GetComponent<Renderer>().material = highlightMaterial;
                    isTriggerEnabled = true;

                    // show message
                    textCount.gameObject.SetActive(true);
                    break;

                case 5:
                    controllerTrigger.GetComponent<Renderer>().material = regularMaterial;
                    break;

                default:
                    // move on to next message
                    PlayNextMessage(true);
                    break;
            }
        }   
    }

    public void PrintNextMidMessage()
    {
        //jump to next message if available
        if (currentMessage <= displayMidMessages.Length)
        {
            textDisplay.text = displayMidMessages[currentMessage];
            currentMessage++;

            // last message
            if (currentMessage == displayMidMessages.Length)
            {
                // Start coroutine.
                IEnumerator endCoroutine = CountdownRoutine(1, displayMidMessages[currentMessage - 1],2);
                StartCoroutine(endCoroutine);
                Debug.Log("End of mid experiment");
                return;
            }

            switch (currentMessage)
            {
                default:
                    // move on to next message
                    PlayNextMessage(false);
                    break;
            }
        }
    }

    public void onClickTrigger()
    {
        // check if trigger can be enabled now
        if (isTriggerEnabled)
        {
            buttonCount++;
            
            // first time that button is pressed
            if (buttonCount == 1)
            {
                textDisplay.text = displayTutorialMessages[currentMessage - 1] + "\n Keep pressing until you reach 10";
            }
            textCount.text = buttonCount.ToString();

            if (buttonCount >= 10)
            {                
                // move on to next message
                PlayNextMessage(true);
            }
        }
    }

    private IEnumerator CountdownRoutine(float waitTime, string message, int testID)
    {
        yield return new WaitForSeconds(waitTime);

        if (countdownTime <= 0)
        {
            Debug.Log("Tutorial Ended");
            textDisplay.gameObject.SetActive(false);
            StartExperiment(testID);
        }

        else
        {
            textCount.gameObject.SetActive(false);
            textDisplay.text = message + "\n It starts in: " + countdownTime + " seconds";
            countdownTime--;

            // Start coroutine.
            IEnumerator endCoroutine = CountdownRoutine(1, message, testID);
            StartCoroutine(endCoroutine);
        }
    }

    void StartExperiment(int testID)
    {
        currentMessage = 0;
        if (testID == 1)
        {
            //GetComponent<AppManager>().startPartOne();

            StartMidTutorial();
        }
        else
        {
            GetComponent<AppManager>().startPartTwo();
        }
    }

    public void StartMidTutorial()
    {
        textDisplay.text = "";// clear messages on screen
        textDisplay.gameObject.SetActive(true);
        PlayNextMessage(false);
    }





}


