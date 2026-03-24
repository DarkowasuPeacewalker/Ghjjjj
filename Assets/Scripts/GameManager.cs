using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player")]
    [SerializeField] private PlayerController2D player;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int maxHp = 5;

    [Header("UI")]
    [SerializeField] private TMP_Text hpLabel;
    [SerializeField] private TMP_Text gemsLabel;
    [SerializeField] private TMP_Text messageLabel;

    [Header("Flow")]
    [SerializeField] private List<string> levelSceneNames = new();

    private int hp;
    private int gemsCollected;
    private int gemsTotal;
    private int currentLevelIndex;
    private bool runEnded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        hp = maxHp;
    }

    private void Start()
    {
        currentLevelIndex = Mathf.Max(0, levelSceneNames.IndexOf(SceneManager.GetActiveScene().name));
        RecountGems();
        RefreshUI();
        SetMessage("Собери все руны и доберись до портала.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartRun();
        }
    }

    public void RegisterGem()
    {
        gemsCollected++;
        SetMessage("Руна поглощена. Энергия портала растёт.");
        RefreshUI();
    }

    public void RegisterGemTotal()
    {
        gemsTotal++;
        RefreshUI();
    }

    public bool AreAllGemsCollected()
    {
        return gemsCollected >= gemsTotal;
    }

    public void DamagePlayer(int damage, string reason)
    {
        if (runEnded)
        {
            return;
        }

        hp -= damage;
        hp = Mathf.Max(0, hp);
        SetMessage(reason);
        RefreshUI();

        if (hp <= 0)
        {
            runEnded = true;
            SetMessage("Поражение. Нажми R для перезапуска забега.");
            if (player != null)
            {
                player.DisableControl();
            }
        }
    }

    public void RespawnPlayer()
    {
        if (spawnPoint == null)
        {
            return;
        }

        if (player != null)
        {
            player.transform.position = spawnPoint.position;
            player.ResetVelocity();
        }
        DamagePlayer(1, "Падение в бездну! -1 HP.");
    }

    public void ReachExit()
    {
        if (!AreAllGemsCollected())
        {
            SetMessage("Портал заперт. Нужны все руны.");
            return;
        }

        if (currentLevelIndex + 1 >= levelSceneNames.Count)
        {
            runEnded = true;
            SetMessage("Победа! Ты сбежал из подземелий Назарика. Нажми R для нового забега.");
            if (player != null)
            {
                player.DisableControl();
            }
            return;
        }

        currentLevelIndex++;
        SceneManager.LoadScene(levelSceneNames[currentLevelIndex]);
    }

    public void RestartRun()
    {
        runEnded = false;
        hp = maxHp;
        gemsCollected = 0;
        gemsTotal = 0;
        currentLevelIndex = 0;

        if (levelSceneNames.Count > 0)
        {
            SceneManager.LoadScene(levelSceneNames[0]);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void RecountGems()
    {
        gemsCollected = 0;
        gemsTotal = 0;

        var gems = FindObjectsOfType<GemCollectible>();
        gemsTotal = gems.Length;
    }

    private void RefreshUI()
    {
        if (hpLabel != null)
        {
            hpLabel.text = $"HP: {hp}";
        }

        if (gemsLabel != null)
        {
            gemsLabel.text = $"Руны: {gemsCollected}/{gemsTotal}";
        }
    }

    public void SetMessage(string text)
    {
        if (messageLabel != null)
        {
            messageLabel.text = text;
        }
    }
}
