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

    public float movementSpeed = 2;


    private AudioSource m_audio;
    // Start is called before the first frame update
    void Start()
    {
        m_audio = GetComponent<AudioSource>();
        m_audio.spatialBlend = 1;
        m_audio.PlayDelayed(0);
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
    }

}
