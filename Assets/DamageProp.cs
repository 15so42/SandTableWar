namespace DefaultNamespace
{
    [System.Serializable]
    public class DamageProp
    {
        public int damageValue;
        //穿透伤害补正和爆破伤害补正，两者加起来为1
        public float penetrateRate = 1;//穿透
        public float explosionRate = 0;//爆破
        public int soul;//灵魂
        public int electromagnetism;
    }
}