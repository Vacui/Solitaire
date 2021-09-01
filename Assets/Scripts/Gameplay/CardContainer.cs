using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(BoxCollider2D))]
public class CardContainer : MonoBehaviour {

    [SerializeField, Disable] protected List<Card> cards;

    protected virtual void Awake() {
        cards = new List<Card>();

        Solitaire.ResetEvent += () => {
            cards = new List<Card>();
            GetComponent<BoxCollider2D>().enabled = true;
        };
    }

    public virtual void AddCard(Card card) {
        if (cards.Contains(card)) {
            return;
        }

        //Debug.Log(string.Format("Adding card {0} to container {1}", card.ToString(), name));

        cards.Add(card);
    }

    public virtual void RemoveCard(Card card) {
        if (!cards.Contains(card)) {
            return;
        }

        //Debug.Log(string.Format("Removing card {0} from container {1}", card.ToString(), name));

        cards.Remove(card);
    }

    public virtual Group GetGroup(Card card) {
        if (!cards.Contains(card)) {
            return null;
        }

        if (!card.IsVisible) {
            return null;
        }

        Group group = new GameObject("Group").AddComponent<Group>();
        group.AddCard(card);
        return group;
    }

    public virtual void Move(Vector3 newPosition) {
        transform.position = newPosition;
    }

}