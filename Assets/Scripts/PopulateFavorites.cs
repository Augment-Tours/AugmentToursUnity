using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Models;
using SimpleJSON;
using Firebase.Auth;
public class PopulateFavorites : MonoBehaviour
{
    // make the favorite dynamic based on the logged in user
    private const string URL = "https://augment-tours-backend.herokuapp.com/favorites/getBy/email";

    public GameObject prefab;

    public int numberToCreate;

    public JSONNode museums;
    // Start is called before the first frame update
    void Start()
    {
        Populate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Populate() 
    {
        GenerateRequest();
        
    }

    void GenerateRequest(){
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;
        StartCoroutine(ProcessRequest(URL + "?email=" + user.Email));
    }

    private IEnumerator ProcessRequest(string uri)
    {
        Debug.Log(uri);
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {

            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                Debug.Log("Error: " + request.error);
            }
            else
            {
                museums = JSON.Parse(request.downloadHandler.text);

                Debug.Log("Received: " + request.downloadHandler.text);
                Debug.Log("Received - objId - " + museums[0]["id"]);
            
                GameObject newObj;
                for (int i = 0; i < museums.Count; i++) {
                    newObj = (GameObject) Instantiate(prefab, transform);
                    newObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = museums[i]["name"];
                }
            }
        }
    }
}

// [{"id":"d9e7a65a-1d79-4776-8249-93c3ddb4ddbb","name":"6k","description":"new museum","image":"url"}]