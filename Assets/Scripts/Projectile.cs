using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "ScriptableObjects/New Projectile", order = 1)]
public class Projectile : ScriptableObject
{
    public GameObject obj;
    public float speed;
    public Material mat;
}
