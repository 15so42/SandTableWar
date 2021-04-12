using System;
using System.Collections.Generic;
using UnityEngine;


    public class IsInBuildingArea : MonoBehaviour
    {
        List<BattleUnitBase> bases=new List<BattleUnitBase>();
        public void Start()
        {
            bases = GlobalItemRangeDisplayer.Instance.GetBuildRange();
        }

        public bool isInBuildingArea=false;
        public bool CanPlace()
        {
            for (int i = 0; i < bases.Count; i++)
            {
                if (Vector3.Distance(transform.position, bases[i].transform.position) < bases[i].prop.viewDistance)
                {
                    return true;
                }
            }

            return false;
        }
        
    }
