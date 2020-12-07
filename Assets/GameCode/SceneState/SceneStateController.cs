using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.SceneManagement;

// 場景狀態控制者
public class SceneStateController
{
	public SceneState state;	
	private bool 	isRunBegin;
	
	// 設定狀態
	public void SetState(SceneState nextState, string loadSceneName)
	{
		//Debug.Log ("SetState:"+State.ToString());
		isRunBegin = false;

		// 載入場景
		LoadScene( loadSceneName );

		// 通知前一個State結束
		this.state?.StateEnd();

		// 設定
		this.state=nextState;	
	}

	// 載入場景
	private void LoadScene(string loadSceneName)
	{
		if( string.IsNullOrEmpty(loadSceneName) )
			return ;
		
		SceneManager.LoadScene( loadSceneName );
	}

	// 更新
	public void StateUpdate()
	{
		// 是否還在載入
		if( Application.isLoadingLevel)
			return ;

		// 通知新的State開始
		if( state != null && isRunBegin==false)
		{
			state.StateBegin();
			isRunBegin = true;
		}

		state?.StateUpdate();
	}
}
