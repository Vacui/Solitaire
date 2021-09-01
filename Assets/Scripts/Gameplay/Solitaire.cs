using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UltEvents;
using UnityEngine;

public class Solitaire : MonoBehaviour {

    private List<Card> cards;

    [SerializeField] private Waste waste;
    [SerializeField] private Pile[] pilesArray;
    private const float SET_PILES_WAIT_TIME = 0.01f;

    public static Action ResetEvent;
    [SerializeField] private UltEvent WinGameEvent;

    public static bool IsPaused { get; private set; }

    public void EndGame() {
        ResetGame();
    }

    public void StartGame() {
        StartGame(false);
    }
    public void RestartGame() {
        StartGame(true);
    }
    private void StartGame(bool restart) {

        Debug.Log("Starting game");

        StopAllCoroutines();

        PauseGame();

        GenerateCards(restart);

        if (!restart) {
            cards = cards.OrderBy(i => Guid.NewGuid()).ToList();
        }

        StartCoroutine(SetPiles());

        SetWaste();

        ResumeGame();
    }

    private void ResetGame() {
        if (cards != null && cards.Count > 0) {
            foreach (Card card in cards) {
                Destroy(card.gameObject);
            }
        }

        cards = new List<Card>();

        ResetEvent?.Invoke();
    }

    private void GenerateCards(bool restart) {
        if(waste == null) {
            Debug.LogWarning("Waste is null", gameObject);
            return;
        }

        List<Card> newCards = new List<Card>();

        if (restart) {
            foreach(Card card in cards) {
                Card newCard = CardManager.GenerateCard(false, card.Suit, card.Number, waste.transform.position);
                newCards.Add(newCard);
            }
        } else {
            foreach (Suits suit in Enum.GetValues(typeof(Suits))) {
                foreach (Numbers number in Enum.GetValues(typeof(Numbers))) {
                    Card newCard = CardManager.GenerateCard(false, suit, number, waste.transform.position);
                    newCards.Add(newCard);
                }
            }
        }

        ResetGame();
        cards = newCards;
    }

    private IEnumerator SetPiles() {
        if(pilesArray == null || pilesArray.Length <= 0) {
            Debug.LogWarning("No piles", gameObject);
            yield break;
        }

        int cardQuantity = 7;
        int cardNum = 0;
        for (int i = pilesArray.Length - 1; i >= 0; i--) {
            for (int c = 0; c < cardQuantity; c++, cardNum++) {
                yield return new WaitForSeconds(SET_PILES_WAIT_TIME);

                if(cards.Count <= cardNum) {
                    Debug.LogWarning(string.Format("Can't move card {0}", cardNum));
                    yield return null;
                }

                Card card = cards[cardNum];

                if (c == cardQuantity - 1) {
                    card.UnLock();
                    card.Reveal(true);
                } else {
                    card.Lock();
                }

                card.EnterContainer(pilesArray[i]);
            }
            cardQuantity--;
        }
    }

    private void SetWaste() {
        if(waste == null) {
            Debug.LogWarning("Waste is null", gameObject);
        }

        Debug.Log(string.Format("Adding {0} cards to the waste", cards.Count - 28));
        for (int i = 28; i < cards.Count; i++) {
            cards[i].EnterContainer(waste);
        }
        waste.UpdateVisual();
    }

    public void CheckWin() {
        bool win = false;

        foreach(Foundation foundation in FindObjectsOfType<Foundation>()) {
            win = foundation.IsComplete;

            if (!win) {
                break;
            }
        }

        if (win) {
            WinGameEvent?.Invoke();
        }
    }

    public void PauseGame() {
        Debug.Log("Pausing game");

        IsPaused = true;
    }

    public void ResumeGame() {
        Debug.Log("Resuming game");

        IsPaused = false;
    }

}