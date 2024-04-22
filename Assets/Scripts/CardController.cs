using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    public static CardController Instance;
    public static int gameSize = 2;
    [SerializeField]
    private GameObject Card_Position;
    [SerializeField]
    private GameObject cardList;
    [SerializeField]
    private Sprite cardBack;
    [SerializeField]
    private Sprite[] sprites;
    // list of card
    private MatchCards[] cards;

    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private GameObject info;
    [SerializeField]
    private MatchCards spritePreload;
    [SerializeField]
    private Text sizeLabel;
    [SerializeField]
    private Slider sizeSlider;
    [SerializeField]
    private Text timeLabel;
    private float time;

    private int spriteSelected;
    private int cardSelected;
    private int cardLeft;
    private bool gameStart;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        gameStart = false;
        panel.SetActive(false);
    }
    private void PreloadCardImage()
    {
        for (int i = 0; i < sprites.Length; i++)
            spritePreload.SpriteID = i;
        spritePreload.gameObject.SetActive(false);
    }
    public void StartCardGame()
    {
        if (gameStart) return;
        gameStart = true;
        panel.SetActive(true);
        info.SetActive(false);
        SetGamePanel();
        cardSelected = spriteSelected = -1;
        cardLeft = cards.Length;
        SpriteCardAllocation();
        StartCoroutine(HideFace());
        time = 0;
    }
    private void SetGamePanel()
    {
        int isOdd = gameSize % 2;

        cards = new MatchCards[gameSize * gameSize - isOdd];
        foreach (Transform child in cardList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        RectTransform panelsize = panel.transform.GetComponent(typeof(RectTransform)) as RectTransform;
        float row_size = panelsize.sizeDelta.x;
        float col_size = panelsize.sizeDelta.y;
        float scale = 1.0f / gameSize;
        float xInc = row_size / gameSize;
        float yInc = col_size / gameSize;
        float curX = -xInc * (float)(gameSize / 2);
        float curY = -yInc * (float)(gameSize / 2);

        if (isOdd == 0)
        {
            curX += xInc / 2;
            curY += yInc / 2;
        }
        float initialX = curX;

        for (int i = 0; i < gameSize; i++)
        {
            curX = initialX;
            for (int j = 0; j < gameSize; j++)
            {
                GameObject c;
                if (isOdd == 1 && i == (gameSize - 1) && j == (gameSize - 1))
                {
                    int index = gameSize / 2 * gameSize + gameSize / 2;
                    c = cards[index].gameObject;
                }
                else
                {
                    c = Instantiate(Card_Position);
                    c.transform.parent = cardList.transform;

                    int index = i * gameSize + j;
                    cards[index] = c.GetComponent<MatchCards>();
                    cards[index].ID = index;
                    c.transform.localScale = new Vector3(scale, scale);
                }
                c.transform.localPosition = new Vector3(curX, curY, 0);
                curX += xInc;

            }
            curY += yInc;
        }

    }
    void ResetFace()
    {
        for (int i = 0; i < gameSize; i++)
            cards[i].ResetRotation();
    }
    IEnumerator HideFace()
    {
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < cards.Length; i++)
            cards[i].Flip();
        yield return new WaitForSeconds(0.5f);
    }
    private void SpriteCardAllocation()
    {
        int i, j;
        int[] selectedID = new int[cards.Length / 2];
        for (i = 0; i < cards.Length / 2; i++)
        {
            int value = Random.Range(0, sprites.Length - 1);
            for (j = i; j > 0; j--)
            {
                if (selectedID[j - 1] == value)
                    value = (value + 1) % sprites.Length;
            }
            selectedID[i] = value;
        }
        for (i = 0; i < cards.Length; i++)
        {
            cards[i].Active();
            cards[i].SpriteID = -1;
            cards[i].ResetRotation();
        }
        for (i = 0; i < cards.Length / 2; i++)
            for (j = 0; j < 2; j++)
            {
                int value = Random.Range(0, cards.Length - 1);
                while (cards[value].SpriteID != -1)
                    value = (value + 1) % cards.Length;

                cards[value].SpriteID = selectedID[i];
            }

    }
    public Sprite GetSprite(int spriteId)
    {
        return sprites[spriteId];
    }
    public Sprite CardBack()
    {
        return cardBack;
    }
    public bool canClick()
    {
        if (!gameStart)
            return false;
        return true;
    }
    public void cardClicked(int spriteId, int cardId)
    {
        if (spriteSelected == -1)
        {
            spriteSelected = spriteId;
            cardSelected = cardId;
        }
        else
        { 
            if (spriteSelected == spriteId)
            {
                cards[cardSelected].Inactive();
                cards[cardId].Inactive();
                cardLeft -= 2;
                CheckGameWin();
            }
            else
            {
                cards[cardSelected].Flip();
                cards[cardId].Flip();
            }
            cardSelected = spriteSelected = -1;
        }
    }
    private void CheckGameWin()
    {
        if (cardLeft == 0)
        {
            EndGame();
        }
    }
    private void EndGame()
    {
        gameStart = false;
        panel.SetActive(false);
    }
    public void GiveUp()
    {
        EndGame();
    }
    public void DisplayInfo(bool i)
    {
        info.SetActive(i);
    }
}
