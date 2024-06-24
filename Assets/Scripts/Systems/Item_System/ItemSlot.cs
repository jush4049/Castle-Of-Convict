using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image image;
    [SerializeField] Image background;
    [SerializeField] Sprite blankImage;

    public RectTransform itemInfo;
    public GameObject _itemInfo;

    public TMP_Text itemNameText;
    public TMP_Text itemInfoText;

    void Awake()
    {
        _itemInfo = GameObject.Find("ItemInfo");
        //_itemInfo.SetActive(false);
        background.sprite = blankImage;
    }

    private Item _item;
    public Item item
    {
        get { return _item; }
        set
        {
            _item = value;
            if (_item != null)
            {
                image.sprite = item.itemImage;
                image.color = new Color(1, 1, 1, 1);
                if (item.itemValue != null && item.itemValue == "Key")
                {
                    Debug.Log("아이템(Key)이 지급되었습니다.");
                    GameManager.GetKey();
                }
            }
            else
            {
                image.color = new Color(1, 1, 1, 0);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item.itemValue == "Blank")
        {
            Debug.Log("아이템 없음");
            return;
        }
        if (item.itemValue != "Blank")
        {
            _itemInfo.SetActive(true);
            Vector2 mousePos = Input.mousePosition;
            float w = itemInfo.rect.width;
            float h = itemInfo.rect.height;
            itemInfo.position = mousePos + (new Vector2(250, -300));
            itemNameText.text = item.itemName;
            itemInfoText.text = item.itemDescription;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item.itemValue != "Blank")
        {
            _itemInfo.SetActive(false);
            Debug.Log("해제");
        }
    }
}