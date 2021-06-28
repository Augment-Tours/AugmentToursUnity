using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PopulateMuseums : MonoBehaviour
{
    private const string URL = "https://augment-tours-backend.herokuapp.com/museums/";

    public GameObject prefab;

    public int numberToCreate;
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
        GameObject newObj;
        GenerateRequest();
        for (int i = 0; i < numberToCreate; i++) {
            newObj = (GameObject) Instantiate(prefab, transform);
        }
    }

    void GenerateRequest(){
        StartCoroutine(ProcessRequest(URL));
    }

    private IEnumerator ProcessRequest(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {

            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                Debug.Log("Error: " + request.error);
            }
            else
            {
                Debug.Log("Received: " + request.downloadHandler.text);
            }
        }
    }
}
