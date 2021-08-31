using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class GetMoves : MonoBehaviour {

    private TMP_Text text;

    private void Awake() {
        text = GetComponent<TMP_Text>();

        InteractionManager.NewInteraction += (interactions) => {
            UpdateValue(interactions);
        };
        UpdateValue(InteractionManager.Interactions);
    }

    private void UpdateValue(int value) {
        if(text == null) {
            Debug.LogWarning("Text is null", gameObject);
            return;
        }

        text.text = value.ToString();
    }

}