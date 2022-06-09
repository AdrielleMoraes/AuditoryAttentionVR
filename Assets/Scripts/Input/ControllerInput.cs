using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

// use this class to get user input from the controller
// remember to always enable and disable input actions
// each input action represents a button/item from the controller 

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Camera))]
public class ControllerInput : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float rayDefaultLenght = 5.0f;
    private LineRenderer lineRenderer = null;


    // get one of the buttons from the VR controller
    private XRInputActions xrInputActions;
    private InputAction clickTrigger = null;
    private InputAction clickTrackpad = null;

     void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        xrInputActions = new XRInputActions();
    }
    private void Update()
    {
        UpdateLine();
    }

    private void UpdateLine()
    {
        // use default lenght
        float targetLenght = rayDefaultLenght;

        // create raycast
        RaycastHit hit = CreateRayCast(targetLenght);

        Vector3 endPosition = transform.position + (transform.forward * targetLenght);

        //based on hit

        if (hit.collider !=null)
        {
            endPosition = hit.point;
        }

        // set line renderer
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPosition);
    }

    private RaycastHit CreateRayCast(float lenght)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, rayDefaultLenght);

        return hit;

    }

    private void OnEnable()
    {
        clickTrigger = xrInputActions.XRIRightHandInteraction.Activate;
        clickTrackpad = xrInputActions.XRIRightHandInteraction.UIPress;
        
        clickTrigger.Enable();
        clickTrackpad.Enable();


        clickTrigger.performed += XRTriggerDown;
        clickTrigger.canceled += XRTriggerUp;

        clickTrackpad.performed += DoChangeThing;
        clickTrackpad.canceled += DoReleasedThing;
    }

    private void OnDisable()
    {
        clickTrigger.Disable();
        clickTrackpad.Disable();
    }

    private void OnDestroy()
    {

        clickTrigger.performed -= DoChangeThing;
        clickTrigger.canceled -= DoReleasedThing;

        clickTrackpad.performed += XRTriggerDown;
        clickTrackpad.canceled += XRTriggerUp;
    }

    private void XRTriggerDown(InputAction.CallbackContext context)
    {
        lineRenderer.enabled = true;
    }

    private void XRTriggerUp(InputAction.CallbackContext context)
    {
        lineRenderer.enabled = false;
    }


    private void DoChangeThing(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //print(context.ReadValue<float>());
            print("ON");
        }
    }

    private void DoReleasedThing(InputAction.CallbackContext context)
    {
            print("Released");
    }

}