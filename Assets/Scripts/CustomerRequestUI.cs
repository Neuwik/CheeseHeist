using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomerRequestUI : MonoBehaviour
{
    public CustomerRequestManager requestManager;
    public GameObject requestListContent;
    public GameObject requestItemPrefab;
    public Button submitButton;

    private CustomerRequest selectedRequest;
    private PlayerCheeseSelection playerCheeseSelection;

    private void Start()
    {
        //playerCheeseSelection = FindObjectOfType<PlayerCheeseSelection>();
        //DisplayRequests();
        //submitButton.onClick.AddListener(OnSubmit);
    }

    public void DisplayRequests()
    {
        requestListContent.SetActive(false);
        foreach (Transform child in requestListContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (CustomerRequest request in requestManager.openRequests)
        {
            requestListContent.SetActive(true);
            GameObject requestItem = Instantiate(requestItemPrefab, requestListContent.transform);
            requestItem.GetComponentInChildren<TMP_Text>().text = request.ToString();
            //requestItem.GetComponent<Button>().onClick.AddListener(() => OnRequestSelected(request));
        }
    }

   
    void OnRequestSelected(CustomerRequest request)
    {
        selectedRequest = request;
        Debug.Log("Selected Request: " + request.ToString());
    }

    void OnSubmit()
    {
        if (selectedRequest != null)
        {
            //playerCheeseSelection.SelectCheese(selectedRequest.cheeseType, selectedRequest.consistency, selectedRequest.fatContent);

            // Hide the UI after selection
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("No request selected!");
        }
    }
}
