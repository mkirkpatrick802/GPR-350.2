using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Integrator
{
    public static void Integrate(Particle2D particle2D, float dt)
    {
        Transform partTransform = particle2D.transform;
        Vector2 oldPos = partTransform.position;
        Vector2 oldVelocity = particle2D.velocity;
        
        particle2D.velocity = new Vector2(oldVelocity.x + particle2D.gravity, oldVelocity.y + particle2D.gravity);
        partTransform.position = new Vector3(oldPos.x + (particle2D.velocity.x * dt),oldPos.y + (particle2D.velocity.y * dt),0);
        
        particle2D.velocity *= Mathf.Pow(particle2D.damping, dt);
    }
}