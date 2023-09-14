using System;
using UnityEngine;

public class Particle2D : MonoBehaviour
{
    public double mass = 1;
    [Range(0, 1f)] public float damping = 1f;
    [HideInInspector] public Vector2 velocity;
    [HideInInspector] public Vector2 acceleration;
    [HideInInspector] public Vector2 accumulatedForces;
    [HideInInspector] public float inverseMass;
    [HideInInspector] public float gravity = -9.9f;

    private void FixedUpdate()
    {
        Integrator.Integrate(this, Time.deltaTime);
    }

    public void ApplyForce(Vector2 force)
    {
        accumulatedForces += force;
    }
}