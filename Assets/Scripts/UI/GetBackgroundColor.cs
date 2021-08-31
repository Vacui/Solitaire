using UnityEngine;
using UnityEngine.UI;

public class GetBackgroundColor : MonoBehaviour {

    private void Awake() {
        GameSettings.NewColor += (color) => {
            Debug.Log("Updating color", gameObject);
            GetComponent<Image>().color = color;
        };
        GetComponent<Image>().color = GameSettings.Instance.Color;
    }

}