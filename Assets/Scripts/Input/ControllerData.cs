using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;


// change this if the actionmaps used are different. These are linked to the vive pro HMD
public static class ActionMapsNames
{
    public const string XRI_HEAD = "XRI Head";  
    public const string XRI_LEFTHAND = "XRI LeftHand";
    public const string XRI_LEFTHAND_INTERACTION = "XRI LeftHand Interaction";
    public const string XRI_LEFTHAND_LOCOMOTION = "XRI LeftHand Locomotion";
    public const string XRI_RIGHTHAND = "XRI RightHand";
    public const string XRI_RIGHTHAND_INTERACTION = "XRI RightHand Interaction";
    public const string XRI_RIGHTHAND_LOCOMOTION = "XRI RightHand Locomotion";
}

public class ControllerData : MonoBehaviour
{

    public string filename;
    private static StreamWriter writer;
    // get the interface with the VR controller buttons
    public XRInputActions xrInputActions;

    [SerializeField]
    private bool CollectHeadData;
    [SerializeField]
    private bool CollectControllerTrackingData;



    void Awake()
    {
        // user input
        xrInputActions = new XRInputActions();
    }

    private void CreateFile()
    {
        var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // create directory to store files
        System.IO.Directory.CreateDirectory("Data/Controller");
        // initialize txt file
        writer = new StreamWriter(string.Format("Data/Controller/{0}{1}_{2}.csv", "CONTROLLER", filename, unixTimestamp));

        string header = "action, control, phase, time";

        // first row indicates when data collection started
        writer.WriteLine(DateTimeOffset.Now.ToUnixTimeMilliseconds());
        writer.WriteLine(header);
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateFile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool FilterActionMap(string ActionMapName)
    {
        if (!CollectHeadData && ActionMapName == ActionMapsNames.XRI_HEAD)
        {
            return true;
        }
        if (!CollectControllerTrackingData)
        {
            if (ActionMapName == ActionMapsNames.XRI_LEFTHAND_INTERACTION || ActionMapName == ActionMapsNames.XRI_RIGHTHAND_INTERACTION)
                return false;

            return true;
        }

        return false;
    }

    void OnEnable()
    {

        foreach (var userAction in xrInputActions)
        {
            if (!FilterActionMap(userAction.actionMap.name))
            {
                //Debug.Log(userAction.name + " enabled.");
                userAction.Enable();
                userAction.performed += printControllerData;
                userAction.canceled += printControllerData;
            }
        }
    }

    private void Release()
    {
        try
        {
            // end writing to file
            writer.WriteLine(DateTimeOffset.Now.ToUnixTimeMilliseconds());
            writer.Close();
        }
        catch
        {
        }
    }

    void OnDisable()
    {
        foreach (var userAction in xrInputActions)
        {
            if (!FilterActionMap(userAction.actionMap.name))
            {
                userAction.Disable();
            }
        }
        Release();
    }

    private void OnApplicationQuit()
    {
        Release();
    }

    void OnDestroy()
    {
        foreach (var userAction in xrInputActions)
        {
            if (!FilterActionMap(userAction.actionMap.name))
            {
                userAction.performed -= printControllerData;
                userAction.canceled -= printControllerData;
            }
        }
    }

    private void WriteToFile(string actionName, string controlType, string phase, string timeStamp)
    {
        string data_row = string.Format("{0},{1},{2},{3}",
            actionName, controlType, phase, timeStamp);

        writer.WriteLine(data_row);
    }

    private void printControllerData(InputAction.CallbackContext context)
    {
       WriteToFile(context.action.name, context.control.ToString(), context.phase.ToString(), context.time.ToString());
    }

}
