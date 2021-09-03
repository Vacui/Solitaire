using System.Linq;
using UnityEngine;

public class Foundation : CardContainer {

    [SerializeField, DisableInPlayMode] private Suits suit;
    public Suits Suit { get { return suit; } }

    public bool IsComplete { get { return cards != null && cards.Count > 0 && cards.Last().Number == Numbers.King; } }

    public override void AddCard(Card card) {
        base.AddCard(card);

        if (cards.Count > 1) {
            cards[cards.Count - 2].Lock();
        }

        Vector3 cardPosition = new Vector3(transform.position.x, transform.position.y, 0 - cards.Count);
        card.SetPosition(cardPosition);
        card.UnLock();

        GetComponent<BoxCollider2D>().enabled = cards.Count <= 0;
    }

    public override void RemoveCard(Card card) {
        base.RemoveCard(card);

        if (cards.Count > 0) {
            cards.Last().UnLock();
        }

        GetComponent<BoxCollider2D>().enabled = cards.Count <= 0;
    }

}