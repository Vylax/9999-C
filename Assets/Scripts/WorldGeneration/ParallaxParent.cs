using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxParent : MonoBehaviour
{
    private void Start()
    {
        transform.parent = Camera.main.transform;
    }
}
