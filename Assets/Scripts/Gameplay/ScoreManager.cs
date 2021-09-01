using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour {

    private static int score;
    public static int Score {
        get { return score; }
        set {
            score = value < 0 ? 0 : value;
            NewScore?.Invoke(score);
        }
    }
    public static Action<int> NewScore;

    private const int WASTE_FOUNDATION = 10;
    private const int WASTE_PILE = 5;
    private const int PILE_FOUNDATION = 10;
    private const int FOUNDATION_PILE = -15;
    private const int REVEAL_PILE_CARD = 5;
    private const int RECYCLE_WASTE = -100;

    private void Awake() {
        Solitaire.ResetEvent += () => {
            Score = 0;
        };
    }

    public static void CardEnterContainer(CardContainer oldContainer, CardContainer newContainer) {
        if (oldContainer is Waste && newContainer is Foundation) {
            Score += WASTE_FOUNDATION;
        }
        if (oldContainer is Foundation && newContainer is Waste) {
            Score -= WASTE_FOUNDATION;
        }

        if (oldContainer is Waste && newContainer is Pile) {
            Score += WASTE_PILE;
        }
        if (oldContainer is Pile && newContainer is Waste) {
            Score -= WASTE_PILE;
        }

        if (oldContainer is Pile && newContainer is Foundation) {
            Score += InteractionManager.IsUndoing ? -FOUNDATION_PILE : PILE_FOUNDATION;
        }
        if (oldContainer is Foundation && newContainer is Pile) {
            Score += InteractionManager.IsUndoing ? -PILE_FOUNDATION : FOUNDATION_PILE;
        }
    }

    public static void RevealPileCard() {
        Score += REVEAL_PILE_CARD;
    }

    public static void HidePileCard() {
        Score -= REVEAL_PILE_CARD;
    }

    public static void RecycleWaste() {
        Score += GameSettings.DrawThree ? 0 : RECYCLE_WASTE;
    }

    public static void UndoRecycleWaste() {
        Score -= GameSettings.DrawThree ? 0 : RECYCLE_WASTE;
    }

}