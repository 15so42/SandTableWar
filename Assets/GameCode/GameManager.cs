using UnityEngine;
using System.Collections;

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

	// 場景狀態控制
	private bool isGameOver = false;

	private GameManager()
	{}
	
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

}
