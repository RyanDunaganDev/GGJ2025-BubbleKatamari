using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

public class BK_HUD : BK_MasterUI
{
    [SerializeField] private string areaName;

    private VisualElement root; // The element at the root of the hierarchy
    private Label areaNameLabel; // The Label in the top left

    private Label timeLabelMinutes;
    private Label timeLabelSeconds;
    private Label timeLabelMS;
    private Label scoreLabel;

    private void OnEnable()
    {
        // We first get the root visual element from the UIDocument component.
        root = GetComponent<UIDocument>().rootVisualElement;

        // Then we get our VisualElement refs by querying (Q<T>(...) finds a single matching UI Element)
        // We can also use Query<T>(...) to get all matching UI Elements
        // We can query by element name (ex: "area-name"), element style class (ex: ".button"), or element type (ex: Label)
        timeLabelMinutes = root.Q<Label>("time-label-minutes");
        timeLabelSeconds = root.Q<Label>("time-label-seconds");
        timeLabelMS = root.Q<Label>("time-label-ms");
        scoreLabel = root.Q<Label>("score-label");
    }

    private void Start()
    {
        BK_GameState.Instance.OnScoreChanged.AddListener(UpdateScore);
        BK_GameState.Instance.OnTimerChanged.AddListener(UpdateTime);

        UpdateScore(0f);
        UpdateTime(0f);
    }

    private void OnDestroy()
    {
        BK_GameState.Instance.OnScoreChanged.RemoveListener(UpdateScore);
        BK_GameState.Instance.OnTimerChanged.RemoveListener(UpdateTime);
    }

    private void UpdateScore(float newScore)
    {
        scoreLabel.text = newScore.ToString("F2", CultureInfo.InvariantCulture);
    }

    private void UpdateTime(float newTime)
    {
        int minutes = (int)newTime / 60;
        int seconds = (int)newTime - 60 * minutes;
        int milliseconds = (int)(1000 * (newTime - minutes * 60 - seconds)) / 10;
        timeLabelMinutes.text = minutes.ToString("00");
        timeLabelSeconds.text = seconds.ToString("00");
        timeLabelMS.text = milliseconds.ToString("00");
    }
}
