using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRoll : MonoBehaviour
{
    [SerializeField]int HorizontalMovement = 1;
    [SerializeField] int VerticalMovement = 0;
    [SerializeField] int Speed = 2;
    AudioSource audioSource;

    Rigidbody rb;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 MoveBall = new Vector3(HorizontalMovement, 0, VerticalMovement);

        //lastly, we will need to access the physics component of our ball game object (i.e. the Rigidbody) and add a force to it based on the values of the vector we just defined. We will multiple this force value with our Speed variable to be able to control the Speed of the force as we wish.
        gameObject.transform.GetComponent<Rigidbody>().AddForce(MoveBall * Speed);


    }
    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Floor")
        {
            rb.freezeRotation = true;
            Speed = 0;
            audioSource.Play();
        }
    }
}
