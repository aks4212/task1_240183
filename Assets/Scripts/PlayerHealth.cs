using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;

    private Image[] hearts; // Will be found dynamically
    private PlayerController playerController;

    // Static variable to persist health across levels
    private static int savedHealth = -1;

    void Start()
    {
        playerController = GetComponent<PlayerController>();

        // Initialize current health
        if (savedHealth == -1)
        {
            currentHealth = maxHealth;
            savedHealth = currentHealth;
        }
        else
        {
            currentHealth = savedHealth;
        }

        FindHeartsUI();       // ← Find hearts from persistent scene
        UpdateHeartsUI();     // ← Apply visual update
    }

    void FindHeartsUI()
    {
        GameObject heartsManager = GameObject.Find("HeartsManager");

        if (heartsManager != null)
        {
            // Get all Image children under HeartsManager
            hearts = heartsManager.GetComponentsInChildren<Image>();
        }
        else
        {
            Debug.LogWarning("HeartsManager not found in scene.");
            hearts = new Image[0];
        }
    }

    // Updated to accept variable damage amount
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Prevent negative HP
        savedHealth = currentHealth;

        UpdateHeartsUI();

        if (currentHealth <= 0)
        {
            playerController.Die();
        }
    }

    // Optional: keep old version in case other code depends on it
    public void TakeDamage()
    {
        TakeDamage(1); // Default to 1 damage
    }

    void UpdateHeartsUI()
    {
        if (hearts == null || hearts.Length == 0)
        {
            FindHeartsUI(); // Try again if not yet found (scene still loading)
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
            {
                hearts[i].enabled = i < currentHealth;
            }
        }
    }

    // Call this when restarting the game to reset health
    public static void ResetStaticHealth()
    {
        savedHealth = -1;
    }
}
