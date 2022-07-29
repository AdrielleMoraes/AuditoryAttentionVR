using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class StimulusButton : MonoBehaviour
{
    public UnityEvent onPressed, onReleased;

    [SerializeField] private string buttonName = "red";
    [SerializeField] private float treshhold = 0.1f;
    [SerializeField] private float deadZone = 0.025f;

    private bool _isPressed;
    private Vector3 _startPos;
    private ConfigurableJoint _joint;


    // Start is called before the first frame update
    private void Start()
    {
        _startPos = transform.localPosition;
        _joint = GetComponent<ConfigurableJoint>();
    }

    private void Update()
    {
        if (!_isPressed && GetValue() + treshhold >= 1)
        {
            Pressed();
        }
        if (_isPressed && GetValue() - treshhold <=0)
        {
            Released();
        }
    }
    public void Pressed()
    {
        _isPressed = true;
        onPressed.Invoke();
        Debug.Log("Button pressed");
    }

    public void Released()
    {
        _isPressed = false;
        onReleased.Invoke();

        // get current timestamp
        var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        // raise event on main script to save this data
        string StimulusEvent = string.Format("{0}, {1}", unixTimestamp, buttonName);
        Debug.Log(StimulusEvent);
    }

    private float GetValue()
    {
        var value = Vector3.Distance(_startPos, transform.localPosition) / _joint.linearLimit.limit;

        if (Math.Abs(value) < deadZone)
        {
            value = 0;
        }

        return Mathf.Clamp(value, -1, 1);
    }

}
