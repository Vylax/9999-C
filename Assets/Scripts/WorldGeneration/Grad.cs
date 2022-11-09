using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grad : MonoBehaviour
{
    private void Update()
    {
        Vector3 temp = transform.position;
        temp.x = Camera.main.transform.position.x;
        transform.position = temp;
    }
}
