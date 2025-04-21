using UnityEngine;
using System.Collections;
public class FoodStatus : MonoBehaviour
{
    public float health = 1.0f;
    private bool dead = false;

    public void ApplyDamage(float damage, PlayerStatus playerStatus) {
        health -= damage;
        if (health <= 0 && !dead) {
            dead = true;
            health = 0;
            playerStatus.AddFoodCount();
            playerStatus.IncreaseEnergy(5);
            StartCoroutine(DelayedDestroy());
        }
    }
    private IEnumerator DelayedDestroy() {
        yield return new WaitForSeconds(1f); 
        Destroy(gameObject);
    }
}
