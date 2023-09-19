using System;
using System.Collections.Generic;

namespace Script.Custom.CustomEnum
{
    public static class FlagHelper
    {
        public static void AddFlagRef(this ref int value, int flag) => value |= flag;
        public static void AddFlagRef<T>(this ref T value, T flag) where T : unmanaged, Enum
        {
            var iValue = ToInt(value);
            var iFlag = ToInt(flag);
            value = ToEnum<T>(iValue.AddFlag(iFlag));
        }

        public static void RemoveFlagRef(this ref int value, int flag) => value &= ~flag;

        public static void RemoveFlagRef<T>(this ref T value, T flag) where T : unmanaged, Enum
        {
            var iValue = ToInt(value);
            var iFlag = ToInt(flag);

            value = ToEnum<T>(iValue.RemoveFlag(iFlag));
        }

        public static int AddFlag(this int value, int flag) => value | flag;

        public static T AddFlag<T>(this T value, T flag) where T : unmanaged, Enum
        {
            var iValue = ToInt(value);
            var iFlag = ToInt(flag);
            return ToEnum<T>(iValue.AddFlag(iFlag));
        }

        public static int RemoveFlag(this int value, int flag) => value & ~flag;

        public static T RemoveFlag<T>(this T value, T flag) where T : unmanaged, Enum
        {
            var iValue = ToInt(value);
            var iFlag = ToInt(flag);

            return ToEnum<T>(iValue.RemoveFlag(iFlag));
        }

        public static bool HasFlagFast(this int value, int flag) => (value & flag) == flag;

        public static bool HasFlagFast<T>(this T value, T flag) where T : unmanaged, Enum
        {
            var iValue = ToInt(value);
            var iFlag = ToInt(flag);

            return iValue.HasFlagFast(iFlag);
        }

        public static unsafe int ToInt<T>(this T value) where T : unmanaged, Enum
        {
            var shell = default(Shell<T>);
            shell.EValue = value;

            int* ptr = &shell.IValue;
            ptr += 1;
            return *ptr;
        }

        public static unsafe T ToEnum<T>(this int value) where T : unmanaged, Enum
        {
            var shell = default(Shell<T>);

            int* ptr = &shell.IValue;
            ptr += 1;
            *ptr = value;

            return shell.EValue;
        }

        public static List<T> FindAllFlagOrNull<T>(in T value) where T : unmanaged, Enum => 
            FindAllFlagOrNull<T>(value.ToInt());

        public static List<T> FindAllFlagOrNull<T>(in int value) where T : unmanaged, Enum
        {
            List<T> result = new List<T>();

            int curEnum = 1 << 0;

            byte one = 0x1;

            var _val = value;

            while (_val > 0)
            {
                if (_val.HasFlagFast(one))
                {
                    result.Add(ToEnum<T>(curEnum));
                }

                curEnum <<= 1;
                _val >>= 1;
            }

            return result;
        }

        private struct Shell<T> where T : unmanaged, Enum
        {
            public int IValue;
            public T EValue;
        }
    }
}