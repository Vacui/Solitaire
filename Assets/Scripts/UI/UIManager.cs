using UnityEngine;

[System.Serializable]
public class CardContainerPosition {
    public CardContainer container;
    public Vector3 portrait;
    public Vector3 landscape;

    public void SetPortrait() {
        if(container == null) {
            Debug.LogWarning("Container is null");
            return;
        }

        container.Move(portrait);
    }

    public void SetLandscape() {
        if (container == null) {
            Debug.LogWarning("Container is null");
            return;
        }

        container.Move(landscape);
    }

}

public class UIManager : MonoBehaviour {

    [SerializeField] private GameObject optionsLandscape;
    [EditorButton(nameof(ToggleOrientation), "Toggle Orientation", ButtonActivityType.OnPlayMode)]
    [SerializeField] private GameObject optionsPortrait;

    [SerializeField] private CardContainerPosition wastePosition;
    [SerializeField] private CardContainerPosition foundationHeartsPosition;
    [SerializeField] private CardContainerPosition foundationDiamondsPosition;
    [SerializeField] private CardContainerPosition foundationClubsPosition;
    [SerializeField] private CardContainerPosition foundationSpadesPosition;

    [SerializeField, Disable] private DeviceOrientation currentOrientation;
    private DeviceOrientation CurrentOrientation {
        get { return currentOrientation; }
        set {
            if (value == DeviceOrientation.Unknown || currentOrientation == value) {
                return;
            }

            currentOrientation = value;

            if (currentOrientation == DeviceOrientation.LandscapeLeft || currentOrientation == DeviceOrientation.LandscapeRight) {
                SetLandscape();
            } else if (currentOrientation == DeviceOrientation.Portrait || currentOrientation == DeviceOrientation.PortraitUpsideDown) {
                SetPortrait();
            }
        }
    }

    private void Awake() {
        CurrentOrientation = Input.deviceOrientation;
        CurrentOrientation = DeviceOrientation.Portrait;
        Debug.Log(currentOrientation);
    }

    private void Update() {
        if (Input.deviceOrientation == currentOrientation) {
            return;
        }

        CurrentOrientation = Input.deviceOrientation;
    }

    private void SetLandscape() {
        optionsLandscape.SetActive(true);
        optionsPortrait.SetActive(false);
        Camera.main.orthographicSize = 5;
        Camera.main.transform.position = new Vector3(0, 0.4f, Camera.main.transform.position.z);

        wastePosition.SetLandscape();
        foundationClubsPosition.SetLandscape();
        foundationDiamondsPosition.SetLandscape();
        foundationHeartsPosition.SetLandscape();
        foundationSpadesPosition.SetLandscape();
    }

    private void SetPortrait() {
        optionsLandscape.SetActive(false);
        optionsPortrait.SetActive(true);
        Camera.main.orthographicSize = 7;
        Camera.main.transform.position = new Vector3(0, 0, Camera.main.transform.position.z);

        wastePosition.SetPortrait();
        foundationClubsPosition.SetPortrait();
        foundationDiamondsPosition.SetPortrait();
        foundationHeartsPosition.SetPortrait();
        foundationSpadesPosition.SetPortrait();
    }

    private void ToggleOrientation() {
        Debug.Log("Toggle orientation");
        if(CurrentOrientation == DeviceOrientation.Portrait) {
            CurrentOrientation = DeviceOrientation.LandscapeLeft;
        }
        else if(CurrentOrientation == DeviceOrientation.LandscapeLeft) {
            CurrentOrientation = DeviceOrientation.Portrait;
        }
    }

}