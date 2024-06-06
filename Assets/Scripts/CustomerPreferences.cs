using UnityEngine;

public class CustomerPreferences : MonoBehaviour
{
    public enum CheeseType { Mild, Spicy, Aged }
    public enum Consistency { Soft, Medium, Hard }

    public CheeseType preferredCheeseType;
    public Consistency preferredConsistency;
    public float preferredFatContent;

    public int coinsReward;

    public void GenerateRandomPreferences()
    {
        preferredCheeseType = (CheeseType)Random.Range(0, System.Enum.GetValues(typeof(CheeseType)).Length);
        preferredConsistency = (Consistency)Random.Range(0, System.Enum.GetValues(typeof(Consistency)).Length);
        preferredFatContent = Random.Range(0.1f, 1.0f); // Example fat content range
        coinsReward = Random.Range(10, 100); // Example coin reward range
    }
}
