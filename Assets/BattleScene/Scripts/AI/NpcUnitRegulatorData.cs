using UnityEngine;

namespace BattleScene.Scripts.AI
{
    [CreateAssetMenu(fileName = "NewUnitRegulatorData", menuName = "MyRTS/NPC Unit Regulator Data")]
    public class NpcUnitRegulatorData : NpcRegulatorData
    {
        [SerializeField, Tooltip("Instances of this unit amount to available population slots target ratio.")]
        private FloatRange ratioRange = new FloatRange(0.1f, 0.2f);
        public float GetRatio () { return ratioRange.getRandomValue(); }
    }
}