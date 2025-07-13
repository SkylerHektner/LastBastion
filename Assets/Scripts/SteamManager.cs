using UnityEngine;

public class SteamManager
{
    public uint appId = 3876840;

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
}
