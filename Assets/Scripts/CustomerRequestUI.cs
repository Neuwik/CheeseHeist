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
        playerCheeseSelection = FindObjectOfType<PlayerCheeseSelection>();
        DisplayRequests();
        submitButton.onClick.AddListener(OnSubmit);
    }

    void DisplayRequests()
    {
        foreach (Transform child in requestListContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (CustomerRequest request in requestManager.requests)
        {
            GameObject requestItem = Instantiate(requestItemPrefab, requestListContent.transform);
            requestItem.GetComponentInChildren<Text>().text = request.ToString();
            requestItem.GetComponent<Button>().onClick.AddListener(() => OnRequestSelected(request));
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
            playerCheeseSelection.SelectCheese(selectedRequest.cheeseType, selectedRequest.consistency, selectedRequest.fatContent);

            // Hide the UI after selection
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("No request selected!");
        }
    }
}
