using UnityEngine;

public class Pistol : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Rigidbody bullet;
    [SerializeField] private float lifeTime;

    public void Shoot()
    {
        var spawned = Instantiate(bullet, attackPoint.position, attackPoint.rotation);
        spawned.linearVelocity = attackPoint.forward * 10;
        
        Destroy(spawned.gameObject, lifeTime);
    }
}