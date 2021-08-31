using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour {

    [SerializeField, Disable] List<Card> cards;
    public List<Card> Cards { get { return cards; } }
    List<Rigidbody2D> cardsRb;

    private const float Y_OFFSET = 0.3f;
    private const float CARD_MOVE_SPEED = 6.5f;
    private const float MOVE_WAIT_TIME = 0.01f;
    private const float MOVE_Z = -25f;

    private void Awake() {
        cards = new List<Card>();
        cardsRb = new List<Rigidbody2D>();
    }

    public void AddCard(Card card) {
        if (cards.Contains(card)) {
            return;
        }

        card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y, MOVE_Z - cards.Count);

        cards.Add(card);
        card.Lock();

        cardsRb.Add(card.GetComponent<Rigidbody2D>());

    }

    public void MoveTo(Vector3 position) {
        StartCoroutine(MoveToCoroutine(position));
    }

    private IEnumerator MoveToCoroutine(Vector3 position) {
        for (int i = 0; i < cardsRb.Count; i++) {
            yield return new WaitForSeconds(MOVE_WAIT_TIME);
            Rigidbody2D cardRb = cardsRb[i];
            Vector3 targetPosition = new Vector3(position.x, position.y - Y_OFFSET * i, cards[i].transform.position.z);
            Vector2 direction = (targetPosition - cardRb.transform.position);
            cardRb.velocity = new Vector2(direction.x, direction.y) * CARD_MOVE_SPEED;
        }
    }

    public void Deselect(Vector3 position) {
        StopAllCoroutines();

        InteractionManager.OpenInteraction();
        for(int i = 0; i < cards.Count; i++) {
            Card card = cards[i];
            card.Deselect(i == 0, position, this);
            card.UnLock();
            cardsRb[i].velocity = Vector2.zero;
        }
        InteractionManager.CloseInteraction();

        Destroy(gameObject);
    }

    public void EnterContainer(CardContainer container) {
        foreach(Card card in cards) {
            card.EnterContainer(container);
            card.UnLock();
        }
    }

}