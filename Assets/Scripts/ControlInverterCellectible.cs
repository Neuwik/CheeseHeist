using UnityEngine;

public class ControlInverterCollectible : MonoBehaviour
{
    public float effectDuration = 5f; // Duration for which the controls are inverted

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Player2"))
        {
            // Assuming you have a way to identify the opponent
            CheeseWheelMovement opponentMovement = FindOpponentMovementComponent(other.gameObject);
            if (opponentMovement != null)
            {
                opponentMovement.InvertControls(effectDuration);
            }
            Destroy(gameObject); // Destroy the collectible after it's collected
        }
    }

    // Implement this method based on your game design to find the correct opponent
    CheeseWheelMovement FindOpponentMovementComponent(GameObject collector)
    {
        // Assuming there are only two players tagged as "Player1" and "Player2"
        string opponentTag = collector.tag == "Player" ? "Player2" : "Player";
        GameObject opponent = GameObject.FindGameObjectWithTag(opponentTag);
        if (opponent != null)
        {
            return opponent.GetComponent<CheeseWheelMovement>();
        }
        return null; // Return null if no opponent is found
    }
}
