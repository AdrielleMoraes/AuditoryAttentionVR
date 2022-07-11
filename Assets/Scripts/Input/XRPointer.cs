using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;




// use this class to get user input from the controller
// remember to always enable and disable input actions
// each input action represents a button/item from the controller 

[RequireComponent(typeof(LineRenderer))]
public class XRPointer : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float rayDefaultLenght = 10.0f;

    private Vector3[] linePoints;

    private LineRenderer lineRenderer = null;
    private GameObject currentSelection = null;
    private GameObject previousSelection = null;



    /// <summary>
    /// Controller Buttons
    /// </summary>
    /// 

    // get one of the buttons from the VR controller
    public XRInputActions xrInputActions;

    private InputAction clickTrigger = null;
    public InputAction clickTrackpad = null;

    

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        CreateLineRenderer();


        // user input
        xrInputActions = new XRInputActions();

    }
    private void Update()
    {
        UpdateLine();
    }

    public void SetVisibility(bool visibility)
    {
        lineRenderer.enabled = visibility;
    }

    private void CreateLineRenderer()
    {
        // Initialise the line renderer
        linePoints = new Vector3[2];

        //set start and end points
        linePoints[0] = Vector3.zero;
        linePoints[1] = transform.position + new Vector3(0, 0, rayDefaultLenght);

        lineRenderer.SetPositions(linePoints);
        lineRenderer.enabled = false;
    }
    private void UpdateLine()
    {
        // create raycast
        Ray ray = new Ray(transform.position, transform.forward);

        RaycastHit hit;

        // hit something
        if(Physics.Raycast(ray, out hit, rayDefaultLenght))
        {
            linePoints[1] = Vector3.zero + new Vector3(0f, 0f, hit.distance);

            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.green;

            currentSelection = hit.collider.gameObject;
        }
        else
        {
            linePoints[1] = Vector3.zero + new Vector3(0f, 0f, rayDefaultLenght);
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;
            currentSelection = null;
        }

        // set line renderer
        lineRenderer.SetPositions(linePoints);
    }


    void OnEnable()
    {
        clickTrigger = xrInputActions.XRIRightHandInteraction.Activate;
        clickTrackpad = xrInputActions.XRIRightHandInteraction.UIPress;

        clickTrigger.Enable();
        clickTrackpad.Enable();

        clickTrackpad.performed += onTrackPadDown;
        clickTrackpad.canceled += onTrackPadUP;

        clickTrigger.performed += onXRTriggerDown;
        clickTrigger.canceled += onXRTriggerUp;
    }

    void OnDisable()
    {
        clickTrigger.Disable();
        clickTrackpad.Disable();
    }

    void OnDestroy()
    {

        clickTrigger.performed -= onTrackPadDown;
        clickTrigger.canceled -= onTrackPadUP;

        clickTrackpad.performed += onXRTriggerDown;
        clickTrackpad.canceled += onXRTriggerUp;
    }

    private void onXRTriggerDown(InputAction.CallbackContext context)
    {
        SetVisibility(true);
    }

    private void onXRTriggerUp(InputAction.CallbackContext context)
    {
        SetVisibility(false);
    }


    private void onTrackPadDown(InputAction.CallbackContext context)
    {
            previousSelection = currentSelection;
    }

    private void onTrackPadUP(InputAction.CallbackContext context)
    {
        if (currentSelection != null)
        {
            if (previousSelection == currentSelection)
            {
                Debug.Log("click");
                // focusing on a UI object
                if (currentSelection.tag == "UIButton")
                {
                    print("Focus on UI");
                    currentSelection.GetComponent<Button>().onClick.Invoke();

                }
                else
                    print("Focus outside UI");
            }

            else
                Debug.Log("misclick");

            previousSelection = null;
        }
        
    }

}