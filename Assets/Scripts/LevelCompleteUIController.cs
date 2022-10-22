using System;
using UnityEngine;
using UnityEngine.UIElements;
using TaloGameServices;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class PlaceholderExt
{
    public static void SetPlaceholderText(this TextField textField, string placeholder)
    {
        string placeholderClass = TextField.ussClassName + "__placeholder";

        onFocusOut();
        textField.RegisterCallback<FocusInEvent>(evt => onFocusIn());
        textField.RegisterCallback<FocusOutEvent>(evt => onFocusOut());

        void onFocusIn()
        {
            if (textField.ClassListContains(placeholderClass))
            {
                textField.value = string.Empty;
                textField.RemoveFromClassList(placeholderClass);
            }
        }

        void onFocusOut()
        {
            if (string.IsNullOrEmpty(textField.text))
            {
                textField.SetValueWithoutNotify(placeholder);
                textField.AddToClassList(placeholderClass);
            }
        }
    }
}

public class LevelCompleteUIController : MonoBehaviour
{
    private ListView entriesView;
    private VisualElement rootView;
    private TextField nameField;

    private float timeTakenThisLevel;

    public VisualTreeAsset listItemTemplate;

    private int myPlacement = - 1;
    private List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

    async void Start()
    {
        LevelManager.instance.RegisterLevelCompleteUI(this);

        var root = GetComponent<UIDocument>().rootVisualElement;
        entriesView = root.Q<ListView>("scores");

        rootView = root.Q<VisualElement>("level-complete-root");
        rootView.style.opacity = 0f;

        nameField = root.Q<TextField>("player-score-name");
        PlaceholderExt.SetPlaceholderText(nameField, "Your name");
        nameField.style.display = DisplayStyle.None;

        var nextLevelButton = root.Q<Button>("next-level-button");
        nextLevelButton.clicked += () => LevelManager.instance.GoToNextLevel();

        var submitButton = root.Q<Button>("player-score-submit");
        submitButton.clicked += () => SubmitScore();

        rootView.Q<Label>("player-score-placement").style.display = DisplayStyle.None;
        if (PlayerPrefs.HasKey("talo-identity"))
        {
            rootView.Q<VisualElement>("player-score-submit-container").style.display = DisplayStyle.None;
        }

        rootView.style.display = DisplayStyle.None;

        await GetLeaderboardEntries();
    }

    private string GetLeaderboardInternalName()
    {
        return "level-" + LevelManager.instance.GetCurrentLevel() + "-time";
    }

    private async Task GetLeaderboardEntries()
    {
        var res = await Talo.Leaderboards.GetEntries(GetLeaderboardInternalName(), 0);
        entries = new List<LeaderboardEntry>(res.entries);
        UpdateLeaderboardUI(entries);
    }

    private void UpdateLeaderboardUI(List<LeaderboardEntry> entries)
    {
        entriesView.makeItem = () =>
        {
            var elem = listItemTemplate.Instantiate();
            return elem;
        };

        entriesView.bindItem = (e, i) =>
        {
            Color col;

            if (i == myPlacement)
            {
                ColorUtility.TryParseHtmlString("#266089", out col);
                e.style.backgroundColor = col;
            }
            else if (i % 2 != 0)
            {
                ColorUtility.TryParseHtmlString("#1A3D57", out col);
                e.style.backgroundColor = col;
            }
            e.Q<Label>("entry-number").text = (i + 1).ToString();
            e.Q<Label>("entry-name").text = entries[i].playerAlias.identifier;
            e.Q<Label>("entry-time-taken").text = TimeSpan.FromSeconds(entries[i].score).ToString("mm\\:ss\\:ff");
            e.Q<Label>("entry-created").text = DateTime.Parse(entries[i].updatedAt).ToString("dd MMM yyyy");
        };

        entriesView.itemsSource = entries;
        entriesView.selectionType = SelectionType.None;
    }

    public async void Show(float timeTakenThisLevel)
    {
        rootView.style.display = DisplayStyle.Flex;

        if (LevelManager.instance.IsOnFinalLevel())
        {
            rootView.Q<Label>("level-complete").text = "Thanks for playing!";
            rootView.Q<Button>("next-level-button").text = "Play again";
        }

        this.timeTakenThisLevel = timeTakenThisLevel;
        var rotationsThisLevel = Mathf.Floor(this.timeTakenThisLevel / 10);

        rootView.style.opacity = 1f;
        rootView.Q<Label>("player-score-time-rot").text = TimeSpan.FromSeconds(timeTakenThisLevel).ToString("mm\\:ss\\:ff") + " with " + rotationsThisLevel + " rotation" + (rotationsThisLevel != 1 ? "s" : "");

        nameField.style.display = DisplayStyle.Flex;

        if (PlayerPrefs.HasKey("talo-identity"))
        {
            await Talo.Players.Identify("username", PlayerPrefs.GetString("talo-identity"));
            await SendScoreToTalo(); 
        }
    }

    public bool IsVisible()
    {
        return rootView.style.display == DisplayStyle.Flex;
    }

    private async Task SendScoreToTalo()
    {
        var res = await Talo.Leaderboards.AddEntry(GetLeaderboardInternalName(), timeTakenThisLevel);
        var placementLabel = rootView.Q<Label>("player-score-placement");
        placementLabel.style.display = DisplayStyle.Flex;
        placementLabel.text = "You are currently #" + (res.Item1.position + 1);

        myPlacement = res.Item1.position;

        var found = false;

        if (entries.Count > 0)
        {
            entries = entries.Select((e) =>
            {
                if (e.playerAlias.identifier == PlayerPrefs.GetString("talo-identity"))
                {
                    found = true;
                    return res.Item1;
                }
                return e;
            })
               .OrderBy((e) => e.score)
               .ToList();
        }

        if (!found)
        {
            entries.Insert(res.Item1.position, res.Item1);
        }

        UpdateLeaderboardUI(entries);
    }

    private async void SubmitScore()
    {
        rootView.Q<VisualElement>("player-score-submit-container").style.display = DisplayStyle.None;

        PlayerPrefs.SetString("talo-identity", nameField.value);
        PlayerPrefs.Save();

        await Talo.Players.Identify("username", nameField.value);
        await SendScoreToTalo();
    }
}
