namespace BattleScene.Scripts.AI
{
    public enum CommanderTaskType
    {
        CreatUnit,
        PlaceBuilding,
    }
    [System.Serializable]
    public class CommanderTask
    {
        public CommanderTask taskType;
        public BattleUnitId battleUnitId;
    }
}