using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    private const string PREFS_USERNAME = "username";
    private const string PREFS_HIGHSCORE = "highscore";

    public static GameManager Instance;

    public string Username;

    public string HighscorePlayer = "NONE";
    public int HighscoreValue;

    public TMP_InputField _inputField;
    public Button _buttonStart;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        _inputField.onValueChanged.AddListener(InputField_ValueChanged);

        Load();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.Exit(0);
#else
        Application.Quit();
#endif
    }

    private void InputField_ValueChanged(string text)
    {
        Username = text;

        _buttonStart.interactable = !string.IsNullOrEmpty(text);
    }

    public void Save(int score)
    {
        HighscorePlayer = Username;
        HighscoreValue = score;

        var data = new HighscoreData();
        data.Username = HighscorePlayer;
        data.Highscore = HighscoreValue;

#if UNITY_WEBGL
        PlayerPrefs.SetString(PREFS_USERNAME, HighscorePlayer);
        PlayerPrefs.SetInt(PREFS_HIGHSCORE, HighscoreValue);
        PlayerPrefs.Save();
#else
        var json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
#endif
    }
    public void Load()
    {
        HighscoreData data;

#if UNITY_WEBGL
        data = new HighscoreData();
        data.Username = PlayerPrefs.GetString(PREFS_USERNAME, "NONE");
        data.Highscore = PlayerPrefs.GetInt(PREFS_HIGHSCORE);
#else
        var path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            data = JsonUtility.FromJson<HighscoreData>(json);
        }
#endif

        HighscorePlayer = data.Username;
        HighscoreValue = data.Highscore;
    }

    [System.Serializable]
    public class HighscoreData
    {
        public string Username;
        public int Highscore;
    }
}
