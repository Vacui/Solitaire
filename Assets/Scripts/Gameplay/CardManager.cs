using System.Collections.Generic;
using UnityEngine;
using Utils;

public enum Suits {
    Hearts,
    Diamonds,
    Clubs,
    Spades
}

public static class SuitsExtensions {
    
    public static Color ToColor(this Suits suit) {
        switch (suit) {
            case Suits.Hearts: return new Color32(219, 15, 53, 255);
            case Suits.Diamonds: return new Color32(219, 15, 53, 255);
            case Suits.Clubs: return new Color32(57, 47, 82, 255);
            case Suits.Spades: return new Color32(57, 47, 82, 255);
        }

        return Color.white;
    }

    public static bool IsColorOpposite(this Suits suit, Suits target) {
        return suit.ToColor() != target.ToColor();
    }

    public static bool IsColorEqual(this Suits suit, Suits target) {
        return suit.ToColor() == target.ToColor();
    }

}

public enum Numbers {
    Ace,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King
}

public static class NumbersExtensions {

    public static bool IsPrevious(this Numbers suit, Numbers target) {
        if(target == Numbers.King) {
            return false;
        }
        if(suit == target) {
            return false;
        }
        return suit - 1 == target;
    }

    public static bool IsNext(this Numbers suit, Numbers target) {
        if(target == Numbers.Ace) {
            return false;
        }
        if(suit == target) {
            return false;
        }
        return suit + 1 == target;
    }

}

public class CardManager : MonoBehaviour {

    [System.Serializable]
    public class TextureNumber {
        public Numbers number;
        public Texture2D tex;
    }

    [System.Serializable]
    public class TextureSuit {
        public Suits suit;
        public Texture2D tex;
    }

    [SerializeField, NotNull] private Card cardPrefab;

    [SerializeField, NotNull] private Texture2D baseTex;
    [SerializeField, NotNull] private Texture2D frontTex;
    [SerializeField, NotNull] private Texture2D backTex;

    [SerializeField, ReorderableList] private TextureSuit[] texSuitsArray;
    private static Dictionary<Suits, Texture2D> texSuitsDictionary;
    [SerializeField, ReorderableList] private TextureNumber[] texNumbersArray;
    private static Dictionary<Numbers, Texture2D> texNumbersDictionary;

    private void Awake() {
        GenerateDictionaries();
    }

    private void GenerateDictionaries() {
        texNumbersDictionary = new Dictionary<Numbers, Texture2D>();
        for (int i = 0; i < texNumbersArray.Length; i++) {
            if (texNumbersDictionary.ContainsKey(texNumbersArray[i].number)) {
                continue;
            }

            texNumbersDictionary.Add(texNumbersArray[i].number, texNumbersArray[i].tex);
        }

        texSuitsDictionary = new Dictionary<Suits, Texture2D>();
        for (int i = 0; i < texSuitsArray.Length; i++) {
            if (texSuitsDictionary.ContainsKey(texSuitsArray[i].suit)) {
                continue;
            }

            texSuitsDictionary.Add(texSuitsArray[i].suit, texSuitsArray[i].tex);
        }
    }

    public static Card GenerateCard(bool isVisible, Suits suit, Numbers number, Vector2 worldPosition) {

        if(Instances.CardManager == null) {
            return null;
        }

        Card newCard = Instantiate(Instances.CardManager.cardPrefab.gameObject).GetComponent<Card>();
        newCard.Initialize(isVisible, suit, number);
        newCard.transform.position = worldPosition;

        return newCard;

    }

