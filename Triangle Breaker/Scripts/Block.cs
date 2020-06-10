using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    //config params
    [SerializeField] AudioClip breakSound;
    [SerializeField] GameObject blockSparklesVFX;
    [SerializeField] Sprite[] hitSprites;

    //cached reference
    Level level;
    GameStatus points;
    public GameObject paddle;

    //state variables
    [SerializeField] int timesHit; // TODO only serialized to debug purposes

    private void Start()
    {
        CountBreakableBlocks();
    }

    private void CountBreakableBlocks()
    {
        level = FindObjectOfType<Level>();
        if (tag == "Breakable")
        {
            level.CountBlocks();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (tag == "Breakable")
        {
            HandleHit();
        }
        if (tag == "Crazy")
        {
            GoCrazy();
        }
        if (tag == "Shorty")
        {
            GoShort();
        }
    }

    private void HandleHit()
    {
        timesHit++;
        int maxHits = hitSprites.Length + 1;
        if (timesHit >= maxHits)
        {
            DestroyBlock();
        }
        else
        {
            ShowNextHitSprite();
        }
    }

    private void GoCrazy()
    {
        if (Camera.main.transform.rotation.z <=0)
        {
            Camera.main.transform.rotation = new Quaternion(0f, 0f, 180f, 0f);
        }
        else
        {
            Camera.main.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        }
    }

    private void GoShort()
    {
        paddle = GameObject.Find("Paddle");
        if (paddle.transform.localScale.x >=1)
        {
            paddle.transform.localScale = new Vector2(0.7f, 1f);
        }
        else
        {
            paddle.transform.localScale = new Vector2(1f, 1f);
        }
    }


    private void ShowNextHitSprite()
    {
        int spriteIndex = timesHit - 1;
        if (hitSprites[spriteIndex] != null)
        {
            GetComponent<SpriteRenderer>().sprite = hitSprites[spriteIndex];
        }
        else
        {
            Debug.LogError("Block Sprite is missing from array" + gameObject.name);
        }
    }

    private void DestroyBlock()
    {
        PlayBlockDestroyedSFX();
        Destroy(gameObject);
        level.BlockDestroyed();
        TriggerSparklesVFX();
    }

    private void PlayBlockDestroyedSFX()
    {
        FindObjectOfType<GameStatus>().AddToScore();
        AudioSource.PlayClipAtPoint(breakSound, Camera.main.transform.position);
    }

    private void TriggerSparklesVFX()
    {
        GameObject sparkles = Instantiate(blockSparklesVFX, transform.position, transform.rotation);
        Destroy(sparkles, 1f);
    }
}
