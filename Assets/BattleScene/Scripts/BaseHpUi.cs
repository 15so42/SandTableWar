using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BaseHpUi:MonoBehaviour
{
    [HideInInspector]public BattleUnitBase owner;
    public Text hpNum;
    public Image hpProgress;

    public void UpdateHpUi()
    {
        float percentage = owner.prop.hp / (float)owner.prop.maxHp;
        hpProgress.DOFillAmount(percentage, 0.1f);
        hpNum.text = $"{owner.prop.hp}/{owner.prop.maxHp}";
    }
}