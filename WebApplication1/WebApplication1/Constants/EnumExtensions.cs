using System.Reflection;

namespace WebApplication1.Constants;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        return enumValue.GetType()
                        .GetMember(enumValue.ToString())[0]
                        .GetCustomAttribute<DisplayAttribute>()?
                        .Name ?? enumValue.ToString();
    }
}