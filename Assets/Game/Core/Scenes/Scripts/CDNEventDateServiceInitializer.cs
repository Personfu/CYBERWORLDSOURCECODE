using UnityEngine;

public class CDNEventDateServiceInitializer : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        // Check if service already exists
        if (CDNEventDateService.Instance != null)
        {
            return;
        }

        // Create a new GameObject with the service
        GameObject serviceObject = new GameObject("CDNEventDateService");
        serviceObject.AddComponent<CDNEventDateService>();
        
        // Make it persist across scenes
        DontDestroyOnLoad(serviceObject);
        
        Debug.Log("[CDNEventDateServiceInitializer] Created CDNEventDateService instance");
    }
}