    public static Sprite Generate(bool isVisible, Suits suit, Numbers number) {

        if (Instances.CardManager == null) {
            return null;
        }

        //Debug.Log(string.Format("Generate card {0} {1}", suit, number));

        Texture2D tex = UtilsClass.ClearTexture(Instances.CardManager.baseTex.width, Instances.CardManager.baseTex.height);

        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        List<Color[]> layers = new List<Color[]>();
        Color[] colorsArray = tex.GetPixels();

        layers.Add(CreateLayer(tex, Instances.CardManager.baseTex, 0, 0, 0, 0, 1, Instances.GameSettings.Color));
        layers.Add(CreateLayer(tex, isVisible ? Instances.CardManager.frontTex : Instances.CardManager.backTex, 0, 0, 0, 0));

        if (isVisible) {
            layers.Add(CreateLayer(tex, texSuitsDictionary[suit], -1, -1, 23, -1, 1f, suit.ToColor()));
            layers.Add(CreateLayer(tex, texNumbersDictionary[number], 13, -1, -1, 13, 1f, suit.ToColor()));
            layers.Add(CreateLayer(tex, texSuitsDictionary[suit], 13, 13, -1, -1, 0.6f, suit.ToColor()));
        }

        for(int x = 0; x < tex.width; x++) {
            for(int y = 0; y < tex.height; y++) {
                int pixelIndex = x + (y * tex.width);
                for(int i = 0; i < layers.Count; i++) {
                    Color colorPixel = colorsArray[pixelIndex];
                    Color layerPixel = layers[i][pixelIndex];
                    if (colorPixel.a == 0 || layerPixel.a == 1) {
                        colorsArray[pixelIndex] = layerPixel;
                    } else if (layerPixel.a > 0) {
                        colorsArray[pixelIndex] = UtilsClass.ColorBlendNormal(layerPixel, colorPixel);
                    }
                }
            }
        }

        tex.SetPixels(colorsArray);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);

    }

    private static Color[] CreateLayer(Texture2D parentTex, Texture2D tex, int top, int right, int bottom, int left) {
        return CreateLayer(parentTex, tex, top, right, bottom, left, 1f, false, Color.white);
    }

    private static Color[] CreateLayer(Texture2D parentTex, Texture2D tex, int top, int right, int bottom, int left, float scale) {
        return CreateLayer(parentTex, tex, top, right, bottom, left, scale, false, Color.white);
    }

    private static Color[] CreateLayer(Texture2D parentTex, Texture2D tex, int top, int right, int bottom, int left, float scale, Color color) {
        return CreateLayer(parentTex, tex, top, right, bottom, left, scale, true, color);
    }

    private static Color[] CreateLayer(Texture2D parentTex, Texture2D tex, int top, int right, int bottom, int left, float scale, bool useColor, Color color) {

        Texture2D layerTex = new Texture2D(tex.width, tex.height);
        layerTex.SetPixels(tex.GetPixels());
        layerTex.Apply();

        if (scale != 1f) {
            TextureScale.Bilinear(layerTex, Mathf.FloorToInt(layerTex.width * scale), Mathf.FloorToInt(layerTex.height * scale));
        }

        if (top < 0 && bottom < 0) {
            // centered vertically
            int halfVertical = parentTex.height - layerTex.height;
            top = bottom = halfVertical / 2;
        }

        if (right < 0 && left < 0) {
            // centered horizontally
            int halfHorizontal = parentTex.width - layerTex.height;
            right = left = halfHorizontal / 2;
        }

        if (top < 0) {
            top = parentTex.height - layerTex.height - bottom;
        }

        if (bottom < 0) {
            bottom = parentTex.height - layerTex.height - top;
        }

        if (right < 0) {
            right = parentTex.width - layerTex.width - left;
        }

        if (left < 0) {
            left = parentTex.width - layerTex.width - right;
        }

        Texture2D sizedTex = UtilsClass.ClearTexture(parentTex.width, parentTex.height);
        sizedTex.SetPixels(left, bottom, layerTex.width, layerTex.height, layerTex.GetPixels());

        Color[] colorsArray = sizedTex.GetPixels();

        if (useColor) {
            for(int i = 0; i < colorsArray.Length; i++) {
                colorsArray[i] = new Color(color.r, color.g, color.b, colorsArray[i].a);
            }
        }

        return colorsArray;

    }

    private void ApplyTexture(Texture2D baseTex, Texture2D addTex) {
        Color[] colorArray = addTex.GetPixels();
        baseTex.SetPixels(colorArray);
        baseTex.Apply();
    }

}