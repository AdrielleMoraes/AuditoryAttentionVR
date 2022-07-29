using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialInfo : MonoBehaviour
{
    public string Name;
    public int Intensity; // get information from audio manager

    public enum Orientation { Auditory, Audiovisual, None }

    public Orientation Type;
}
