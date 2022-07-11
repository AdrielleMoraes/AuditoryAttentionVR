using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerData : MonoBehaviour
{
    // get the interface with the VR controller buttons
    public XRInputActions xrInputActions;

    public InputActionReference refAction;

    void Awake()
    {
        // user input
        xrInputActions = new XRInputActions();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        foreach (var userAction in xrInputActions)
        {
            userAction.Enable();
            userAction.performed += onPerformed;
            userAction.canceled += onCanceled;
        }
    }

    void OnDisable()
    {
        foreach (var userAction in xrInputActions)
        {
            userAction.Disable();
        }
    }

    void OnDestroy()
    {
        foreach (var userAction in xrInputActions)
        {
            userAction.performed -= onPerformed;
            userAction.canceled -= onCanceled;
        }
    }

    private void onPerformed(InputAction.CallbackContext context)
    {
        Debug.Log(string.Format("{0} Performed", context.action));
    }

    private void onCanceled(InputAction.CallbackContext context)
    {
        Debug.Log(string.Format("{0} Canceled", context.action));
    }
}
