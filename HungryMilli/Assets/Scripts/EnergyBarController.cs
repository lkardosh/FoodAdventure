using UnityEngine;

public class EnergyBarController : MonoBehaviour
{
    private EnergyBarStatus energyBarStatus;

    void Start() {
        energyBarStatus = GetComponent<EnergyBarStatus>();
    }

    public void energyFound(float damage, PlayerStatus playerStatus) {
        energyBarStatus.ApplyDamage(damage, playerStatus);
    }
}
