using System.Collections.Generic;

namespace BattleScene.Scripts.AI
{
    public abstract class NpcRegulator<T> where T: BattleUnitBase
    {
        public BattleUnitId battleUnitId;
        
        public int TargetCount { protected set; get; }
        public int MinAmount { protected set; get; }
        public int MaxAmount { protected set; get; }
        public int Count {protected set; get;}
        public int pendingAmount = 0;
        public int MaxPendingAmount { protected set; get; }
        public FactionManager factionManager;
        public FightingManager fightingManager;
        
        protected List<T> instances = new List<T>();

        public NpcRegulator (NpcRegulatorData data, BattleUnitId battleUnitId,FightingManager fightingManager, FactionManager factionManager)
        {
            this.battleUnitId = battleUnitId;
            
            this.factionManager = factionManager;
           
            //pick the rest random settings from the given info.
            MaxAmount = data.GetMaxAmount();
            MinAmount = data.GetMinAmount();
            MaxPendingAmount = data.GetMaxPendingAmount();

            Count = 0;
        }
        
        public virtual bool CanBeRegulated (T factionEntity)
        {
            return (factionEntity.factionId == factionManager.FactionId && factionEntity.configId == battleUnitId);
        }
        
        public virtual void Add(T factionEntity)
        {
            if (!CanBeRegulated(factionEntity)) //only proceed if the faction entity can be regulated by this component
                return;

            //add it to list:
            instances.Add(factionEntity);
            pendingAmount--; //decrease pending Count
        }
        
        /// <summary>
        /// Marks a new pending faction entity that will be regulated by the NPCRegulator instance once the faction entity is initialized.
        /// </summary>
        /// <param name="factionEntity">The pending FactionEntity derived instance to add, setting it to null means that the component will not check whether it can be regulated or not.</param>
        public virtual void AddPending (T factionEntity = null)
        {
            if (factionEntity != null && !CanBeRegulated(factionEntity)) //only proceed if the faction entity can be regulated by this component
                return;

            //increment current count and pending amount:
            Count++;
            pendingAmount++; //decrease pending Count
        }
        
        public virtual void Remove (T factionEntity = null)
        {
            if (factionEntity != null && !CanBeRegulated(factionEntity)) //faction entity can not be regulated by this component
                return;

            Count--; //decrease the amount of current items.
            //remove the item from the current items list:
            if (instances.Remove(factionEntity) == false) //if the item wasn't on the list to begin with
                pendingAmount--; //decrease pending amount

            if(factionEntity)
                OnSuccessfulRemove (factionEntity);
        }
        
        protected abstract void OnSuccessfulRemove(T factionEntity);
        
        /// </summary>
        public void IncMinAmount() { MinAmount++; }
        
        public bool HasReachedMinAmount()
        {
            return Count >= MinAmount;
        }
    }
}