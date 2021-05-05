using System;
using BattleScene.Scripts;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BaseHpUi:MonoBehaviour
{
    private BattleUnitBase owner;
    [SerializeField]private Text hpNum;
    [SerializeField]private Image hpProgress;
    [SerializeField]private Image unitIconBg;
    [SerializeField] private Image unitIcon;
    [SerializeField] private GameObject container;

    private Color factionColor;
    private Vector3 offset;
    private RecycleAbleObject recycleAbleObject;
    private Camera mainCamera;
    
    private bool isShow;

    private void Awake()
    {
        
    }

    public void Init(BattleUnitBase battleUnitBase, Color factionColor, Vector3 offset)
    {
        this.factionColor = factionColor;
        hpProgress.color = this.factionColor;
        this.offset = offset;
        
        owner = battleUnitBase;
        Sprite configType=UnitIconLoader.GetSpriteByUnitId(owner.configId);
        UpdateHpUi();
        Show(false,true);
        if (configType == null)
        {
            unitIcon.gameObject.SetActive(false);
        }

        unitIcon.sprite = configType;
        recycleAbleObject = GetComponent<RecycleAbleObject>();
        mainCamera = Camera.main;
        owner.OnHpChanged.AddListener(UpdateHpUi);
        Show(true,false);
    }
    public void UpdateHpUi()
    {
        float percentage = owner.prop.hp / (float)owner.prop.maxHp;
        hpProgress.DOFillAmount(percentage, 0.1f);
        hpNum.text = $"{owner.prop.hp}/{owner.prop.maxHp}";
    }

    public void Update()
    {
        UpdatePos();
    }

    private void UpdatePos()
    {
        transform.position = mainCamera.WorldToScreenPoint(owner.transform.position) + offset;
    }

    public void Show(bool value,bool force=false)
    {
        bool canShow = false;
        if (force==false)
        {
            if (owner.IsInFog()==false)
            {
                canShow = true;
            }
        }

        if (value  && canShow)
        {
            container.SetActive(true);
            isShow = true;
        }

        if (value == false)
        {
            container.SetActive(false);
            isShow = false;
        }
    }

    public bool GetShowingStatus()
    {
        return isShow;
    }
    public void Destroy()
    {
        owner.OnHpChanged.RemoveListener(UpdateHpUi);
        recycleAbleObject.Recycle();
    }
}