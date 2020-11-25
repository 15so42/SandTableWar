using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 戰鬥狀態
public class BattleState : ISceneState
{
	public BattleState(SceneStateController Controller):base(Controller)
	{
		this.StateName = "BattleState";
	}

	// 開始
	public override void StateBegin()
	{
		GameManager.Instance.Init();
	}

	// 結束
	public override void StateEnd()
	{
		GameManager.Instance.Release();
	}
			
	// 更新
	public override void StateUpdate()
	{	
		// 遊戲邏輯,例如fightingManager
	}
}
