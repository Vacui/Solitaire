using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class GetDrawMode : MonoBehaviour {

    private void Awake() {
        GetComponent<Toggle>().isOn = GameSettings.DrawThree;
    }

}