using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class GetScore : MonoBehaviour {

    private TMP_Text text;

    private void Awake() {
        text = GetComponent<TMP_Text>();

        ScoreManager.NewScore += (moves) => {
            UpdateValue(moves);
        };
        UpdateValue(ScoreManager.Score);
    }

    private void UpdateValue(int value) {
        if (text == null) {
            Debug.LogWarning("Text is null", gameObject);
            return;
        }

        text.text = value.ToString();
    }

}