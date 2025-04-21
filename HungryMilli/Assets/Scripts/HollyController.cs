using UnityEngine;

public class HollyController : MonoBehaviour
{
    private HollyStatus hollyStatus;

    void Start() {
        hollyStatus = GetComponent<HollyStatus>();
    }

    public void HollyFound(float damage, PlayerStatus playerStatus) {
        hollyStatus.ApplyDamage(damage, playerStatus);
    }
}
