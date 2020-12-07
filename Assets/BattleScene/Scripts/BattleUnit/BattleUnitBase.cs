using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class BattleUnitBase : MonoBehaviour
{
    [HideInInspector]public StateController stateController;

    
    [HideInInspector]public NavMeshAgent NavMeshAgent { get;private set;}

    private FightingManager fightingManager;
    private PhotonView photonView;

    public int campId;//陣營Id,用於區分敵我
    //單位都是用武器攻擊敵人，因此抽象出武器類
    public Weapon weapon;
    public BattleUnitBaseProp prop;//单位基础属性
    public GameObject selectMark;

    protected void Awake()
    {
        prop=new BattleUnitBaseProp();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        photonView = GetComponent<PhotonView>();
        weapon = GetComponent<Weapon>();
    }

    // Start is called before the first frame update
    protected void Start()
    {
        UnSelect();
        stateController=new StateController(this);
        fightingManager = GameManager.Instance.GetFightingManager();
    }

    // Update is called once per frame
    protected void Update()
    {
        stateController?.Update();
    }

    /// <summary>
    /// 設置移動位置,和navmesh的setDestion類似
    /// </summary>
    /// <param name="pos"></param>
    public void SetTargetPos(Vector3 pos)
    {
        stateController.SetTargetPos(pos);
    }
    
    #region 鼠标控制
    protected void OnMouseDown()
    {
        //throw new NotImplementedException();
    }

    protected void OnMouseDrag()
    {
        //throw new NotImplementedException();
    }

    protected void OnMouseUp()
    {
        //throw new NotImplementedException();
    }

    protected void OnMouseUpAsButton()
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        Select();
        fightingManager.SelectUnit(this);
    }
    #endregion

    public void SetCampId(int value)
    {
        campId = value;
    }

    public bool IsMine()
    {
        return photonView.IsMine;
    }

    [PunRPC]
    public void RecycleBullet(Bullet bullet)
    {
        bullet.Recycle();
    }

    #region 伤害管理

    public void ReduceHp(int value)
    {
        int leftHp = prop.ReduceHp(value);
        if (leftHp < 0)
        {
            Die();
        }
    }
    
    #endregion

    private void Die()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    private void Select()
    {
        selectMark.gameObject.SetActive(true);
    }

    private void UnSelect()
    {
        selectMark.gameObject.SetActive(false);
    }
}
