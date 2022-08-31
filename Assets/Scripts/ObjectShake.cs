using UnityEngine;
using System.Collections;

public class ObjectShake : MonoBehaviour
{
    public float blinkInterval = 0.4f;

    private void Start()
    {
        InvokeRepeating("Blink", 0, blinkInterval);
	}
    void Blink()
	{
        Renderer objRenderer = GetComponent<Renderer>();
        objRenderer.enabled = !objRenderer.enabled;
    }
}
