//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class FinishLine : MonoBehaviour
//{
//    // Start is called before the first frame update
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    //public static bool isFinished = false;  // Track if the finish line has been crossed
    // This function is called when another collider enters the trigger collider attached to this GameObject
    private void OnTriggerEnter(Collider other)
    {

        // Check if the collider belongs to a CheeseWheelMovement
        CheeseWheelMovement wheel;
        if (other.TryGetComponent<CheeseWheelMovement>(out wheel))
        {
            /*
            // Log a message to the Console to indicate which player has crossed the finish line
            Debug.Log(wheel.name + " wins!");
            isFinished = true;  // Set the flag when the finish line is crossed
            // Optional: Add additional logic here to handle the end of the game, such as:
            // - Displaying a win message
            // Stop the movement of the player who crosses the finish line
            Rigidbody playerRigidbody = wheel.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                playerRigidbody.velocity = Vector3.zero; // Stop any current movement
                playerRigidbody.isKinematic = true; // Disables physics interactions
            }
            // - Triggering a victory animation
            // - Loading a new scene or restarting the game
            */

            MassController mass = wheel.GetComponent<MassController>();
            Player player = GameManager.Instance.GetPlayer(wheel);
            if (player != null && mass != null)
            {
                GameManager.Instance.DeliveredCheese(player, mass);
                if (GameManager.Instance.State == EGameManagerState.Racing)
                    player.ResetCheeseWheelAndChooseNext();
            }
        }
    }
}


