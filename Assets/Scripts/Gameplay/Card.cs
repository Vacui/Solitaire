using UnityEngine;

[RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(BoxCollider2D))]
public class Card : MonoBehaviour {

    private const float WIDTH = 193f;
    private const float HEIGHT = 290f;
    private const float REVEAL_ANIM_DURATION = 0.3f;
    private const float TO_ORIGIN_ANIM_DURATION = 0.3f;
    private const float SCALE = 0.95f;

    [Header("Settings")]
    [SerializeField, DisableInPlayMode] private Suits suit;
    public Suits Suit { get { return suit; } }

    [SerializeField, DisableInPlayMode] private Numbers number;
    public Numbers Number { get { return number; } }

    [SerializeField, Disable] private bool isVisible;
    public bool IsVisible {
        get { return isVisible; }
        set { isVisible = value; }
    }

    [SerializeField, Disable]  private bool isLocked;
    private bool IsLocked {
        get { return isLocked; }
        set {
            isLocked = value;
            GetComponent<BoxCollider2D>().enabled = !value;
        }
    }

    [SerializeField, Disable] private Vector3 originPosition;
    private int moveTweenId;

    [SerializeField, Disable] private CardContainer currentContainer;
    public CardContainer CurrentContainer { get { return currentContainer; } }
    public bool InPile { get { return currentContainer != null && currentContainer is Pile; } }
    public bool InFoundation { get { return currentContainer != null && currentContainer is Foundation; } }

    private void Awake() {
        transform.localScale = Vector2.one * SCALE;
    }

    public void Initialize(bool isVisible, Suits suit, Numbers number) {
        IsVisible = isVisible;
        this.suit = suit;
        this.number = number;

        name = ToString();

        UpdateVisual();
    }

    private void UpdateVisual() {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Sprite card = CardManager.Generate(IsVisible, suit, number);
        sr.sprite = card;
    }

    public Group Select() {
        if (IsLocked) {
            return null;
        }

        if (!IsVisible) {
            Reveal(true);
            return null;
        }

        Debug.Log("Selected", gameObject);

        if (moveTweenId > 0 && LeanTween.isTweening(moveTweenId)) {
            LeanTween.cancel(moveTweenId, false);
        }

        Debug.Log(currentContainer);

        if (currentContainer != null) {
            return currentContainer.GetGroup(this);
        } else {
            Group group = new GameObject("Group").AddComponent<Group>();
            group.AddCard(this);
            return group;
        }
    }

    public void Deselect(bool checkOver, Vector3 checkPosition, Group group) {
        Debug.Log("Deselected", gameObject);

        if (checkOver) {
            IsOverCard(checkPosition, group);
        }
        SetPosition(originPosition);
    }

    private void IsOverCard(Vector3 checkPosition, Group group) {

        Debug.Log("Is over card?", gameObject);

        RaycastHit2D hitInfo = Physics2D.Raycast(checkPosition, Vector2.zero);

        if (hitInfo.collider != null) {

            Card overCard = null;
            CardContainer overContainer = null;

            CardContainer container = null;

            overCard = hitInfo.collider.transform.GetComponent<Card>();
            if (overCard != null) {
                container = OverCard(overCard);
            }

            overContainer = hitInfo.collider.transform.GetComponent<CardContainer>();
            if (overContainer != null) {
                container = OverContainer(overContainer);
            }

            if (container != null) {
                InteractionManager.AddInteraction(new MoveGroupInteraction(currentContainer, container, group));
                group.EnterContainer(container);
            }

            return;

        }
    }

    private CardContainer OverCard(Card card) {
        Debug.Log(card.ToString());

        if (!card.InPile && !card.InFoundation) {
            return null;
        }

        if(currentContainer == card.CurrentContainer) {
            return null;
        }

        if ((card.InPile && !card.Suit.IsColorOpposite(suit)) || (card.InFoundation && card.Suit != suit)) {
            return null;
        }

        Debug.Log("Over suit is correct");

        if ((card.InPile && !card.Number.IsPrevious(number)) || (card.InFoundation && !card.Number.IsNext(number))) {
            return null;
        }

        Debug.Log("Card number is correct");

        return card.CurrentContainer;
    }

    private CardContainer OverContainer(CardContainer container) {

        if (currentContainer == container) {
            return null;
        }

        if (container is Pile) {
            if(number != Numbers.King) {
                return null;
            }
        }
        
        if (container is Foundation) {
            Foundation foundation = container as Foundation;

            if (foundation.Suit != suit) {
                return null;
            }

            if(number != Numbers.Ace) {
                return null;
            }
        }

        return container;

    }

    public void SetPosition(Vector2 position) {
        SetPosition(new Vector3(position.x, position.y, originPosition.z));
    }

    public void SetPosition(Vector3 position, bool useAnim = true) {
        if (!gameObject.activeSelf) {
            useAnim = false;
        }

        Debug.Log(string.Format("Moving {0} from {1} to {2} {3} animation", name, transform.position, position, useAnim ? "with" : "without"), gameObject);
        transform.rotation = Quaternion.identity;
        originPosition = position;

        if (useAnim) {
            moveTweenId = transform.LeanMove(originPosition, TO_ORIGIN_ANIM_DURATION).id;
        } else {
            transform.position = originPosition;
        }
    }

    public void Lock() {
        if (IsLocked) {
            Debug.LogWarning("Can't lock an already locked card");
            return;
        }

        Debug.Log(string.Format("Locking card {0}", name));
        IsLocked = true;
    }

    public void UnLock() {
        if (!IsLocked) {
            Debug.LogWarning("Can't unlock an already unlocked card");
            return;
        }

        Debug.Log(string.Format("UnLocking card {0}", name));
        IsLocked = false;
    }

    public void Hide() {
        if (!IsVisible) {
            Debug.LogWarning("Can't hide an already hided card");
            return;
        }

        if (InPile) {
            ScoreManager.HidePileCard();
        }

        IsVisible = false;
        UpdateVisual();
    }

    public void Reveal(bool useScaleAnim) {
        if (IsLocked) {
            Debug.LogWarning(string.Format("Can't reveal locked card {0}", name));
            return;
        }

        if (IsVisible) {
            Debug.LogWarning("Can't reveal an already revealed card");
            return;
        }

        Debug.Log(string.Format("Revealing card {0} {1} animation", name, useScaleAnim ? "with" : "without"));

        if (InPile) {
            ScoreManager.RevealPileCard();
        }

        if (useScaleAnim) {
            transform.LeanScale(new Vector3(0, SCALE), REVEAL_ANIM_DURATION / 2f).setOnComplete(() => {
                IsVisible = true;
                UpdateVisual();
                transform.LeanScale(Vector3.one * SCALE, REVEAL_ANIM_DURATION / 2f);
            });
        } else {
            IsVisible = true;
            UpdateVisual();
        }
    }

    public void EnterContainer(CardContainer newContainer, bool avoidCheck = false) {
        if(newContainer == currentContainer) {
            return;
        }

        if(currentContainer != null) {
            ScoreManager.CardEnterContainer(currentContainer, newContainer);

            ExitContainer();
        }

        currentContainer = newContainer;
        currentContainer.AddCard(this);
    }

    public void ExitContainer() {
        if(currentContainer == null) {
            return;
        }

        currentContainer.RemoveCard(this);
        currentContainer = null;
    }

    public override string ToString() {
        return string.Format("{0}{1}", number, suit);
    }

    private void OnDestroy() {
        LeanTween.cancel(gameObject);
    }

}