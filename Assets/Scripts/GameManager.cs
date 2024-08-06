using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector] public LevelManager levelManager;

    [SerializeField] private LevelsBag levelsBag;
    public LevelsBag SavedLevels
    {
        get { return levelsBag; }
        set { levelsBag = value; }
    }

    private int sessionScore = 0;
    public int SessionScore
    {
        get { return sessionScore; }
        set { sessionScore = value; }
    }
    private int playingLevel = 0;
    public int PlayingLevel
    {
        get { return playingLevel; }
        set { playingLevel = value; }
    }
    private void Awake()
    {
        levelManager = LevelManager.Instance;
    }

    private void Start()
    {
        SetupSavedLevels();

        Screen.orientation = ScreenOrientation.Portrait;
    }

    public void SetupSavedLevels()
    {
        int bestLevel = 0;

        if (PlayerPrefs.HasKey("BestLevel"))
        {
            bestLevel = PlayerPrefs.GetInt("BestLevel");
        }
        else
            PlayerPrefs.SetInt("BestLevel", bestLevel);

        foreach (Transform child in UIManager.Instance.selectLevelPanel.transform.Find("LevelsParent"))
        {
            // Setup Button
            child.GetComponent<Button>().onClick.RemoveAllListeners();
            child.GetComponent<Button>().onClick.AddListener(delegate { LoadSelectedLevel(child.GetSiblingIndex()); });

            if (child.GetSiblingIndex() > bestLevel)
            {
                child.GetComponent<Button>().interactable = false;
                child.GetComponentInChildren<Text>().text = $"";
            }
            else
            {
                child.GetComponent<Button>().interactable = true;
                child.GetComponentInChildren<Text>().text = $"{child.GetSiblingIndex() + 1}";
            }
        }
    }

    public void LoadSelectedLevel(int index)
    {
        levelManager.LoadThisLevel(index);
        UIManager.Instance.ShowSelectLevelPanel();
    }
    public void CheckScore()
    {
        if (PlayerPrefs.HasKey("BestLevel"))
        {
            if (PlayerPrefs.GetInt("BestLevel") < playingLevel)
                PlayerPrefs.SetInt("BestLevel", playingLevel);
        }
        else
        {
            PlayerPrefs.SetInt("BestLevel", playingLevel);
        }

        int tempScore = PlayerPrefs.GetInt("PlayerScore") + sessionScore;
        PlayerPrefs.SetInt("PlayerScore", tempScore);

        UIManager.Instance.UpdateScore();
    }
 
    public void LevelCompleted()
    {
        PlayingLevel++;
        SessionScore += (LevelManager.Instance.LevelSettings.width * LevelManager.Instance.LevelSettings.height) * 100;

        CheckScore();
        SetupSavedLevels();
        UIManager.Instance.MainButtonClick();
       
        UIManager.Instance.ShowEndLevel(true);
    }
}
