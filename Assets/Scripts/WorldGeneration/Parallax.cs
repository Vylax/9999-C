using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startPos;
    public GameObject cam;
    public float parallaxEffectSpeed;
    private float startHeight;

    private void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        startHeight = transform.position.y;
    }

    private void Update()
    {
        float temp = cam.transform.position.x * (1 - parallaxEffectSpeed);
        float dist = cam.transform.position.x * parallaxEffectSpeed;

        transform.position = new Vector3(startPos + dist, startHeight, transform.position.z);

        if (temp > startPos + length) startPos += length;
        else if (temp < startPos - length) startPos -= length;
    }
}
