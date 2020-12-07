using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

// 主選單狀態
public class MainMenuSceneState : SceneState
{
	public MainMenuManager mainMenuManager;
	
	public MainMenuSceneState(SceneStateController controller):base(controller)
	{
		this.StateName = "MainMenuState";
	}

	public override void StateBegin()
	{
		base.StateBegin();
		mainMenuManager = Object.FindObjectOfType<MainMenuManager>();
		mainMenuManager.Init();
	}

	public override void StateEnd()
	{
	}
	
}
