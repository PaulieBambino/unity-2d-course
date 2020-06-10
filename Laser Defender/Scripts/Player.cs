using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // config params
   
    [Header("Player")]
    public Animator animator;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float padding = 1f;
    [SerializeField] float yPadding = 7f;
    [SerializeField] public float health = 1000f;
    [SerializeField] GameObject playerExplosionAnimation;
    [SerializeField] AudioClip playerShootSound;
    [SerializeField] AudioClip playerExplosionSound;
    [SerializeField] [Range(0, 1)] float playerShootVolume = 0.4f;
    [SerializeField] [Range(0, 1)] float playerExplosionVolume = 0.9f;
    
    [Header("Projectiles")]
    [SerializeField] GameObject laserRightPrefab;
    [SerializeField] GameObject laserLeftPrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileFiringPeriod = 0.1f;

    Coroutine firingCoroutine;

    public GameObject leftBulletSpawner;
    public GameObject rightBulletSpawner;

    public HealthBar healthBar;

    float xMin;
    float xMax;
    float yMin;
    float yMax;

    // Use this for initialization
    private void Start()
    {
        SetUpMoveBoundaries();
        healthBar.SetMaxHealth(health);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
        animator.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
    }

    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        transform.position = new Vector2(newXPos, newYPos);
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            GameObject leftBullet = Instantiate(laserLeftPrefab, leftBulletSpawner.transform.position, Quaternion.identity) as GameObject;
            GameObject rightBullet = Instantiate(laserRightPrefab, rightBulletSpawner.transform.position, Quaternion.identity) as GameObject;
            leftBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
            rightBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
            AudioSource.PlayClipAtPoint(playerShootSound, Camera.main.transform.position, playerShootVolume);
            yield return new WaitForSeconds(projectileFiringPeriod);
        }
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
        healthBar.SetHealth(health);
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        FindObjectOfType<Level>().LoadGameOver();
        Destroy(gameObject);
        PlayExplosionAnimation();
        AudioSource.PlayClipAtPoint(playerExplosionSound, Camera.main.transform.position, playerExplosionVolume);
    }

    private void PlayExplosionAnimation()
    {
        GameObject explosion = (GameObject)Instantiate(playerExplosionAnimation);
        explosion.transform.position = transform.position;
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 1, 0)).y - yPadding;
    }
}
