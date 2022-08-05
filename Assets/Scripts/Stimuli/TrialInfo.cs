using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialInfo : MonoBehaviour
{
    public string Name;
    public int Intensity; // get information from audio manager
    public bool isSpeaker;
    public GameObject speaker;
    public enum Orientation { Auditory, Audiovisual, None }

    public Orientation Type;

    [SerializeField] [Range(1.5f, 10.0f)] private float duration;
    private float timeRemaining;

    private void Start()
    {
        timeRemaining = duration;

        if (isSpeaker)
        {
            speaker = GameObject.Find("/Characters/Speaker/Female Speaker");
            speaker.GetComponent<CharacterWalking>().startMovement();
        }
    }

    private void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            if (isSpeaker)
            {
                speaker.GetComponent<CharacterWalking>().stopMovement();
            }
            Destroy(gameObject);
        }
    }

}
