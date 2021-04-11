using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GlobalItem : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
  
  [HideInInspector]public GlobalItemType itemType;
  private GlobalItemManager globalItemManager;

  public Image icon;
  public Text nameText;
  public Image fill;
  
  private const string spritePath = "Sprite/GlobalItem/";
  private const string rangeMarkPath = "Prefab/RangeMark/";

  public bool isUsing;//正在使用此道具或技能
 
  public void Init(GlobalItemManager globalItemManager,GlobalItemType itemType)
  {
    this.itemType = itemType;
    this.globalItemManager = globalItemManager;
    GlobalItemConfig config = globalItemManager.GetConfigByType(itemType);
    icon.sprite = Resources.Load<Sprite>(spritePath + config.iconPath);
    nameText.text = config.name;
    fill.fillAmount = 0;
  }

  public void UpdateItem()
  {
    int curPoint = globalItemManager.GetPoint(itemType);
    int needPoint = globalItemManager.GetNeedPoint(itemType);

    var percentage = (curPoint % needPoint) / (float)needPoint;
    fill.fillAmount = percentage;
  }

  public void OnBeginDrag(PointerEventData eventData)
  {
      isUsing = true;
      GameManager.Instance.GetFightingManager().isUsingItem = true;
      //GlobalItemRangeDisplayer.Instance.Display();
  }

  public void OnDrag(PointerEventData eventData)
  {
    //throw new System.NotImplementedException();
    //GlobalItemRangeDisplayer.Instance.Display();
  }

  public void OnEndDrag(PointerEventData eventData)
  {
    //throw new System.NotImplementedException();
    GlobalItemRangeDisplayer.Instance.Hide();
  }

  private GameObject iMark;
  public GameObject GetUsingItemRangeMark()
  {
    if (iMark == null)
    {
      iMark = Instantiate(
        Resources.Load<GameObject>(rangeMarkPath + globalItemManager.GetConfigByType(itemType).rangeMarkPath));
    }

    return iMark;
  }
}
