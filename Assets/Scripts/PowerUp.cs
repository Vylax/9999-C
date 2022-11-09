using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public int type = -1;
    public bool used = true;
    public float spawnChance = 20f;// en %
    public ParticleSystem ps;
    public Color[] particlesColor = new Color[3];

    private void Start()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;

        if (GameManager.instance.GetCurrChunkId() < 1) return;

        InitPowerUp();
    }

    private void InitPowerUp()
    {
        float tempSpawn = Random.Range(0f, 100f);
        if (tempSpawn > spawnChance) return;

        float temp = Random.Range(0f, 100f);//0-50=health 50-75=fly 75-100=flame
        if(temp <= 50f)
        {
            type = 0;//health
        }else if(temp <= 75f)
        {
            type = 1;//fly
        }else if(temp <= 100f)
        {
            type = 2;//flame
        }
        used = false;
        this.GetComponent<Animator>().SetInteger("type", type);
        this.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used) return;

        if (other.transform.tag == "Player")
        {
            used = true;
            this.GetComponent<SpriteRenderer>().enabled = false;
            if(type > 0)
            {
                other.transform.GetComponent<Player>().PowerUp(type);
            }
            else
            {
                other.transform.GetComponent<Player>().ChangeHealth(+30f);
            }
            //play used anim
            ps.startColor = particlesColor[type];
            ps.Play();
        }
    }
}
