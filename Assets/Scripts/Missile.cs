using UnityEngine;

public class Missile : MonoBehaviour
{
    // Tracks who fired the missile.
    // false = player missile
    // true = enemy missile
    private bool launchedByEnemy = false;

    // Movement speed of missile.
    public float speed = 5f;

    // Target the missile will move toward.
    private Transform target;

    // Explosion prefab spawned on hit.
    public GameObject explosionPrefab;

   public void SetTarget(Transform newTarget, bool fromEnemy)
    {
        // Assign target.
        target = newTarget;
        // Store who launched missile.
        launchedByEnemy = fromEnemy;
    }

    void Update()
    {
        // If no target exists, do nothing.
        if (target == null) return;
        
        // Move missile toward target every frame.
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );
    }

  private void OnTriggerEnter(Collider other)
{
    // PLAYER MISSILE LOGIC

    if (!launchedByEnemy)
    {
        // Check if missile hit enemy.
        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null)
        {
            // Spawn explosion VFX.
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            // Play explosion sound.
            AudioManager.Instance.PlayExplosion();
            // Damage enemy.
            enemy.TakeDamage(25);
            // Trigger enemy shake effect.
            enemy.HitReaction();
            // Detach explosion so it survives after missile is destroyed.
            transform.DetachChildren();
            // Destroy missile.
            Destroy(gameObject, 0.1f);
            return;
        }
    }
        // ENEMY MISSILE LOGIC
    if (launchedByEnemy)
    {
        // Check if missile hit player.
       if (other.CompareTag("Player"))
    {
        // Spawn explosion VFX.
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        // Damage player.
        PlayerHealth.Instance.TakeDamage(25);
        // Detach children before destroy.
        transform.DetachChildren();
        // Destroy missile.
        Destroy(gameObject, 0.1f);
        return;
    }
    }
}
}
