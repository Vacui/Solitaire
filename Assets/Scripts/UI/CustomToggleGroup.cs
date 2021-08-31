using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CustomToggleGroup : ToggleGroup {

    public delegate void ChangedEventHandler(Toggle newActive);

    public event ChangedEventHandler OnChange;
    protected override void Start() {
        base.Start();

        foreach (Transform transformToggle in gameObject.transform) {
            Toggle toggle = transformToggle.gameObject.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((isSelected) => {
                if (!isSelected) {
                    return;
                }
                Toggle activeToggle = Active();
                DoOnChange(activeToggle);
            });
        }
    }
    public Toggle Active() {
        return ActiveToggles().FirstOrDefault();
    }

    protected virtual void DoOnChange(Toggle newactive) {
        OnChange?.Invoke(newactive);
    }

}