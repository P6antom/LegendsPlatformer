using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCoins : MonoBehaviour
{
    [Tooltip("The particles that appear after the player collects a coin.")]
    public GameObject coinParticles;

    [Tooltip("Amount of health to add to the player when collected.")]
    public float healthToAdd = 5f;

    PlayerMovement playerMovementScript;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerMovementScript = other.GetComponent<PlayerMovement>();
            //playerMovementScript.ModifyHealth(healthToAdd);
            playerMovementScript.soundManager.PlayCoinSound();
            playerMovementScript.ModifyHealth(5, "coin collected");
            ScoreManager.score += 10;
            GameObject particles = Instantiate(coinParticles, transform.position, new Quaternion());
            Destroy(gameObject);
        }
    }
}