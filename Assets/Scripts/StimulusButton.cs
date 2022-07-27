using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StimuliButton : MonoBehaviour
{
    public string buttonName = "red";
    // Start is called before the first frame update

    public void OnClick()
    {
        // get current timestamp
        var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        // raise event on main script to save this data
        string StimulusEvent = string.Format("{0}, {1}", unixTimestamp, buttonName);
    }
}
