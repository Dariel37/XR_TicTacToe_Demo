using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }
   public int maxHealth = 100;
    private int currentHealth;

    public TMP_Text playerHealthText;
    public TMP_Text winnerText;
    public TMP_Text turnText;
    public Button restartButton;



    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        AudioManager.Instance.PlayHit();

        UpdateUI();

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    private void UpdateUI()
    {
        playerHealthText.text = "Player HP: " + currentHealth;
    }

    private void GameOver()
    {
        GameManager.Instance.PlayerDefeated();
    }
}
