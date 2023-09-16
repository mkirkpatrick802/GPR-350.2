using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public bool slingBack;
    public bool deadBrop;
    public bool flyer;

    private void Start()
    {
        if (slingBack) gameObject.GetComponent<Particle2D>().acceleration = new Vector2(-15, -10);
        if (deadBrop) StartCoroutine(DropTimer());
        if (flyer) gameObject.GetComponent<Particle2D>().acceleration = new Vector2(0, 10);
    }

    IEnumerator DropTimer()
    {
        yield return new WaitForSeconds(.25f);
        gameObject.GetComponent<Particle2D>().acceleration = new Vector2(0, -300);
    }
}