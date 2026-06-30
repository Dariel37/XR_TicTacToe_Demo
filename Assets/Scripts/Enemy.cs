using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class Enemy : MonoBehaviour
{
    // Maximum HP of the enemy.
     public int maxHealth = 100;
     
    // Current HP during gameplay.
    private int currentHealth;
    // Stores original position for shake effect.
    private Vector3 originalPosition;
    // UI image used as health bar fill.
    public Image healthBarFill;


    void Start()
    {
        // Start enemy at full health.
        currentHealth = maxHealth;
        // Save starting position for hit shake effect.
        originalPosition = transform.position;
    }

    public void TakeDamage(int damage)
    {
        // Reduce enemy HP.
        currentHealth -= damage;
        // Clamp HP so it never goes below 0 or above max.
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);


        Debug.Log("Boss HP: " + currentHealth);
        // Update health bar fill amount.
        healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        // Play hit sound effect.
        AudioManager.Instance.PlayHit();
        
        // If HP reaches 0, enemy dies.
        if (currentHealth <= 0)
        {
            Die();
        }
    }

     IEnumerator Shake()
    {
        // Shake effect duration.
        float duration = 0.3f;
        // Shake intensity.
        float strength = 0.2f;
        // Timer.
        float elapsed = 0f;
        // Shake while timer is active.
        while (elapsed < duration)
        {
            // Move enemy randomly around original position.
            transform.position = originalPosition + Random.insideUnitSphere * strength;
            
            elapsed += Time.deltaTime;
            
            // Wait one frame.
            yield return null;
        }
        // Reset enemy to original position after shaking.
        transform.position = originalPosition;
    }





    public void HitReaction()
    {
        // Start shake animation when enemy gets hit.
        StartCoroutine(Shake());
    }



    void Die()
    {
        Debug.Log("Enemy Defeated!");
        // Hide enemy from scene.
        gameObject.SetActive(false);
        // Notify GameManager that boss is dead.
        GameManager.Instance.EnemyDefeated();
    }
}
