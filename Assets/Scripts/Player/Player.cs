using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CustomClasses;

public class Player : MonoBehaviour
{
    public float health = 100f;
    public float speedFactor = 1f;

    public bool[] powerUps = new bool[3];
    public float[] powerUpsStopTime = new float[3];
    public ParticleSystem[] powerUpsparticles = new ParticleSystem[3];

    public GameObject textInfo;

    private void FixedUpdate()
    {
        if (transform.position.y < -6)
            Die();

        for (int i = 0; i < powerUps.Length; i++)
        {
            if(powerUps[i] && Time.time >= powerUpsStopTime[i])
            {
                powerUps[i] = false;
            }
        }
    }

    public void ChangeHealth(float health, bool isTimeDamage=false)
    {
        if (health < 0 && powerUps[2]) return;

        this.health += health;
        float variation = health;

        if (this.health > 100f)
        {
            variation = health - (this.health - 100f);
            this.health = 100f;
        }
        else if (this.health < 0f)
        {
            this.health = 0f;
            Die();
        }

        if (!isTimeDamage) AlteredHealth(variation);
    }

    private void Die()
    {
        if (GameManager.instance.status != GameStatus.InGame)
            return;

        for (int i = 0; i < powerUps.Length; i++)
        {
            powerUps[i] = false;
        }
        GameManager.instance.status = GameStatus.IsDead;
        GameManager.instance.EndGame();
    }

    public void PowerUp(int powerUp)
    {
        powerUpsStopTime[powerUp] = Time.time + 12f;
        powerUpsparticles[powerUp].Clear();
        powerUpsparticles[powerUp].Play();
        powerUps[powerUp] = true;
    }

    private void AlteredHealth(float health)
    {
        //play anim here
        TextInfo temp = Instantiate(textInfo, transform.position - Vector3.forward, Quaternion.identity).GetComponent<TextInfo>();
        float rdm = Random.Range(-20f, 20f);
        temp.dir = Quaternion.AngleAxis(rdm, Vector3.forward) * Vector3.up;
        temp.transform.eulerAngles = new Vector3(temp.transform.eulerAngles.x, temp.transform.eulerAngles.y, rdm);
        temp.bonus = health > 0;
        StartCoroutine(temp.InitText($"{(health > 0 ? "+" : "")}{Mathf.RoundToInt(health)}"));
    }
}
