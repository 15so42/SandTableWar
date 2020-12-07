using UnityEngine;
using System;
using System.Collections;
using DefaultNamespace;
using Photon.Pun;
using Random = UnityEngine.Random;

// 遊戲主迴圈
public class GameLoop : MonoBehaviour 
{
	// 場景狀態
	private readonly SceneStateController sceneStateController = new SceneStateController();

	// 
	private void Awake()
	{
		// 切換場景不會被刪除
		DontDestroyOnLoad(gameObject);
	}

	// Use this for initialization
	private void Start () 
	{
		// 設定起始的場景,空场景名表示不跳转场景
		GameManager.Instance.SetSceneController(sceneStateController);
		sceneStateController.SetState(new StartSceneState(sceneStateController), "");
	}

	// Update is called once per frame
	private void Update () 
	{
		sceneStateController.StateUpdate();	
	}
}
