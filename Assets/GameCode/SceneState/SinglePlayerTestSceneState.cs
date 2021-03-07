using DefaultNamespace;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Timer = UnityTimer.Timer;

// 戰鬥狀態
public class SinglePlayerTestSceneState : SceneState
{
	public FightingManager fightingManager;
	public SinglePlayerTestSceneState(SceneStateController controller):base(controller)
	{
		this.StateName = "SinglePlayerTestSceneState";
	}

	// 開始
	public override void StateBegin()
	{
		fightingManager=new FightingManager();
		fightingManager.Init();
		
		fightingManager.SpawnBase();
		OnBattleStart();
		
	}

	/// <summary>
	/// 双方均准备完成
	/// </summary>
	private void OnBattleStart()
	{
		MainBattleDialog.ShowDialog();
		SpawnBuildingDialog.ShowDialog();
	}

	// 結束
	public override void StateEnd()
	{
	}
			
	// 更新
	public override void StateUpdate()
	{	
		// 遊戲邏輯,例如fightingManager
		fightingManager.Update();
	}

	
	
}
