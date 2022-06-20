using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class UIInputModule : MonoBehaviour
{

    // get one of the buttons from the VR controller
    private XRInputActions xrInputActions;
    private InputAction clickTrigger = null;
    private InputAction clickTrackpad = null;


    // Start is called before the first frame update
    void Awake()
    {
        xrInputActions = new XRInputActions();
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

    }

    private void onXRTriggerUp(InputAction.CallbackContext context)
    {

    }


    private void onTrackPadDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //print(context.ReadValue<float>());
            print("ON");
        }
    }

    private void onTrackPadUP(InputAction.CallbackContext context)
    {
        print("Released");
    }
}
