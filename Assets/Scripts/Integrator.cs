using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Integrator
{
    public static void Integrate(Particle2D particle2D, float dt)
    {
        //particle2D.acceleration = Vector2.up * particle2D.gravity;
        particle2D.transform.position += (Vector3)particle2D.velocity * dt;
        
        particle2D.velocity *= Mathf.Pow(particle2D.damping, dt);
        particle2D.velocity += particle2D.acceleration * dt;
    }
}

//particle2D.acceleration += particle2D.accumulatedForces * particle2D.inverseMass;
//particle2D.accumulatedForces = Vector2.zero;