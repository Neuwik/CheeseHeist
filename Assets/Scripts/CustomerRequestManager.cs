using System.Collections.Generic;
using UnityEngine;

public class CustomerRequestManager : MonoBehaviour
{
    public int numberOfRequests = 5;
    public List<CustomerRequest> requests;

    private void Start()
    {
        GenerateCustomerRequests();
    }

    public void GenerateCustomerRequests()
    {
        requests = new List<CustomerRequest>();

        for (int i = 0; i < numberOfRequests; i++)
        {
            requests.Add(new CustomerRequest());
        }
    }
}
