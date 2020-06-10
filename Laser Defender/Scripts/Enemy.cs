using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [Header("Enemy Stats")]
    [SerializeField] float health = 100;
    [SerializeField] int scoreValue = 100;

    [Header("Shooting")]
    float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 1f;
    [SerializeField] GameObject enemyBulletPrefab;
    [SerializeField] float projectileSpeed = 10f;

    [Header("Animation and SFX")]
    [SerializeField] GameObject explosionAnimation;
    [SerializeField] AudioClip enemyExplosionSound;
    [SerializeField] AudioClip enemyShootSound;
    [SerializeField] [Range(0, 1)] float enemyShootVolume = 0.1f;
    [SerializeField] [Range(0, 1)] float enemyExplosionVolume = 0.8f;


    public GameObject laserSpawner;
    SpriteRenderer spriteRenderer;
    public Player player;
    public HealthUp healthUp;

    // Start is called before the first frame update
    void Start()
    {
        shotCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
            CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0)
        {
            Fire();
            shotCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
            AudioSource.PlayClipAtPoint(enemyShootSound, Camera.main.transform.position, enemyShootVolume);            
        }
    }

    private void Fire()
    {
        GameObject enemyBullet = Instantiate(enemyBulletPrefab, laserSpawner.transform.position, Quaternion.identity) as GameObject;
        enemyBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -projectileSpeed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        spriteRenderer.color = new Color(255f, 0f, 0f);
        if (health <= 0)
        {
            Die();
        }
        else
        {
            Invoke("ResetColor", .1f);
        }
    }

    private void Die()
    {
        FindObjectOfType<GameSession>().AddToScore(scoreValue);
        Destroy(gameObject);
        PlayExplosionAnimation();
        AudioSource.PlayClipAtPoint(enemyExplosionSound, Camera.main.transform.position, enemyExplosionVolume);
    }


    private void PlayExplosionAnimation()
    {
        GameObject explosion = (GameObject)Instantiate(explosionAnimation);
        explosion.transform.position = transform.position;
    }

    private void ResetColor()
    {
        spriteRenderer.color = new Color(255f, 255f, 255f);
    }
}
