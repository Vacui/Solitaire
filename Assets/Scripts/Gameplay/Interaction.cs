using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InteractionGroup {
    private List<Interaction> interactions;

    public bool IsEmpty { get { return interactions == null || interactions.Count <= 0; } }

    public void AddInteraction(Interaction interaction) {
        Debug.Log(string.Format("Add interaction {0}", interaction.ToString()));

        if (interaction == null) {
            return;
        }

        if (interactions == null) {
            interactions = new List<Interaction>();
        }

        interactions.Add(interaction);
    }

    public void UndoInteractions() {
        Debug.Log("Undo group interactions");

        foreach (Interaction subInteraction in interactions) {
            if(subInteraction == null) {
                Debug.LogWarning("Sub Interaction is null");
                continue;
            }

            subInteraction.Undo();
        }
    }

}

[System.Serializable]
public class Interaction {

    public virtual void Undo() {
        Debug.Log(string.Format("Undo interaction {0}", ToString()));
    }

}

[System.Serializable]
public class MoveGroupInteraction : Interaction {

    public CardContainer prevContainer;
    public CardContainer newContainer;
    public List<Card> cardsGroup;

    public MoveGroupInteraction(CardContainer prevContainer, CardContainer newContainer, Group group) {
        this.prevContainer = prevContainer;
        this.newContainer = newContainer;
        cardsGroup = group.Cards;
    }

    public override void Undo() {
        base.Undo();

        foreach (Card card in cardsGroup) {
            card.EnterContainer(prevContainer);
            if (prevContainer is Waste) {
                (prevContainer as Waste).DrawCard(true);
            }
        }
    }

    public override string ToString() {
        return string.Format("Moving group of {0} cards from {1} to {2}", cardsGroup.Count, prevContainer, newContainer);
    }

}

[System.Serializable]
public class RevealInteraction : Interaction {

    public Card card;

    public RevealInteraction(Card card) {
        this.card = card;
    }

    public override void Undo() {
        base.Undo();
        card.Hide();
    }

    public override string ToString() {
        return string.Format("Revealing card {0}", card.ToString());
    }

}

[System.Serializable]
public class UnLockInteraction : Interaction {

    public Card card;

    public UnLockInteraction(Card card) {
        this.card = card;
    }

    public override void Undo() {
        base.Undo();
        card.Lock();
    }

    public override string ToString() {
        return string.Format("UnLocking card {0}", card.ToString());
    }

}

[System.Serializable]
public class DrawCardInteraction : Interaction {

    public Card card;

    public DrawCardInteraction(Card card) {
        this.card = card;
    }

    public override void Undo() {
        base.Undo();
        Object.FindObjectOfType<Waste>().UnDrawCard(true);
    }

    public override string ToString() {
        return string.Format("Drawing card {0} from waste", card.ToString());
    }

}

public class RecycleWasteInteraction : Interaction {

    public override void Undo() {
        base.Undo();
        Object.FindObjectOfType<Waste>().UndoRecycle();
    }

    public override string ToString() {
        return "Resetting waste";
    }

}