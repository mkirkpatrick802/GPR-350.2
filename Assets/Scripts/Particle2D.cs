using System;
using UnityEngine;

public class Particle2D : MonoBehaviour
{
    public double mass = 1;
    [Range(0, 1f)] public float damping = 1f;
    [HideInInspector] public Vector2 velocity;
    private Vector2 acceleration;
    private Vector2 accumulatedForces;
    private Single inverseMass;
    [HideInInspector] public float gravity = -9.9f;

    private void FixedUpdate()
    {
        if(velocity == Vector2.zero) return;
        Integrator.Integrate(this, Time.deltaTime);
    }
}