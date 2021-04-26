using BehaviorDesigner.Runtime.Tasks;

public class CommanderInit : Task
{
    public SharedFactionManager sharedFactionManager;
    public override void OnAwake()
    {
        base.OnAwake();
        sharedFactionManager.Value = transform.root.GetComponent<NpcCommander>().factionManager;
    }
}