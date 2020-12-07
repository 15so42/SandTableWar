using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityTimer;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MainMenuManager : MonoBehaviourPunCallbacks,IOnEventCallback
{
	[SerializeField]private Button matchBtn;//匹配按钮，点击匹配按钮后开始匹配
	[SerializeField]private Text loadingTips;//显示服务器连接状态

	[SerializeField]private Image matchTimePanel;
	[SerializeField]private Text matchTimeText;
	[SerializeField]private Button cancelMatchBtn;
    //准备界面
    [SerializeField]private Transform readyPanel;
    [SerializeField]private Button readyBtn;
    [SerializeField]private Button cancelReadyBtn;

    private bool isPlayerReady;

    //连接至国区服务器
    private void ConnectToCNServer()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "cn";
        PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
        PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = GameSettingsConst.appId;
        PhotonNetwork.PhotonServerSettings.AppSettings.Server = "ns.photonengine.cn";
        PhotonNetwork.ConnectUsingSettings();
    }
    // Start is called before the first frame update
    public void Init()
    {
	    //不是从StartScene场景中进入的
	    if (GameManager.Instance.sceneStateController == null)
	    {
		    SceneManager.LoadScene("StartScene");
	    }

	    // 匹配按鈕
        matchBtn.gameObject.SetActive(false);
        //匹配计时面板
        matchTimePanel.gameObject.SetActive(false);
        //准备面板
        readyPanel.gameObject.SetActive(false);
        RegisterEvent();
        //连接服务器
        ConnectToCNServer(); 
        
    }

    void RegisterEvent()
    {
	    matchBtn.onClick.AddListener(OnMatchGameBtnClick);
	    cancelMatchBtn.onClick.AddListener(OnCancelMatchBtnClick);
	    readyBtn.onClick.AddListener(OnReadyBtnClick);
	    cancelReadyBtn.onClick.AddListener(OnCancelReadyBtnClick);
    }

    void UnRegisterEvent()
    {
	    matchBtn.onClick.RemoveListener(OnMatchGameBtnClick);
	    readyBtn.onClick.RemoveListener(OnReadyBtnClick);
	    cancelReadyBtn.onClick.RemoveListener(OnCancelReadyBtnClick);
    }
    
    // 開始匹配
    private void OnMatchGameBtnClick()
    {
	    PhotonNetwork.JoinRandomRoom();
    }

    private void OnReadyBtnClick()
    {
	    Hashtable props = new Hashtable() {{GameManager.PLAYER_READY, true}};
	    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    private void OnCancelMatchBtnClick()
    {
	    PhotonNetwork.LeaveRoom();
    }

    private void OnCancelReadyBtnClick()
    {

	    PhotonNetwork.LeaveRoom();
	    readyPanel.gameObject.SetActive(false);
	    
    }

    private void OnDestroy()
    {
	    UnRegisterEvent();
    }
    
    #region 连接服务器
    	public override void OnConnected()
    	{
    		Debug.Log("服务器连通成功(未验证)");
    	}
    
    	public override void OnConnectedToMaster()
    	{
    		Debug.Log("服务器连通成功(已验证)");
    		//显示匹配按钮
            matchBtn.gameObject.SetActive(true);
            loadingTips.gameObject.SetActive(false);
        }
        
    	public override void OnDisconnected(DisconnectCause cause)
    	{
    		Debug.Log($"服务器断开连接,{cause}");
    	}
        
    
    	public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    	{
    		Debug.Log($"自定义验证成功");
    	}
    
    	public override void OnCustomAuthenticationFailed(string debugMessage)
    	{
    		Debug.Log($"自定义验证失败");//不知道这个东西是哈
    	}
    	
    	#endregion
    	
    
    	#region 加入房间
        //这个似乎用错函数了
    	// public override void OnFriendListUpdate(List<FriendInfo> friendList)
    	// {
    	// 	Debug.Log("玩家好友列表变化");
     //        if (friendList.Count == GameSettingsConst.MaxPlayer)
     //        {
	    //         readyPanel.gameObject.SetActive(true);
     //        }
     //        //准备面板30秒倒计时,如没有准备
     //        Timer.Register(GameSettingsConst.readyCountDownTime, () =>
     //        {
	    //         readyPanel.gameObject.SetActive(false);
	    //         TipsDialog.ShowDialog("有人取消了本次对局");
	    //         PhotonNetwork.LeaveRoom();
     //        });
    	// }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
	        Debug.Log($"有玩家进入了房间，现在房间的人数有{PhotonNetwork.PlayerList.Length}");
	        base.OnPlayerEnteredRoom(newPlayer);
	        if (PhotonNetwork.IsMasterClient&&PhotonNetwork.PlayerList.Length == GameSettingsConst.MaxPlayer)
	        {
		        PhotonNetwork.RaiseEvent((int)PhotonEvent.ReadyToStartBattle, default, new RaiseEventOptions(){Receivers = ReceiverGroup.All}, default);
	        }
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
    	{
    		Debug.Log($"创建房间失败,code:{returnCode},message:{message}");
            TipsDialog.ShowDialog("匹配失败，请稍后再试");
    	}
    
    	public override void OnJoinedRoom()
    	{
    		Debug.Log("加入房间成功");
    		//设置自定义属性，打开准备面板
            SetInitialCustomProp();
            matchTimePanel.gameObject.SetActive(true);
            matchBtn.gameObject.SetActive(false);
        }
        
    	public override void OnJoinRandomFailed(short returnCode, string message)
    	{
    		Debug.Log($"加入随机房间失败,code:{returnCode},message:{message}");
            if (returnCode == 32760)//没有现存房间
            {
	            RoomOptions roomOptions=new RoomOptions();
	            // roomOptions.PlayerTtl = 600000;
	            // roomOptions.EmptyRoomTtl = 600000;
	            roomOptions.MaxPlayers = GameSettingsConst.MaxPlayer;
	            PhotonNetwork.JoinOrCreateRoom(Time.time.ToString(),roomOptions,default);
            }
            else
            {
	            TipsDialog.ShowDialog($"加入随机房间失败,code:{returnCode},message:{message}");
            }
        }
    
    	public override void OnLeftRoom()
    	{
    		Debug.Log($"离开了房间");
            SetInitialCustomProp();
            Reset();
    	}

        //当别人离开房间时自己也需要离开房间
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
	        base.OnPlayerLeftRoom(otherPlayer);
	        PhotonNetwork.LeaveRoom();
        }

        //完成取消匹配等操作后需要重置
        private void Reset()
        {
	        matchBtn.gameObject.SetActive(true);
	        matchTimePanel.gameObject.SetActive(false);
	        readyPanel.gameObject.SetActive(false);
        }
    	
    
    	#endregion

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
	        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

	        //todo 加入ui即时状态显示
	        if (PhotonNetwork.IsMasterClient && CheckAllPlayerReady())
	        {
		        //接受者默认是others，需要发送到全部，也可以自己手动执行操作，不过ALL比较方便
		        PhotonNetwork.RaiseEvent((int)PhotonEvent.LoadBattleScene, default, new RaiseEventOptions(){Receivers = ReceiverGroup.All}, default);
	        }
	        
        }

        public bool CheckAllPlayerReady()
        {
	        foreach (Player p in PhotonNetwork.PlayerList)
	        {
		        if (p.CustomProperties.TryGetValue(GameManager.PLAYER_READY, out var playerReady))
		        {
			        if (!(bool) playerReady)
			        {
				        return false;
			        }
		        }
		        else
		        {
			        return false;
		        }
	        }

	        return true;
        }

        //设置玩家自定义属性，包括是否准备，战斗场景加载完毕等
        void SetInitialCustomProp()
        {
	        //玩家根據加入順序決定陣營id，比如1v1時，先加入的陣營為0，后加入的為1
	        int campId = 0;
	        campId = PhotonNetwork.PlayerList.Length - 1;//在玩家進入房間后執行此函數，所以要減1
	        Hashtable initialProps = new  Hashtable() {{GameManager.PLAYER_READY, isPlayerReady}, {GameManager.PLAYER_LOADED_LEVEL, false},{GameManager.PLAYER_CAMP_ID,campId}};
	        PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
        }
        
    	public void OnEvent(EventData photonEvent)
    	{
    		//进入战斗场景
    		if (photonEvent.Code == (int) PhotonEvent.LoadBattleScene)
    		{
	            LoadBattleSceneLocally();
    		}
            else if (photonEvent.Code == (int) PhotonEvent.ReadyToStartBattle)
            {
	          OpenReadyPanel();
            }
    	}

        private void OpenReadyPanel()
        {
	        matchTimePanel.gameObject.SetActive(false);
	        readyPanel.gameObject.SetActive(true);
        }
        
        private void LoadBattleSceneLocally()
        {
	        GameManager.Instance.sceneStateController.SetState(new BattleSceneState(GameManager.Instance.sceneStateController), "BattleScene");
        }
}
