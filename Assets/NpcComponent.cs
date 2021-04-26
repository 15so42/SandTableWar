using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
    /// Parent class of all NPC components except the NPCManager.
    /// </summary>
    public abstract class NpcComponent : MonoBehaviour {

        #region Class Properties
        //other components that the NPCComponent might need access to
        protected NpcCommander npcCommander;
        protected FactionManager factionMgr;
        protected FightingManager fightingManager;

        //Is the current component in active state or not?
        public bool IsActive { private set; get; }
        /// <summary>
        /// Activates the NPCComponent instance.
        /// </summary>
        public virtual void Activate () { IsActive = true; }
        /// <summary>
        /// Deactivates the NPCComponent instance.
        /// </summary>
        public virtual void Deactivate() { IsActive = false; }
        #endregion

        #region Initializing/Terminating
        /// <summary>
        /// Initializes the NPCComponent instance.
        /// </summary>
        /// <param name="fightingManager">GameManager instance of the current game.</param>
        /// <param name="npcMgr">NPCManager instance that manages this NPCComponent instance.</param>
        /// <param name="factionMgr">FactionManager instance of the faction that this component manages.</param>
        public virtual void Init(FightingManager fightingManager, NpcCommander npcCommander, FactionManager factionMgr)
        {
            //assign components
            this.fightingManager = fightingManager;
            Assert.IsNotNull(this.fightingManager, "[NPCComponent] Initializing without a reference to the GameManager instance is not allowed!");

            this.npcCommander = npcCommander;
            Assert.IsNotNull(this.npcCommander, "[NPCComponent] Initializing without a reference to the faction's NPCManager instance is not allowed!");

            this.factionMgr = factionMgr;
            Assert.IsNotNull(this.factionMgr, "[NPCComponent] Initializing without a reference to the faction's FactionManager instance is not allowed!");
        }
        #endregion

        #region Updating Component
        /// <summary>
        /// Unity's MonoBehavior Update method
        /// </summary>
        private void Update()
        {
            if (!IsActive)
                return;

            OnActiveUpdate();
        }

        /// <summary>
        /// Called every frame as long as the NPC Component is active.
        /// </summary>
        protected virtual void OnActiveUpdate ()
        {

        }
        #endregion
    }