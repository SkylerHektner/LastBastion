using UnityEngine;

public class SteamManager
{
    public uint appId = 3876840;

    // TODO Assign once we have a DLC setup
    public uint cosmeticsDLCAppId = 0;

    public SteamManager()
    {
        try
        {
            Steamworks.SteamClient.Init(appId, false);
            Debug.Log("Steamworks initialized succesfully");
        }
        catch (System.Exception e)
        {
            Debug.Log($"Failed to initialize steamworks {e}");
        }
    }

    public void Update()
    {
        Steamworks.SteamClient.RunCallbacks();
    }

    public bool HasCosmeticsDLC()
    {
        return Steamworks.SteamApps.IsDlcInstalled(cosmeticsDLCAppId);
    }
}
