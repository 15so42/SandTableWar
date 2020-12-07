namespace DefaultNamespace
{
    //事件列表，其中的数字表示id，服务器通过事件id来分发事件
    public enum PhotonEvent
    {
        LoadBattleScene=0,//加载战斗场景，使用事件而不是自动同步场景是因为本项目使用基于状态模式的场景切换，需要在调用特殊的函数切换场景
        ReadyToStartBattle=1,//玩家数量足够，要求玩家准备开始战斗，参考英雄联盟的匹配成功要求准备倒计时页面
        StartBattle=2,//所有玩家加载玩战斗场景
    }
    public class PhotonEventConst
    {
       
    }
}