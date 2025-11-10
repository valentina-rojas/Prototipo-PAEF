using UnityEngine;

public static class AssetLoader
{
    public static T Load<T>(string name) where T : Object
    {
        var asset = Resources.Load<T>(name);
        if (asset == null)
            Debug.LogError($"No se encontró el asset '{name}' en Resources.");
        else
            Debug.Log($"Asset '{name}' cargado correctamente.");
        return asset;
    }
}
