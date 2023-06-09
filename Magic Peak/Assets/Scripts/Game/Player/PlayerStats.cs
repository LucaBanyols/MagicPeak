using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float currentHealth = 100;

    [Header("Stats")]
    [SerializeField] private float attackDamage = 1f;
    [SerializeField] private float level = 1;
    [SerializeField] private float experience = 0;
    [SerializeField] private float experienceNeeded = 100;

    [Header("Settings")]
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private Xp xpBar;
    [SerializeField] private WalletManager walletManager;
    [SerializeField] private string changeScene;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI winningMoneyText;

    private bool updateStatsEnemy = false;
    private bool isDead = false;
    private WalletData walletData;

    void Start()
    {
        if (PlayerPrefs.HasKey("currentHealth") == false || PlayerPrefs.HasKey("maxHealth") == false || PlayerPrefs.HasKey("level") == false || PlayerPrefs.HasKey("experience") == false || PlayerPrefs.HasKey("experienceNeeded") == false || PlayerPrefs.GetFloat("experienceNeeded") == 0 || PlayerPrefs.HasKey("attackDamage") == false)
        {
            ResetToBaseStatPlayer();
        }
        else
        {
            GetSavedUserStat();
            if (PlayerPrefs.HasKey("VitorSpecialSauce") == false)
            {
                attackDamage = 1f + ((level - 1) * 0.1f);
                PlayerPrefs.SetFloat("attackDamage", attackDamage);
                PlayerPrefs.SetInt("VitorSpecialSauce", 1);
                PlayerPrefs.Save();
                // This is to put everybodies attack damage to the correct value
            }
        }
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);
        xpBar.SetMaxXp(experienceNeeded);
        DisplayHealth();
        DisplayXp();
        DisplayMoney();
        DisplayLevel();
    }

    void Update()
    {
        UpdateStatsPlayer();
        KillPlayer();
        DisplayHealth();
        DisplayXp();
        DisplayMoney();
        DisplayLevel();
        SaveUserStat();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    public void HealPlayer(float heal)
    {
        currentHealth += heal;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
            healthBar.SetHealth(currentHealth);
        }
    }

    public void GainXp(float gainedXp)
    {
        experience += gainedXp;
    }

    public void GainMoney(int money)
    {
        walletManager.AddCurrency(money);
    }

    public float GetAttackPlayer()
    {
        return attackDamage;
    }

    public bool AbleToUpdateStatsEnemy()
    {
        return updateStatsEnemy;
    }

    private void ResetToBaseStatPlayer()
    {
        maxHealth = 100;
        currentHealth = 100;
        attackDamage = 1f;
        level = 1;
        experience = 0;
        experienceNeeded = 100;
    }

    private void ResetStatsPlayer()
    {
        string json = PlayerPrefs.GetString("walletData");
        walletData = JsonUtility.FromJson<WalletData>(json);
        walletData.winningCurrencyAmount = 0;
        json = JsonUtility.ToJson(walletData);
        PlayerPrefs.SetString("walletData", json);
        PlayerPrefs.SetFloat("currentHealth", 100);
        PlayerPrefs.SetFloat("maxHealth", 100);
        PlayerPrefs.SetFloat("level", 1);
        PlayerPrefs.SetFloat("experience", 0);
        PlayerPrefs.SetFloat("experienceNeeded", 100);
        PlayerPrefs.SetFloat("attackDamage", 1f);
        PlayerPrefs.Save();
        currentHealth = maxHealth;
        SaveUserStat();
    }

    private void UpdateStatsPlayer()
    {
        if (experience >= experienceNeeded)
        {
            updateStatsEnemy = true;
            level++;
            experience -= experienceNeeded;
            experienceNeeded = Mathf.RoundToInt(experienceNeeded * 1.5f);
            maxHealth += 10;
            HealPlayer(maxHealth);
            xpBar.SetXp(0);
            xpBar.SetMaxXp(experienceNeeded);
            attackDamage += 0.1f;
        }
        else
        {
            updateStatsEnemy = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            var enemy = collision.GetComponent<EnemyAI>();
            Debug.Log(enemy);
            if (enemy != null)
            {
                TakeDamage(enemy.GetAttackDamageEnemy());
            }
        }
    }

    private void SaveUserStat()
    {
        PlayerPrefs.SetFloat("currentHealth", currentHealth);
        PlayerPrefs.SetFloat("maxHealth", maxHealth);
        PlayerPrefs.SetFloat("level", level);
        PlayerPrefs.SetFloat("experience", experience);
        PlayerPrefs.SetFloat("experienceNeeded", experienceNeeded);
        PlayerPrefs.SetFloat("attackDamage", attackDamage);
    }

    private void GetSavedUserStat()
    {
        currentHealth = PlayerPrefs.GetFloat("currentHealth");
        maxHealth = PlayerPrefs.GetFloat("maxHealth");
        level = PlayerPrefs.GetFloat("level");
        experience = PlayerPrefs.GetFloat("experience");
        experienceNeeded = PlayerPrefs.GetFloat("experienceNeeded");
        attackDamage = PlayerPrefs.GetFloat("attackDamage");
    }

    private void DisplayHealth()
    {
        healthText.text = currentHealth + "/" + maxHealth;
    }

    private void DisplayXp()
    {
        xpBar.SetXp(experience);
    }

    private void DisplayMoney()
    {
        moneyText.text = "$" + walletManager.GetWalletData().ToString();
        winningMoneyText.text = "(+$" + walletManager.GetWinningWalletData().ToString() + ")";
    }

    private void DisplayLevel()
    {
        levelText.text = level.ToString();
    }

    private void KillPlayer()
    {
        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            ResetStatsPlayer();
            SceneManager.LoadScene(changeScene);
        }
    }
}
