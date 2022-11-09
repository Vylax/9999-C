using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public FixedJoystick LeftJoystick;

    private Vector2 inputs
    {
        get { return new Vector2(LeftJoystick.Horizontal, LeftJoystick.Vertical); }
    }

    /*private void OnGUI()
    {
        GUILayout.Box($"x: {inputs.x} \n y: {inputs.y}");
    }*/
}
