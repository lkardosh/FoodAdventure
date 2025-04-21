using UnityEngine;
using System.Collections;
public class HollyStatus : MonoBehaviour
{
    public float health = 1.0f;
    private bool dead = false;

    public void ApplyDamage(float damage, PlayerStatus playerStatus) {
        health -= damage;
        if (health <= 0 && !dead) {
            dead = true;
            health = 0;
            playerStatus.IncreaseEnergy(100);
            StartCoroutine(DelayedDestroy());
        }
    }
    private IEnumerator DelayedDestroy() {
        yield return new WaitForSeconds(1f); // Wait for 2 seconds
        Destroy(gameObject);
    }
}
