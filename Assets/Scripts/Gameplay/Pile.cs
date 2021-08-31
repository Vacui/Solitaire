using System.Linq;
using UnityEngine;

public class Pile : CardContainer {

    private const float Y_OFFSET_HIDDEN = 0.3f;
    private const float Y_OFFSET_VISIBLE = 0.5f;

    private int lastHiddenCardIndex {
        get {
            return cards.Where(c => c.IsVisible == false).Count();
        }
    }

    public override void AddCard(Card card) {

        int lastHiddenIndex = lastHiddenCardIndex;

        Vector3 cardPosition = new Vector3(transform.position.x, transform.position.y - lastHiddenIndex * Y_OFFSET_HIDDEN - (cards.Count - lastHiddenIndex) * Y_OFFSET_VISIBLE, 0 - cards.Count - 1);

        base.AddCard(card);

        Debug.Log(string.Format("Adding card {0} to list position: {1}", card.ToString(), cardPosition));
        card.SetPosition(cardPosition);
    }

    public override void RemoveCard(Card card) {
        base.RemoveCard(card);

        if(cards.Count > 0) {
            card = cards.Last();

            if (card.IsVisible) {
                return;
            }
            InteractionManager.AddInteraction(new UnLockInteraction(card));
            card.UnLock();
            InteractionManager.AddInteraction(new RevealInteraction(card));
            card.Reveal(true);
        }
    }

    public override Group GetGroup(Card card) {
        Group group = base.GetGroup(card);

        if(group == null) {
            return null;
        }

        for(int i = cards.IndexOf(card) + 1; i < cards.Count; i++) {
            group.AddCard(cards[i]);
        }

        return group;

    }

}