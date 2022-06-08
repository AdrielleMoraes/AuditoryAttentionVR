using UnityEngine;
using UnityEngine.InputSystem;

// use this class to get user input from the controller
// remember to always enable and disable input actions
// each input action represents a button/item from the controller 
public class ControllerInput : MonoBehaviour
{
    private XRInputActions xrInputActions;
    public bool printStuff = true;

    // get one of the buttons from the VR controller
    public InputAction clickTrigger = null;


     void Awake()
    {
        xrInputActions = new XRInputActions();
    }
    private void Start()
    {
        
    }

    private void OnEnable()
    {
        clickTrigger = xrInputActions.XRIRightHandInteraction.UIPressValue;
        clickTrigger.Enable();


        clickTrigger.started += DoPressedThing;
        clickTrigger.performed += DoChangeThing;
        clickTrigger.canceled += DoReleasedThing;
    }

    private void OnDisable()
    {
        clickTrigger.Disable();
    }

    private void OnDestroy()
    {
        clickTrigger.started -= DoPressedThing;
        clickTrigger.performed -= DoChangeThing;
        clickTrigger.canceled -= DoReleasedThing;
    }

    private void DoPressedThing(InputAction.CallbackContext context)
    {
        if (printStuff)
            print("Pressed");
    }

    private void DoChangeThing(InputAction.CallbackContext context)
    {
        if (printStuff)
        {
            //print(context.ReadValue<float>());
            print("ON");
        }
    }

    private void DoReleasedThing(InputAction.CallbackContext context)
    {
        if (printStuff)
            print("Released");
    }

}