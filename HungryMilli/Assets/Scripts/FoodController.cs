using UnityEngine;

public class FoodController : MonoBehaviour
{
    private FoodStatus foodStatus;

    void Start() {
        foodStatus = GetComponent<FoodStatus>();
    }

    public void EatFood(float damage, PlayerStatus playerStatus) {
        foodStatus.ApplyDamage(damage, playerStatus);
    }
}
