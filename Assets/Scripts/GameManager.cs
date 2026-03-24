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
        DontDestroyOnLoad(gameObject);
        hp = maxHp;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        SyncCurrentLevelIndex();
        ResolveSceneReferences();
        RecountGems();
        RefreshUI();

        if (string.IsNullOrWhiteSpace(messageLabel != null ? messageLabel.text : null))
        {
            SetMessage("Собери все руны и доберись до портала.");
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
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
        if (runEnded)
        {
            return;
        }

        gemsCollected = Mathf.Min(gemsCollected + 1, gemsTotal);
        SetMessage("Руна поглощена. Энергия портала растёт.");
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
        if (runEnded || player == null || spawnPoint == null)
        {
            return;
        }

        player.transform.position = spawnPoint.position;
        player.ResetVelocity();
        DamagePlayer(1, "Падение в бездну! -1 HP.");
    }

    public void ReachExit()
    {
        if (runEnded)
        {
            return;
        }

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

        if (levelSceneNames.Count > 0)
        {
            currentLevelIndex = 0;
            SceneManager.LoadScene(levelSceneNames[0]);
            return;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SyncCurrentLevelIndex();
        ResolveSceneReferences();
        RecountGems();
        RefreshUI();
    }

    private void SyncCurrentLevelIndex()
    {
        int index = levelSceneNames.IndexOf(SceneManager.GetActiveScene().name);
        currentLevelIndex = Mathf.Max(0, index);
    }

    private void ResolveSceneReferences()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerController2D>();
        }

        if (spawnPoint == null)
        {
            GameObject spawn = GameObject.FindGameObjectWithTag("Respawn");
            if (spawn != null)
            {
                spawnPoint = spawn.transform;
            }
        }

        if (hpLabel == null)
        {
            GameObject hpObj = GameObject.Find("HPLabel");
            if (hpObj != null)
            {
                hpLabel = hpObj.GetComponent<TMP_Text>();
            }
        }

        if (gemsLabel == null)
        {
            GameObject gemsObj = GameObject.Find("GemsLabel");
            if (gemsObj != null)
            {
                gemsLabel = gemsObj.GetComponent<TMP_Text>();
            }
        }

        if (messageLabel == null)
        {
            GameObject messageObj = GameObject.Find("MessageLabel");
            if (messageObj != null)
            {
                messageLabel = messageObj.GetComponent<TMP_Text>();
            }
        }

        if (player != null)
        {
            player.EnableControl();
        }
    }

    private void RecountGems()
    {
        gemsCollected = 0;
        gemsTotal = FindObjectsOfType<GemCollectible>().Length;
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
