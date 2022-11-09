using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public FixedJoystick joystick;

    public float speed = 40f;

    private float SpeedFactor => this.GetComponent<Player>().speedFactor;//will be used if we decide to increase speed over time

    float moveX = 0f;
    bool jump = false;
    bool crouch = false;
    bool hover = false;

    private void Update()
    {
        moveX = joystick.Horizontal * speed * SpeedFactor;

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (touch.position.x > Screen.width / 2)
                {
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            jump = true;
                            break;
                    }
                    break;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        controller.Move(moveX * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }
}
