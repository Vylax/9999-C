using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    bool onCooldown = true;
    private float endCooldownTime = 10000000000f;
    private float cooldownLength = .25f;
    private float spawnChance = 7f;// en %

    private void Start()
    {
        this.GetComponent<Collider2D>().enabled = false;
        this.GetComponent<SpriteRenderer>().enabled = false;

        if (GameManager.instance.GetCurrChunkId() < 1) return;

        float temp = Random.Range(0f, 100f);
        if(temp < spawnChance)
        {
            InitializeTrap();
        }
    }

    private void InitializeTrap()
    {
        this.GetComponent<Collider2D>().enabled = true;
        this.GetComponent<SpriteRenderer>().enabled = true;
        onCooldown = false;
        endCooldownTime = -1f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (onCooldown) return;

        if(collision.transform.tag == "Player")
        {
            onCooldown = true;
            endCooldownTime = Time.time + cooldownLength + 10000000;

            collision.transform.GetComponent<Player>().ChangeHealth(-40f);

            this.GetComponent<SpriteRenderer>().enabled = false;
            this.GetComponent<Collider2D>().enabled = false;
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if (onCooldown && Time.time > endCooldownTime) onCooldown = false;
    }
}
