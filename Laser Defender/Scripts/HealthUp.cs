using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUp : MonoBehaviour
{
    [SerializeField] int healing = 200;


    public int Heal()
    {
        return healing;
    }

    public void OnHit()
    {
        Destroy(gameObject);
    }
    }