using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private GameObject endLevel;
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject menuBtn;
    [SerializeField] private GameObject mainBtn;
    [SerializeField] public GameObject score;
    public GameObject selectLevelPanel;

    private Vector2 originalMenuPosition;
    private Vector2 endMenuPosition;
    private bool isOpen = false;
    public void ShowSelectLevelPanel()
    {
        selectLevelPanel.SetActive(!selectLevelPanel.activeSelf);
    }

    public void UpdateScore()
    {
        score.GetComponent<Text>().text = $"{PlayerPrefs.GetInt("PlayerScore")}";
    }
    public void CleanCanvas()
    {
        if (isOpen)
            ShowMenu();
    }
    public void ShowMenu()
    {
        StartCoroutine(LerpMenu());
    } 
    private IEnumerator LerpMenu()
    {
        float timeOfTravel = 0.15f;
        float currentTime = 0;
        float normalizedValue;

        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.deltaTime;
            normalizedValue = currentTime / timeOfTravel;


            if (!isOpen)
            {
                menu.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(originalMenuPosition, endMenuPosition, normalizedValue);
                Camera.main.orthographicSize = Mathf.Lerp(8.5f, 7, 8.5f);
            }
            else
            {
                menu.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(endMenuPosition, originalMenuPosition, normalizedValue);
                Camera.main.orthographicSize = Mathf.Lerp(12, 6f, 12);
            }

            yield return null;
        }

        isOpen = !isOpen;

    }

    public void ShowEndLevel(bool value)
    {
        endLevel.SetActive(value);
    }
    public void MainButtonClick()
    {
        mainBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        mainBtn.GetComponent<Button>().onClick.AddListener(delegate { LevelManager.Instance.LoadThisLevel(GameManager.Instance.PlayingLevel); });
    }

}
