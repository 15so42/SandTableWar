using DefaultNamespace;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Timer = UnityTimer.Timer;

// 戰鬥狀態
public class BattleSceneState : SceneState,IOnEventCallback,IInRoomCallbacks
{
	public FightingManager fightingManager;
	public BattleSceneState(SceneStateController controller):base(controller)
	{
		this.StateName = "BattleState";
	}

	// 開始
	public override void StateBegin()
	{
		PhotonNetwork.AddCallbackTarget(this);
		fightingManager=new FightingManager();
		fightingManager.Init();
		
		Hashtable prop=new Hashtable()
		{
			{GameManager.PLAYER_LOADED_LEVEL,true}
		};
		PhotonNetwork.LocalPlayer.SetCustomProperties(prop);
		
	}

	/// <summary>
	/// 双方均准备完成
	/// </summary>
	private void OnBattleStart()
	{
		MainBattleDialog.ShowDialog();
	}

	// 結束
	public override void StateEnd()
	{
		PhotonNetwork.RemoveCallbackTarget(this);
	}
			
	// 更新
	public override void StateUpdate()
	{	
		// 遊戲邏輯,例如fightingManager
		fightingManager.Update();
	}

	public void OnEvent(EventData photonEvent)
	{
		//throw new System.NotImplementedException();
		if (photonEvent.Code == (int) PhotonEvent.StartBattle)
		{
			//生成基地
			OnBattleStart();
			fightingManager.SpawnBase();
		}
	}

	public void OnPlayerEnteredRoom(Player newPlayer)
	{
		//throw new System.NotImplementedException();
	}

	public void OnPlayerLeftRoom(Player otherPlayer)
	{
		//throw new System.NotImplementedException();
	}

	public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
		//throw new System.NotImplementedException();
	}

	public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
		//throw new System.NotImplementedException();
		if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length==GameSettingsConst.MaxPlayer && CheckAllPlayerLoaded())
		{
			//对局正式开始
			PhotonNetwork.RaiseEvent((int)PhotonEvent.StartBattle, default,
				new RaiseEventOptions() {Receivers = ReceiverGroup.All}, default);
		}
		
	}

	private bool CheckAllPlayerLoaded()
	{
		foreach (Player p in PhotonNetwork.PlayerList)
		{
			if (p.CustomProperties.TryGetValue(GameManager.PLAYER_LOADED_LEVEL, out var loaded))
			{
				if (!(bool) loaded)
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

	public void OnMasterClientSwitched(Player newMasterClient)
	{
		//throw new System.NotImplementedException();
	}
}
