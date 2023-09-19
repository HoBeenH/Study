using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Script.Custom.CustomEnum
{
    public class FlagField<T> where T : unmanaged, Enum
    {
        public int HasEnumCnt { get; private set; } = 0;
        public Type EnumType { get; private set; } = default;

        private T m_EnumValue = default;
        private readonly HashSet<T> m_EnumSet = null;
        private bool m_Init = false;

        public FlagField()
        {
            EnumType = typeof(T);
            m_EnumSet = new HashSet<T>();
            try
            {
                if (Enum.GetValues(EnumType) is T[] allEnums)
                {
                    foreach (T e in allEnums)
                    {
                        m_EnumSet.Add(e);
                    }
                }

                m_Init = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Can't Not GetValue {EnumType}\n{e}");
                m_Init = false;
                return;
            }
        }

        private bool InitCheck()
        {
            if (m_Init == false)
                return false;

            if (EnumType == null)
                return false;

            if (m_EnumSet == null)
                return false;

            return true;
        }

        private bool EnumConditionCheck(T value)
        {
            return InitCheck() && !m_EnumSet.Contains(value);
        }

        public void AddFlag(T value)
        {
            if (EnumConditionCheck(value) == false)
                return;

            if (m_EnumValue.HasFlagFast(value) == true)
                return;

            HasEnumCnt += 1;
            m_EnumValue.AddFlagRef(value);
        }

        public void RemoveFlag(T value)
        {
            if (EnumConditionCheck(value) == false)
                return;

            if (m_EnumValue.HasFlagFast(value) == false)
                return;

            HasEnumCnt -= 1;
            m_EnumValue.RemoveFlagRef(value);
        }

        public void Clear(int defaultValue = 0)
        {
            m_EnumValue = defaultValue.ToEnum<T>();
            HasEnumCnt = 0;
        }

        public void Clear(T defaultValue)
        {
            m_EnumValue = defaultValue;
            HasEnumCnt = 0;
        }

        public bool HasFlag(T flag)
        {
            if (EnumConditionCheck(flag) == false)
                return false;

            return FlagHelper.HasFlagFast(m_EnumValue, flag);
        }
        
        public List<T> GetHasFlagListOrNull()
        {
            if (InitCheck() == false)
                return null;

            if (HasEnumCnt <= 0)
                return null;

            List<T> result = new List<T>(HasEnumCnt);

            int val = FlagHelper.ToInt(m_EnumValue);
            int curEnum = 1 << 0;

            byte one = 0x1;

            while (val > 0)
            {
                if (val.HasFlagFast(one))
                {
                    result.Add(FlagHelper.ToEnum<T>(curEnum));
                }

                curEnum <<= 1;
                val >>= 1;
            }

            return result;
        }

        public T GetFlag() => m_EnumValue;

        public bool Equals<K>([CanBeNull] FlagField<K> other) where K : unmanaged, Enum
        {
            if (other == null)
                return false;

            if (EnumType != other.EnumType)
                return false;

            return FlagHelper.ToInt(m_EnumValue) == FlagHelper.ToInt(other.m_EnumValue);
        }

        public override string ToString()
        {
            return m_EnumValue.ToString();
        }
    }
}