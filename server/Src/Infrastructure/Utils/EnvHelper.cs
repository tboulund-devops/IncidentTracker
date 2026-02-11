using Domain.Exceptions;
using Domain.Interfaces.Utility;

namespace Infrastructure.Utils;

public class EnvHelper : IEnvHelper
{
    private static T? ParseOrThrow<T>(string value)
    {
        try
        {
            if (typeof(T) == typeof(bool))
            {
                var boolValue = bool.Parse(value);
                return (T)(object) boolValue;
            }
            if (typeof(T) == typeof(int))
            {
                var intValue = int.Parse(value);
                return (T)(object) intValue;
            }
            if (typeof(T) == typeof(char))
            {
                var charValue = char.Parse(value);
                return (T)(object) charValue;
            }
            if (typeof(T) == typeof(string))
            {
                return (T)(object) value;
            }
        }
        catch (FormatException formatException)
        {
            throw new FormatException($"The value '{value}' is not in valid format for '{typeof(T).Name}' type", formatException);
        }

        throw new WrongTypeEnvironmentVariableException($"Cannot convert type: {typeof(T).FullName} with value: {value}");
    }
    public T? Get<T>(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);

        return string.IsNullOrEmpty(value) ? throw new  EnvironmentVariableNotFoundException(key) : ParseOrThrow<T>(value);
    }

    public T GetOrDefault<T>(string key, T defaultValue)
    {
        var value = Get<T>(key);
        return value ?? defaultValue;
    }

    public T GetRequired<T>(string key)
    {
        var value = Get<T>(key);
        return value ?? throw new RequireEnvironmentVariableException($"Variable {key} is required!");
    }
    
}