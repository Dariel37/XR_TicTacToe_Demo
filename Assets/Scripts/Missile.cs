using UnityEngine;

public class Missile : MonoBehaviour
{

    private bool launchedByEnemy = false;
    public float speed = 5f;
    private Transform target;
    public GameObject explosionPrefab;

   public void SetTarget(Transform newTarget, bool fromEnemy)
    {
        target = newTarget;
        launchedByEnemy = fromEnemy;
    }

    void Update()
    {
        if (target == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );
    }

  private void OnTriggerEnter(Collider other)
{
    if (!launchedByEnemy)
    {
        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            AudioManager.Instance.PlayExplosion();

            enemy.TakeDamage(25);
            enemy.HitReaction();
            transform.DetachChildren();
            Destroy(gameObject, 0.1f);
            return;
        }
    }

    if (launchedByEnemy)
    {
       if (other.CompareTag("Player"))
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        PlayerHealth.Instance.TakeDamage(25);
        transform.DetachChildren();
        Destroy(gameObject, 0.1f);
        return;
    }
    }
}
}
