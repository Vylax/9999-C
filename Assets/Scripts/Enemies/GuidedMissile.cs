using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedMissile : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] private float moveForce;
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private ParticleSystem trail;

    private bool isChasing = false;
    private bool exploded = false;

    private Vector2 playerDirection
    {
        get { return (GameManager.instance.player.transform.position - this.transform.position).normalized; }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void StartChase()
    {
        isChasing = true;
    }

    private void Update()
    {
        if (!isChasing)
        {
            return;
        }
        rb.AddForce(moveForce * playerDirection, ForceMode2D.Force);

        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (exploded) return;

        if (collision.transform.tag == "Player")
        {
            //damage player here
            exploded = true;
            collision.transform.GetComponent<Player>().ChangeHealth(-20);
        }
        Explode();
    }

    private void Explode()
    {
        //animations fx & sfx here
        explosion.Play();
        trail.Stop();
        this.GetComponent<SpriteRenderer>().enabled = false;
        this.GetComponentInChildren<SpriteRenderer>().enabled = false;
        this.GetComponent<Collider2D>().enabled = false;
        this.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        Destroy(this.gameObject, explosion.duration);
    }
}
