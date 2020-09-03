
using System;

namespace Nullspace
{
    public class EnumUtils
    {
        public static string EnumToString<T>(T value)
        {
            return Enum.GetName(typeof(T), value);
        }
        public static T StringToEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        public static object StringToEnum(string value, Type type)
        {
            return Enum.Parse(type, value);
        }

        public static U EnumToBaseType<T, U>(T value)
        {
            return (U)(object)value;
        }

        public static int EnumToInt<T>(T value)
        {
            return EnumToBaseType<T, int>(value);
        }

        public static T IntToEnum<T>(int value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        public static string[] GetNames(Type enumType)
        {
            return Enum.GetNames(enumType);
        }

        public static bool IsDefined(Type enumType, object value)
        {
            return Enum.IsDefined(enumType, value);
        }
    }
}

