using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    /// <summary>
    /// Particle system explosion. Impact dictates the size of the explosion and should be a int from (0 to 3)
    /// </summary>
    /// <param name="parent_velocity"></param>
    /// <param name="impact"></param>
    public void explosion(Vector2 parent_velocity, int impact){
        
        Vector2 newVelocity = parent_velocity * Random.Range(0.2f, 0.3f);
        GetComponent<Rigidbody2D>().velocity = newVelocity;

        // set particle system values
        ParticleSystem ps = GetComponent<ParticleSystem>();
        var velocityLimit = ps.limitVelocityOverLifetime; // Copy the module
        velocityLimit.limit = impact / 10; // Modify the property
        GetComponent<ParticleSystem>().Emit(50 + Random.Range(10, 20) * impact);

        // Destroy this object after the particle system fades
        Invoke("Die", 2f);
    }

    private void Die(){ Destroy(gameObject); }
}
