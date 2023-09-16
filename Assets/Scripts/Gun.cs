using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public GameObject[] projectiles;
    public Transform spawnpoint;
    private int _projectileIndex;
    private float _currentProjectileSpeed;

    /// <summary>
    /// The direction of the initial velocity of the fired projectile. That is,
    /// this is the direction the gun is aiming in.
    /// </summary>
    public Vector3 FireDirection => spawnpoint.up;

    /// <summary>
    /// The position in world space where a projectile will be spawned when
    /// Fire() is called.
    /// </summary>
    public Vector3 SpawnPosition => spawnpoint.position;

    /// <summary>
    /// The currently selected weapon object, an instance of which will be
    /// created when Fire() is called.
    /// </summary>
    public GameObject CurrentWeapon
    {
        get
        {
            GameObject scriptableObj = projectiles[_projectileIndex];
            _currentProjectileSpeed = scriptableObj.GetComponent<Projectile>().speed;
            return scriptableObj;
        }
    }

    /// <summary>
    /// Spawns the currently active projectile, firing it in the direction of
    /// FireDirection.
    /// </summary>
    /// <returns>The newly created GameObject.</returns>
    public GameObject Fire()
    {
        Particle2D particle = GameObject.Instantiate(CurrentWeapon, SpawnPosition, Quaternion.identity).GetComponent<Particle2D>();
        particle.velocity = FireDirection * _currentProjectileSpeed;
        return particle.gameObject;
    }

    /// <summary>
    /// Moves to the next weapon. If the last weapon is selected, calling this
    /// again will roll over to the first weapon again. For example, if there
    /// are 4 weapons, calling this 4 times will end up with the same weapon
    /// selected as if it was called 0 times.
    /// </summary>
    public void CycleNextWeapon()
    {
        _projectileIndex++;
        if (_projectileIndex > projectiles.Length - 1) _projectileIndex = 0;
    }

    void Update()
    {
        //Switch Projectile
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            CycleNextWeapon();
        }
        
        //Rotate counter-clockwise
        if (Keyboard.current.digit1Key.isPressed)
        {
            transform.Rotate(Vector3.forward, 0.25f);
        }
        
        //Rotate clockwise
        if (Keyboard.current.digit2Key.isPressed)
        {
            transform.Rotate(Vector3.forward, -0.25f);
        }
        
        //Fire
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            Fire();
        }
    }
}
