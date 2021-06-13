using System.Collections;
using System.Collections.Generic;
using BattleScene.Scripts.AI;
using UnityEngine;

namespace RTSEngine
{
    //settings for how the building should be placed:
    public enum NpcPlacementOption { aroundCenter, aroundBuilding, aroundResource};

    /// <summary>
    /// Includes data that will be used to regulate the creation of the assigned building by an NPC faction.
    /// </summary>
    [CreateAssetMenu(fileName = "NewBuildingRegulatorData", menuName = "MyRTS/NPC Building Regulator Data", order = 53)]
    public class NpcBuildingRegulatorData : NpcRegulatorData
    {
        [SerializeField, Tooltip("Should the building type be regulated per building center or for the whole faction overall?")]
        private bool regulatePerBuildingCenter = true;
        public bool RegulatePerBuildingCenter => regulatePerBuildingCenter;

        //either randomly, around a specific building or a around a specific resource.
        [SerializeField, Tooltip("How should the NPC faction place this building?")]
        private NpcPlacementOption placementOption = NpcPlacementOption.aroundCenter;
        public NpcPlacementOption GetPlacementOption () { return placementOption; }

        [SerializeField, Tooltip("If the building is to be placed around a building/resource, this is that building/resource code.")]
        private BattleUnitId placementOptionInfo ; //only valid when the placement option is set to aroundBuilding (provide building code) or aroundResource (provide resource name).
        public BattleUnitId GetPlacementOptionInfo () { return placementOptionInfo; }

        //distance of the building from its closest building center:
        [SerializeField, Tooltip("How far should the building be placed of its center?")]
        private FloatRange buildAroundDistance = new FloatRange(1.0f, 2.0f);
        public float GetBuildAroundDistance () { return buildAroundDistance.getRandomValue(); }
        [SerializeField, Tooltip("If enabled, the building will be placed and rotated towards its center.")]
        private bool rotate = false; //when true, the building will rotate to look at its placement around object
        public bool CanRotate () { return rotate; }
    }
}
