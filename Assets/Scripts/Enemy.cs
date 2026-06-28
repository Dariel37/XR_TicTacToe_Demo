using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class Enemy : MonoBehaviour
{
     public int maxHealth = 100;
    private int currentHealth;
    private Vector3 originalPosition;

    public Image healthBarFill;


    void Start()
    {
        currentHealth = maxHealth;
        originalPosition = transform.position;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);


        Debug.Log("Boss HP: " + currentHealth);

        healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        AudioManager.Instance.PlayHit();
        

        if (currentHealth <= 0)
        {
            Die();
        }
    }

     IEnumerator Shake()
    {
        float duration = 0.3f;
        float strength = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = originalPosition + Random.insideUnitSphere * strength;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
    }





    public void HitReaction()
    {
        StartCoroutine(Shake());
    }



    void Die()
    {
        Debug.Log("Boss Defeated!");
        gameObject.SetActive(false);

        GameManager.Instance.EnemyDefeated();
    }
}
