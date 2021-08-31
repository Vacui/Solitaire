using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour {

    private const string COLOR_KEY = "color";

    public static GameSettings Instance = null;

    private Toggle currentBackgroundToggle;

    [SerializeField] private Color[] colors;
    public Color Color {
        get {
            return colors[toggles.IndexOf(currentBackgroundToggle)];
        }
    }
    [SerializeField] private List<Toggle> toggles;
    [SerializeField] private CustomToggleGroup toggleGroup;

    private const string DRAW_MODE_KEY = "draw";
    public static bool DrawThree {
        get {
            return PlayerPrefs.GetInt(DRAW_MODE_KEY) == 1;
        }
        set {
            Debug.Log(value);
            PlayerPrefs.SetInt(DRAW_MODE_KEY, value ? 1 : 0);
        }
    }

    public static Action<Color> NewColor;

    private void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(this);
        }

        Instance = this;
    }

    private void Start() {
        toggleGroup.OnChange += (toggle) => SetBackgroundColor(toggle);
        UpdateVisuals();
    }

    public void UpdateVisuals() {
        int colorIndex = PlayerPrefs.GetInt(COLOR_KEY);
        toggles[colorIndex].isOn = true;
        SetBackgroundColor(colorIndex);
    }

    private void SetBackgroundColor(Toggle toggle) {
        SetBackgroundColor(toggles.IndexOf(toggle));
    }

    public void SetBackgroundColor(int index) {
        if(currentBackgroundToggle != null && currentBackgroundToggle != toggles[index]) {
            currentBackgroundToggle.isOn = false;
        }

        currentBackgroundToggle = toggles[index];

        if(colors == null || colors.Length <= index) {
            Debug.LogWarning("No colors");
            return;
        }

        Debug.Log("Set color: " + index);

        Color color = colors[index];
        Camera.main.backgroundColor = colors[index];
        PlayerPrefs.SetInt(COLOR_KEY, index);
        NewColor?.Invoke(color);
    }

    public void SetDrawMode(bool draw) {
        DrawThree = draw;
    }

}