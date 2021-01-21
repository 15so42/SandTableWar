using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
   public List<Collider> colliders=new List<Collider>();
   private void OnTriggerEnter(Collider other)
   {
      colliders.Add(other);
   }

   private void OnTriggerExit(Collider other)
   {
      colliders.Remove(other);
   }

   public bool CanPlace()
   {
      int count = 0;
      for (int i = 0; i < colliders.Count; i++)
      {
         if (colliders[i].gameObject.layer == LayerMask.NameToLayer("Ground"))
         {
            continue;
         }

         count++;
      }

      if (count > 0)
         return false;
      return true;
   }
}
