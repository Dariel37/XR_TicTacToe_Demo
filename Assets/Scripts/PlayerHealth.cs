using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    // Singleton reference so other scripts can access PlayerHealth easily.
    public static PlayerHealth Instance { get; private set; }
    // Maximum HP.
    public int maxHealth = 100;
    // Current HP during gameplay.
    private int currentHealth;
    // UI text displaying player health.
    public TMP_Text playerHealthText;
    // These are currently unused in this script.
    public TMP_Text winnerText;
    public TMP_Text turnText;
    public Button restartButton;



    private void Awake()
    {
        // Register singleton instance.
        Instance = this;
    }


    private void Start()
    {
         // Start player at full health.
        currentHealth = maxHealth;
        // Update UI at game start.
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        // Reduce player HP.
        currentHealth -= damage;
        // Clamp HP so it stays between 0 and maxHealth.
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        // Play hit sound.
        AudioManager.Instance.PlayHit();
        // Update HP UI.
        UpdateUI();
        // If HP reaches 0, trigger game over.
        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    private void UpdateUI()
    {
        // Update health text on screen.
        playerHealthText.text = "Player HP: " + currentHealth;
    }

    private void GameOver()
    {
        // Notify GameManager that player has died.
        GameManager.Instance.PlayerDefeated();
    }
}
