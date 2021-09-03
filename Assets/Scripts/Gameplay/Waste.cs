using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class Waste : CardContainer {
    
    [SerializeField, Disable] private List<Card> shownCards;
    private const int MAX_Z = 25;
    private const float Y_OFFSET = 0.5f;
    private const float Y_OFFSET_BASE = 2f;
    private const float DRAW_CARD_WAIT_TIME = 0.01f;

    protected override void Awake() {
        base.Awake();
        shownCards = new List<Card>();

        Solitaire.ResetEvent += () => {
            shownCards = new List<Card>();
        };
    }

    public override void AddCard(Card card) {
        base.AddCard(card);

        card.SetPosition(new Vector3(transform.position.x, transform.position.y, cards.Count - MAX_Z));
        card.Hide();
        card.Lock();
    }

    public override void RemoveCard(Card card) {
        base.RemoveCard(card);

        if (shownCards.Contains(card)) {
            shownCards.Remove(card);
            UpdateVisual();
        }
    }

    public void DrawCards(int quantity) {
        if(cards.Count == 0) {
            Debug.LogWarning("No card to draw");

            if (InteractionManager.IsUndoing) {
                Debug.LogWarning("Can't recycle while undoing");
                return;
            }

            Recycle();
            return;
        }

        StartCoroutine(DrawCardsCoroutine(quantity));

    }

    private IEnumerator DrawCardsCoroutine(int quantity) {
        InteractionManager.OpenInteraction();
        for (int i = 0; i < quantity; i++) {
            DrawCard();
            yield return new WaitForSeconds(DRAW_CARD_WAIT_TIME);
        }
        InteractionManager.CloseInteraction();
    }

    private void DrawCard() {
        Card card = cards.Last();

        Debug.Log(string.Format("Drawing card {0}", card.ToString()));

        cards.RemoveAt(cards.Count - 1);
        InteractionManager.AddInteraction(new DrawCardInteraction(card));
        shownCards.Add(card);

        UpdateVisual();
    }

    public void AddCardToShown(Card card) {
        if (card == null) {
            return;
        }

        shownCards.Add(card);
        UpdateVisual();
    }

    public void UnDrawCard() {
        if(shownCards.Count <= 0) {
            Debug.LogWarning("No show card to undraw");
            return;
        }

        Card card = shownCards.Last();

        //Debug.Log(string.Format("UnDrawing card {0}", card.ToString()));

        AddCard(card);
        shownCards.RemoveAt(shownCards.Count - 1);

        UpdateVisual();
    }

    public void UpdateVisual() {

        float start = Time.realtimeSinceStartup;

        if (shownCards == null || shownCards.Count <= 0){
            return;
        }

        int diff = Mathf.Clamp(shownCards.Count - 3, 0, shownCards.Count);

        if (diff > 0) {
            for (int i = diff - 1; i < diff; i++) {
                Card card = shownCards[i];
                card.Lock();
            }
        }

        for (int i = diff; i < shownCards.Count; i++) {
            Card card = shownCards[i];

            card.SetPosition(new Vector3(transform.position.x, transform.position.y - Y_OFFSET_BASE - Y_OFFSET * (i - diff), -i - 1));

            if (i != shownCards.Count - 1) {
                card.Lock();
            } else {
                card.UnLock();
                card.Reveal(true);
            }
        }

        //Debug.Log("Update visual duration " + (Time.realtimeSinceStartup - start) * 1000 + "ms");
    }

    private void Recycle() {
        if(cards.Count > 0) {
            return;
        }

        InteractionManager.OpenInteraction();

        //Debug.Log("Recycling waste cards");

        ScoreManager.RecycleWaste();

        GetComponent<BoxCollider2D>().enabled = false;

        int cardsToUnDraw = shownCards.Count;

        InteractionManager.AddInteraction(new RecycleWasteInteraction());
        for(int i = 0; i < cardsToUnDraw; i++) {
            UnDrawCard();
            UpdateVisual();
        }
        
        GetComponent<BoxCollider2D>().enabled = true;

        InteractionManager.CloseInteraction();
    }

    public void UndoRecycle() {
        if(shownCards.Count > 0) {
            return;
        }
                
        //Debug.Log("Undo recycling waste cards");

        ScoreManager.UndoRecycleWaste();

        GetComponent<BoxCollider2D>().enabled = false;

        int cardsToDraw = cards.Count;

        for (int i = 0; i < cardsToDraw; i++) {
            DrawCard();
        }

        GetComponent<BoxCollider2D>().enabled = true;
    }

    public override Group GetGroup(Card card) {
        if (!shownCards.Contains(card)) {
            return null;
        }

        if (!card.IsVisible) {
            return null;
        }

        Group group = new GameObject("Group").AddComponent<Group>();
        group.AddCard(card);
        return group;
    }

}