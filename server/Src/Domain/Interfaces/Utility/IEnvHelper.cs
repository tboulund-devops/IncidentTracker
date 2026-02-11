namespace Domain.Interfaces.Utility;

public interface IEnvHelper
{
    T? Get<T>(string key);
    T GetOrDefault<T>(string key, T defaultValue);
    T GetRequired<T>(string key);
}