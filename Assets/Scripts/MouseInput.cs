using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseInput : MonoBehaviour
{
    public bool printStuff = true;


    private PlayerInput playerInput;

    private void Start()
    {

        playerInput = GetComponent<PlayerInput>();

        playerInput.onActionTriggered += ButtonOnActionTriggered;
        
    }

    private void ButtonOnActionTriggered(InputAction.CallbackContext context)
    {
        if(context.performed)
            Debug.Log(context);
    }

   
}
