using UnityEngine;

public class Instances : MonoBehaviour {

    private static Instances Instance { get; set; }

    public static CardManager CardManager { get; private set; }
    public static Solitaire Solitaire { get; private set; }
    public static InputController InputController { get; private set; }
    public static GameSettings GameSettings { get; private set; }

    private void Awake() {

        if(Instance != null && Instance != this) {
            Destroy(gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        GetReferences();

    }

    private void GetReferences() {
        CardManager = GetComponent<CardManager>();
        Solitaire = GetComponent<Solitaire>();
        InputController = GetComponent<InputController>();
        GameSettings = GetComponent<GameSettings>();
    }

}