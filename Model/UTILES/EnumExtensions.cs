using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        Type enumType = enumValue.GetType();
        string memberName = enumValue.ToString();
        FieldInfo fieldInfo = enumType.GetField(memberName);

        if (fieldInfo == null)
        {
            return memberName;
        }

        DisplayAttribute attribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();

        return attribute?.Name ?? memberName;
    }
}