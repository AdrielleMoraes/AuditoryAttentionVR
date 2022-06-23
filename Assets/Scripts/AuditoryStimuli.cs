using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AuditoryStimuli : MonoBehaviour
{
    public enum ENUMmovementType // your custom enumeration
    {
        Linear,
        ZigZag,
        Static
    };

    public ENUMmovementType movementType;
    [SerializeField] [Range(1f, 2f)] private int loudnessLevel;
    [SerializeField] [Range (1.5f, 10.0f)]private float duration;

    public float movementSpeed = 2;

    private float timeRemaining;

    private AudioSource m_audio;
    // Start is called before the first frame update
    void Start()
    {
        m_audio = GetComponent<AudioSource>();
        m_audio.spatialBlend = 1;
        m_audio.PlayDelayed(0);

        timeRemaining = duration;
    }

    // Update is called once per frame
    void Update()
    {
        switch (movementType)
        {
            case ENUMmovementType.Linear:
                transform.position -= Vector3.right * Time.deltaTime * movementSpeed;
                break;
            case ENUMmovementType.ZigZag:
                if (transform.position.x <= 1)
                {
                    movementSpeed = Mathf.PingPong(Time.time, 1);
                }
                else if (transform.position.x >= 1)
                {
                    movementSpeed = -Mathf.PingPong(Time.time, 1);
                }
                transform.position = new Vector3(movementSpeed, Mathf.PingPong(Time.time, 2), transform.position.z);
                break;
            default:
                break;
        }

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;           
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }
}
