using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private CanvasGroup playScreenCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private GameObject MainMenuCamera;
    [SerializeField] private GameObject LeaderboardCamera;
    [SerializeField] private GameObject SettingsCamera;

    public void ClickAnywhereToStart()
    {
        StartCoroutine(FadeOutAndDisable());
    }

    private IEnumerator FadeOutAndDisable()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            playScreenCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        playScreenCanvasGroup.alpha = 0f;
        playScreenCanvasGroup.gameObject.SetActive(false);
    }

    public void GoToLeaderboard()
    {
        MainMenuCamera.SetActive(false);
        LeaderboardCamera.SetActive(true);
        SettingsCamera.SetActive(false);
    }

    public void GoToSettings()
    {
        MainMenuCamera.SetActive(false);
        LeaderboardCamera.SetActive(false);
        SettingsCamera.SetActive(true);
    }

    public void GoToMainMenu()
    {
        MainMenuCamera.SetActive(true);
        LeaderboardCamera.SetActive(false);
        SettingsCamera.SetActive(false);
    }

    public void Play()
    {
        SceneManager.LoadScene("CampScene_Version1_Blockout");
    }
}