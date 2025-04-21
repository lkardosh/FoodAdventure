using UnityEngine;
using System.Collections;
public class EnergyBarStatus : MonoBehaviour
{
    public float health = 1.0f;
    private bool dead = false;

    public void ApplyDamage(float damage, PlayerStatus playerStatus) {
        health -= damage;
        if (health <= 0 && !dead) {
            dead = true;
            health = 0;
            playerStatus.AddLife();
            StartCoroutine(DelayedDestroy());
        }
    }
    private IEnumerator DelayedDestroy() {
        yield return new WaitForSeconds(1f); // Wait for 1
        Destroy(gameObject);
    }
}
