using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForeAndBackGrounds : MonoBehaviour
{
    private Vector3 playerStartPos;
    private Vector3 startPos;

    private GameObject player;

    public float speedFactor;

    private float translationSpeed;
    /*private float TranslationSpeed
    {
        get
        {
            if (translationSpeed > 0)
            {
                //return translationSpeed;
            }
            translationSpeed = speedFactor * (transform.position.z < 0 ? Mathf.Abs(transform.position.z) : 1f / transform.position.z);
            return translationSpeed;
        }
    }*/

    private enum Type
    {
        Foreground,
        Background,
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerStartPos = player.transform.position;
        startPos = transform.position;
    }

    private void Update()
    {
        transform.position = startPos + speedFactor * LinearVector3(player.transform.position - playerStartPos) /** TranslationSpeed*/;
    }

    private Vector3 LinearVector3(Vector3 v)
    {
        return v.x * Vector3.right;
    }
}
