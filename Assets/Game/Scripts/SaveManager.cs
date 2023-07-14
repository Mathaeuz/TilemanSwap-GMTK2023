using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveManager : Singleton<SaveManager>
{
    public CanvasGroup IntroMenu;
    HistoryData History = new();
    public Button LoadGamePrefab, NewGame;
    List<Button> ActiveSaveButtons = new();
    public int MaxSaveHistory = 5;
    private float FadeLength = 0.3f;
    private bool JustLoadedSave = false;
    public AudioClip MenuBeep;
    private List<EventTrigger.Entry> BeepTrigger;

    const int SaveVersion = 1;

    [Serializable]
    public class SaveData
    {
        public Vector3 Position;
        public int[] Swaps;
        public string Name;
        public long DateTime;
    }

    [Serializable]
    public class HistoryData
    {
        public int Version;
        public List<SaveData> Saves = new();
    }

    private void Awake()
    {
        IntroMenu.alpha = 0;
        LoadHistory();
        BuildMenu();
        StartCoroutine(Fade(1));
    }

    private void BuildMenu()
    {
        var ev = new EventTrigger.TriggerEvent();
        ev.AddListener(PlayBeep);
        BeepTrigger = new List<EventTrigger.Entry>() {new EventTrigger.Entry
        {
            callback = ev,
            eventID = EventTriggerType.PointerEnter,
        } };
        NewGame.onClick.AddListener(NewSelect);
        NewGame.GetComponent<EventTrigger>().triggers = BeepTrigger;
        BuildSaveButtons();
        SetInteractionsActive(true);
    }

    private void PlayBeep(BaseEventData arg0)
    {
        if (IntroMenu.interactable)
            SharedSoundEmiter.Instance.Play(MenuBeep);
    }

    private void BuildSaveButtons()
    {
        for (int i = 0; i < ActiveSaveButtons.Count; i++)
        {
            Destroy(ActiveSaveButtons[i].gameObject);
        }

        for (int i = 0; i < History.Saves.Count; i++)
        {
            var index = i;
            var button = Instantiate(LoadGamePrefab);
            button.onClick.AddListener(() => LoadSelect(index));
            ActiveSaveButtons.Add(button);
            button.GetComponentInChildren<Text>().text = $"{DateTime.FromBinary(History.Saves[i].DateTime)} - {History.Saves[i].Name}";
            button.transform.SetParent(LoadGamePrefab.transform.parent);
            button.gameObject.SetActive(true);
            button.GetComponent<EventTrigger>().triggers = BeepTrigger;
        }
    }

    void NewSelect()
    {
        SetInteractionsActive(false);
        StartCoroutine(Fade(0));
        PlayerSpawn.Instance.SpawnPlayer();
    }

    private void SetInteractionsActive(bool value)
    {
        IntroMenu.blocksRaycasts = value;
    }

    void LoadSelect(int i)
    {
        SetInteractionsActive(false);
        StartCoroutine(Fade(0));
        PlayerSpawn.Instance.SpawnPlayer(History.Saves[i].Position);
        MusicPlayer.Instance.StartRandom = true;
        MusicPlayer.Instance.Play();
        var swaps = RoleManager.Instance.BuildSwapsFromIndex(History.Saves[i].Swaps);
        RoleManager.Instance.RollbackSwaps(swaps);
        JustLoadedSave = true;
    }

    private void LoadHistory()
    {
        var json = PlayerPrefs.GetString(nameof(History));
        if (string.IsNullOrEmpty(json))
        {
            History = new HistoryData { Version = SaveVersion };
            return;
        }
        History = JsonUtility.FromJson<HistoryData>(json);
        if (History == null || History.Saves == null || History.Version != SaveVersion)
        {
            History = new HistoryData { Version = SaveVersion };
            return;
        }
    }

    private void SaveHistory()
    {
        PlayerPrefs.SetString(nameof(History), JsonUtility.ToJson(History));
    }

    public void NewHistory(Role[] Swaps, Vector3 position, string name)
    {
        if (JustLoadedSave)
        {
            JustLoadedSave = false;
            return;
        }

        History.Saves.Insert(0, new SaveData
        {
            Name = name,
            Position = position,
            Swaps = RoleManager.Instance.BuildIndexFromSwaps(Swaps),
            DateTime = DateTime.Now.Ticks
        });
        if (History.Saves.Count > MaxSaveHistory)
        {
            History.Saves.RemoveAt(History.Saves.Count - 1);
        }
        SaveHistory();
    }

    private IEnumerator Fade(float target)
    {
        var origin = IntroMenu.alpha;
        var timer = FadeLength;
        while (timer > 0)
        {
            IntroMenu.alpha = Mathf.Lerp(target, origin, timer / FadeLength);
            timer -= Time.unscaledDeltaTime;
            yield return 0;
        }
        IntroMenu.alpha = target;
    }
}