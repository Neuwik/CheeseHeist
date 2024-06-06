using UnityEngine;

[System.Serializable]
public class CustomerRequest
{
    public CustomerPreferences.CheeseType cheeseType;
    public CustomerPreferences.Consistency consistency;
    public float fatContent;
    public int coinsReward;

    public CustomerRequest()
    {
        GenerateRandomRequest();
    }

    public void GenerateRandomRequest()
    {
        cheeseType = (CustomerPreferences.CheeseType)Random.Range(0, System.Enum.GetValues(typeof(CustomerPreferences.CheeseType)).Length);
        consistency = (CustomerPreferences.Consistency)Random.Range(0, System.Enum.GetValues(typeof(CustomerPreferences.Consistency)).Length);
        fatContent = Random.Range(0.1f, 1.0f);
        coinsReward = Random.Range(10, 100);
    }

    public override string ToString()
    {
        return $"{cheeseType}, {consistency}, Fat: {fatContent:F2}, Reward: {coinsReward} coins";
    }
}
