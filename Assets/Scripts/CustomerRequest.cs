using UnityEngine;

[System.Serializable]
public class CustomerRequest
{
    //public CustomerPreferences.CheeseType cheeseType;
    //public CustomerPreferences.Consistency consistency;
    //public float fatContent;
    public int coinsReward;

    public CheeseMass Cheese;

    public CustomerRequest()
    {
        //GenerateRandomRequest();
    }

    public void GenerateRandomRequest()
    {
        //cheeseType = (CustomerPreferences.CheeseType)Random.Range(0, System.Enum.GetValues(typeof(CustomerPreferences.CheeseType)).Length);
        //consistency = (CustomerPreferences.Consistency)Random.Range(0, System.Enum.GetValues(typeof(CustomerPreferences.Consistency)).Length);
        float fatContent = Random.Range(0.1f, 1.0f);
        float cheesetype = Random.Range(0.1f, 1.0f);
        float cheeseconsistency = Random.Range(0.1f, 1.0f);
        float amount = Random.Range(20f, 50f);

        coinsReward = Random.Range(10, 50);
        Cheese = new CheeseMass(amount, 
            CheeseMass.StatEnumToVector3(ECheeseMassStats.Greasy) *fatContent + CheeseMass.StatEnumToVector3(ECheeseMassStats.Spicy) * cheesetype + CheeseMass.StatEnumToVector3(ECheeseMassStats.Molten) * cheeseconsistency);
    }


    public override string ToString()
    {
        return $"{Cheese.ToString()} Reward: {coinsReward} coins";
    }
}
