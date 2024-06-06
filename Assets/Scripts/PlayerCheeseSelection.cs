using UnityEngine;

public class PlayerCheeseSelection : MonoBehaviour
{
    public CustomerPreferences.CheeseType selectedCheeseType;
    public CustomerPreferences.Consistency selectedConsistency;
    public float selectedFatContent;

    public void SelectCheese(CustomerPreferences.CheeseType type, CustomerPreferences.Consistency consistency, float fatContent)
    {
        selectedCheeseType = type;
        selectedConsistency = consistency;
        selectedFatContent = fatContent;
    }
}
