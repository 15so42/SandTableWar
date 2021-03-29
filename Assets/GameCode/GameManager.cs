using UnityEngine;
using System.Collections;
using Photon.Pun;

public enum DamageType{
	Physical,//物理伤害
	Real//魔法伤害
}

public enum GameMode
{
	Campaign,//单机
	PVP
}
public class GameManager
{
	//------------------------------------------------------------------------
	// Singleton模版
	private static GameManager _instance;
	public static GameManager Instance
	{
		get
		{
			if (_instance == null)			
				_instance = new GameManager();
			return _instance;
		}
	}
	

	public SceneStateController sceneStateController;
	public GameMode gameMode;
	
	public const string PLAYER_READY = "IsPlayerReady";
	public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";
	public const string PLAYER_CAMP_ID = "PlayerCampId";
	
	private GameManager()
	{}

	public int GetSelfId()
	{
		return GetFightingManager().campId;
	}
	public void Init()
	{
		// 註冊遊戲事件系統
		RegisterGameEvent();
	}

	// 註冊遊戲事件系統
	private void RegisterGameEvent()
	{
		// 事件註冊
	}

	// 釋放遊戲系統
	public void Release()
	{
		UITool.ReleaseCanvas();
	}

	// 更新
	public void Update()
	{
		// 玩家輸入
		InputProcess();
	}

	// 玩家輸入
	private void InputProcess()
	{
		//这里用来控制回车，菜单栏之类的控制
	}

	public void SetSceneController(SceneStateController sceneStateController)
	{
		this.sceneStateController = sceneStateController;
	}

	public SceneStateController GetSceneController()
	{
		return sceneStateController;
	}

	public FightingManager GetFightingManager()
	{
		//战斗场景中
		if (GetSceneController().state.GetType() == typeof(BattleSceneState))
		{
			return (sceneStateController.state as BattleSceneState)?.fightingManager;
		}

		if (GetSceneController().state.GetType() == typeof(SinglePlayerTestSceneState))
		{
			return (sceneStateController.state as SinglePlayerTestSceneState)?.fightingManager;
		}

		return null;
	}
	
	public void InstantiateByResources(string path,Vector3 pos,Quaternion quaternion)
	{
		if (PhotonNetwork.IsConnected)
		{
			PhotonNetwork.Instantiate(path,pos,quaternion);
		}
		else
		{
			Object.Instantiate(Resources.Load<GameObject>(path), pos, quaternion);
		}
	}

}
