using UnityEngine;

public class GarbageController : MonoBehaviour
{
    private GarbageStatus garbageStatus;

    void Start() {
        garbageStatus = GetComponent<GarbageStatus>();
    }

    public void garbageFound(float damage, PlayerStatus playerStatus) {
        garbageStatus.ApplyDamage(damage, playerStatus);
    }
}
