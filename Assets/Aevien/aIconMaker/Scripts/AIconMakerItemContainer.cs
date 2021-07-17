using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aevien.IconMaker
{
    public class AIconMakerItemContainer : MonoBehaviour
    {
        private Vector3 _offsetPosition;
        private string _name;

        private void Start()
        {
            _offsetPosition = Vector3.zero;
        }

        /// <summary>
        /// Set a name of the item
        /// </summary>
        /// <param name="p_name"></param>
        public void SetName(string p_name)
        {
            _name = p_name;
        }

        /// <summary>
        /// Get the name of the item
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return string.IsNullOrEmpty(_name) ? name : _name;
        }

        /// <summary>
        /// offset item to X values
        /// </summary>
        /// <param name="p_x"></param>
        public void OffsetItemByX(float p_x)
        {
            _offsetPosition.x = p_x;
            UpdateOffset();
        }

        /// <summary>
        /// offset item to Y values
        /// </summary>
        /// <param name="p_y"></param>
        public void OffsetItemByY(float p_y)
        {
            _offsetPosition.y = p_y;
            UpdateOffset();
        }

        /// <summary>
        /// offset item to Z values
        /// </summary>
        /// <param name="p_z"></param>
        public void OffsetItemByZ(float p_z)
        {
            _offsetPosition.z = p_z;
            UpdateOffset();
        }

        /// <summary>
        /// Update item offset position
        /// </summary>
        private void UpdateOffset()
        {
            transform.localPosition = _offsetPosition;
        }

        /// <summary>
        /// Get item offset
        /// </summary>
        /// <returns></returns>
        public Vector3 GetOffset()
        {
            if(_offsetPosition != null)
            {
                return _offsetPosition;
            }
            else
            {
                return Vector3.zero;
            }
        }
    }
}
