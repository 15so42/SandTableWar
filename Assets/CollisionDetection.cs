using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
   [Header("触碰到的碰撞体")]
   public List<Collider> colliders=new List<Collider>();

   
   private bool closedColliders;
   private void OnDisable()
   {
      colliders.Clear();
   }

   private void Start()
   {
      
   }

   public void ToggleCollider(bool status)
   {
      GetComponent<Collider>().enabled = status;
      closedColliders = !status;
   }

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
      if (closedColliders)
         return false;
      
      int count = 0;
      for (int i = 0; i < colliders.Count; i++)
      {
         if(colliders[i]==null)
            continue;
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
