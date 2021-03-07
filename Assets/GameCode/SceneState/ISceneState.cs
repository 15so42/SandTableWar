using UnityEngine;
using System.Collections;

// 場景狀態
public class SceneState
{
	// 狀態名稱

	public string StateName { get; set; } = "ISceneState";

	// 控制者
	protected readonly SceneStateController sceneController = null;
		
	// 建構者
	protected SceneState(SceneStateController controller)
	{ 
		sceneController = controller; 
	}

	// 開始
	public virtual void StateBegin()
	{}

	// 結束
	public virtual void StateEnd()
	{}

	// 更新
	public virtual void StateUpdate()
	{}

	public override string ToString ()
	{
		return $"[I_SceneState: StateName={StateName}]";
	}


}
