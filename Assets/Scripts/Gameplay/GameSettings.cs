using System;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour {

    private const string COLOR_KEY = "color";

    private Toggle currentBackgroundToggle;

    [SerializeField] private Color[] colors;
    private int colorIndex {
        get {
            return Mathf.Clamp(PlayerPrefs.GetInt(COLOR_KEY), 0, colors.Length - 1);
        }
        set {

            if (colors == null || colors.Length <= 0) {
                Debug.LogWarning("No colors");
                return;
            }

            Debug.Log("Set color: " + value);

            if (value >= colors.Length) {
                value = 0;
            } else if (value < 0) {
                value = colors.Length - 1;
            }

            Camera.main.backgroundColor = colors[value];

            PlayerPrefs.SetInt(COLOR_KEY, value);

            NewColor?.Invoke(Color);
        }
    }
    public Color Color {
        get {
            return colors[colorIndex];
        }
    }

    private const string DRAW_MODE_KEY = "draw";
    public static bool DrawThree {
        get {
            return PlayerPrefs.GetInt(DRAW_MODE_KEY) == 1;
        }
        set {
            PlayerPrefs.SetInt(DRAW_MODE_KEY, value ? 1 : 0);
        }
    }

    public static Action<Color> NewColor;

    public void NextColor() {
        colorIndex++;
    }

    public void PrevColor() {
        colorIndex--;
    }

    public void SetDrawMode(bool draw) {
        DrawThree = draw;
    }

}