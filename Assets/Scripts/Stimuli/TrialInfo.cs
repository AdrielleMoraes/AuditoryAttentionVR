using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialInfo : MonoBehaviour
{
    public string Name;
    public enum Orientation { Auditory, Audiovisual, None }

    public Orientation Type;

    [SerializeField] [Range(1.5f, 10.0f)] private float duration;
    private float timeRemaining;

    private void Start()
    {
        timeRemaining = duration;
    }

    private void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
