using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomerRequestManager : MonoBehaviour
{
    public int numberOfRequests = 3;
    public List<CustomerRequest> requests;

    public CustomerRequestUI CustomerRequestUI;
    public bool allRequestsDone { get { return requests == null || requests.Where(r => r.Cheese.Mass > 0).ToList().Count == 0; } }
    private void Start()
    {
        //GenerateCustomerRequests();

    }
    public void GenerateCustomerRequests()
    {
        requests = new List<CustomerRequest>();

        for (int i = 0; i < numberOfRequests; i++)
        {
            CustomerRequest cstmrqst = new CustomerRequest();
            cstmrqst.GenerateRandomRequest();
            requests.Add(cstmrqst);
        }
        CustomerRequestUI.DisplayRequests();
    }

    public List<CustomerRequest> openRequests { get { return requests.Where(r => r.Cheese.Mass > 0).ToList(); } }  

    public float DeliverCheese(CheeseMass cheese)
    {
        float coins = 0;
        while (cheese.Mass > 0 && !allRequestsDone)
        {
            CustomerRequest customerRequest = null;
            float currentMatch = 0;
            foreach (var request in requests)
            {
                if (request.Cheese.Mass > 0)
                {
                    float match = request.Cheese.StatsMatch(cheese);
                    if (match > currentMatch)
                    {
                        currentMatch = match;
                        customerRequest = request;
                    }
                }
            }
            float deliveredMass = cheese.Mass;
            if(deliveredMass > customerRequest.Cheese.Mass)
            {
                deliveredMass = customerRequest.Cheese.Mass;
            }
            int gainedCoins = (int)(customerRequest.coinsReward * deliveredMass / customerRequest.Cheese.Mass);
            coins += gainedCoins;
            cheese.LooseMass(deliveredMass);
            customerRequest.Cheese.LooseMass(deliveredMass);
            customerRequest.coinsReward -= gainedCoins;
        }
        CustomerRequestUI.DisplayRequests();
        return coins; 
        
    }
}
