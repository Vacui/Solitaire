using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform)), RequireComponent(typeof(Image))]
public class CardImage : MonoBehaviour {

    [SerializeField] private Suits suit;
    [SerializeField] private Numbers number;

    private void Start() {
        Image image = GetComponent<Image>();

        if(image == null) {
            Debug.LogWarning("No Image attached", gameObject);
            return;
        }

        Sprite sprite = CardManager.Generate(true, suit, number);
        image.sprite = sprite;
        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, (sprite.texture.height / (float)sprite.texture.width) * rt.sizeDelta.x);
        image.preserveAspect = true;
    }

}