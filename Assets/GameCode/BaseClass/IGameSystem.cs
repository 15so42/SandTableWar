using UnityEngine;
using System.Collections;

// 遊戲子系統共用界面
public abstract class IGameSystem
{
	protected GameManager gameManager = null;
	public IGameSystem( GameManager gameManager )
	{
		this.gameManager = gameManager;
	}

	public virtual void Initialize(){}
	public virtual void Release(){}
	public virtual void Update(){}

}
