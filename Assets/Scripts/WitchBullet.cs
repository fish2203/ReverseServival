using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchBullet : MonoBehaviour
{
    [HideInInspector] public float power; // Äá¾ËÅº ÇÇÇØ·®
    [HideInInspector] public bool hitToHero = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //hitToHero = true;
            //Destroy(this);
        }
    }
}
