using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Models;
using SimpleJSON;
using Firebase.Auth;
public class PopulateMuseums : MonoBehaviour
{
    private const string URL = "https://augment-tours-backend.herokuapp.com/museums/";

    public GameObject prefab;

    public int numberToCreate;

    public JSONNode museums;
    // Start is called before the first frame update
    void Start()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;
        Debug.Log("User " + user.Email);
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
                museums = JSON.Parse(request.downloadHandler.text);

                Debug.Log("Received: " + request.downloadHandler.text);
                Debug.Log("Received - objId - " + museums[0]["id"]);
            
                GameObject newObj;
                for (int i = 0; i < museums.Count; i++) {
                    newObj = (GameObject) Instantiate(prefab, transform);
                    newObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = museums[i]["name"];
                    var image = newObj.GetComponentInChildren<Image>();
                    StartCoroutine(LoadImageInternet(museums[i]["image"], image));
                }
            }
        }
    }

    IEnumerator LoadImageInternet(string imageUrl, Image web_image)
    {
        Texture2D tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        WWW Link = new WWW(imageUrl);
        yield return Link;
        Link.LoadImageIntoTexture(tex);
        web_image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
    }
}

// [{"id":"d9e7a65a-1d79-4776-8249-93c3ddb4ddbb","name":"6k","description":"new museum","image":"url"}]