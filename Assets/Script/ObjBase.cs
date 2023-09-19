using System;
using Script.Manager;
using UnityEngine;

namespace Script
{
    public class ObjBase : MonoBehaviour
    {
        public bool Placed { get; private set; }
        public Vector3Int Size { get; private set; }

        private Vector3[] m_Vertices;

        public virtual void Place()
        {
            var _drag = GetComponent<ObjDrag>();
            Destroy(_drag);

            Placed = true;
        }
        
        private void Start()
        {
            Get();
            Calc();
        }

        private void Get()
        {
            var _b = GetComponent<BoxCollider>();
            m_Vertices = new Vector3[4];

            var _center = _b.center;
            var _size = _b.size;
            var _x = _size.x;
            var _y = _size.y;
            var _z = _size.z;

            m_Vertices[0] = _center + new Vector3(-_x, -_y, -_z) * 0.5f;
            m_Vertices[1] = _center + new Vector3(_x, -_y, -_z) * 0.5f;
            m_Vertices[2] = _center + new Vector3(_x, -_y, _z) * 0.5f;
            m_Vertices[3] = _center + new Vector3(-_x, -_y, _z) * 0.5f;
        }

        private void Calc()
        {
            var _len = m_Vertices.Length;
            var _vertices = new Vector3Int[_len];
            
            for (var i = 0; i < _len; i++)
            {
                var _wPos = transform.TransformPoint(m_Vertices[i]);
                _vertices[i] = GameManager.Instance.m_Grid.WorldToCell(_wPos);
            }

            Size = new Vector3Int(Mathf.Abs((_vertices[0] - _vertices[1]).x),
                Mathf.Abs((_vertices[0] - _vertices[3]).y), 1);
        }

        private Vector3 GetStart() => transform.TransformPoint(m_Vertices[0]);
    }
}