using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 offset;
    private GameObject player;
    private float startHeight;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(offset == Vector3.zero)
        {
            offset = Vector3.forward * (transform.position.z - player.transform.position.z);
        }
        startHeight = transform.position.y;
    }

    private void Update()
    {
        transform.position = new Vector3(player.transform.position.x, startHeight, player.transform.position.z) + offset;
    }
}
