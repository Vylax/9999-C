using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextInfo : MonoBehaviour
{
    private bool isActivated = false;
    public float speed = 1.3f;
    private TextMesh tm;

    public Color color = Color.white;
    public Vector3 dir = Vector3.up;
    //public Font[] fonts;
    public bool currFont = false;
    public float fontSwitchRate = .15f;

    public bool bonus = false;

    private void Awake()
    {
        this.GetComponent<MeshRenderer>().enabled = false;
        tm = this.GetComponent<TextMesh>();
    }
    public IEnumerator InitText(string text, float delay=0.1f)
    {
        yield return new WaitForSeconds(delay);
        isActivated = true;
        tm.text = text;
        tm.color = color;
        InvokeRepeating("ToggleFont", 0f, fontSwitchRate);
        this.GetComponent<MeshRenderer>().enabled = true;
    }

    private void Update()
    {
        if (transform.position.y > 15f) Destroy(this.gameObject);

        if (isActivated)
        {
            transform.position += dir * speed * Time.deltaTime;
        }
    }

    private void ToggleFont()
    {
        tm.color = currFont ? (bonus ? Color.green : Color.red) : Color.white;
        currFont = !currFont;
    }
}
