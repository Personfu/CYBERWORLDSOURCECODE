using UnityEngine;

public class DiscordRoomTrigger : MonoBehaviour
{
    public string areaNameOnEnter = "";

    private int insideCount = 0;

    private void ApplyPresence()
    {
        if (!string.IsNullOrEmpty(areaNameOnEnter))
            DiscordController.SetAreaNameOverrideGlobal(areaNameOnEnter);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        insideCount++;

        if (insideCount == 1)
            ApplyPresence();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (insideCount <= 0)
            insideCount = 1;

        ApplyPresence();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        insideCount--;
        if (insideCount < 0) insideCount = 0;

        if (insideCount == 0)
            DiscordController.ClearAreaNameOverrideGlobal();
    }
}
