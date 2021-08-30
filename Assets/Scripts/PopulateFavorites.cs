using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Models;
using SimpleJSON;
using Firebase.Auth;
using TMPro;

public class PopulateFavorites : MonoBehaviour
{
    // make the favorite dynamic based on the logged in user
    private const string URL = "https://augment-tours-backend.herokuapp.com/favorites/getBy/email";

    private const string PROFILE_URL = "https://augment-tours-backend.herokuapp.com/accounts/getBy/email";

    public GameObject prefab;

    public TMP_Text username;

    public int numberToCreate;

    public JSONNode museums;

    UnityWebRequest www;
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

    void GenerateRequest()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;
        StartCoroutine(ProcessRequest(URL + "?email=" + user.Email));
        StartCoroutine(LoadUserName(PROFILE_URL + "?email=" + user.Email));
    }

    private IEnumerator LoadUserName(string uri)
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
                var fetchedUser = JSON.Parse(request.downloadHandler.text);

                Debug.Log("Recieved: " + request.downloadHandler.text);
                username.text = fetchedUser["name"];
            }
        }
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
                for (int i = 0; i < museums.Count; i++)
                {
                    newObj = (GameObject)Instantiate(prefab, transform);
                    newObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = museums[i]["name"];
                    // newObj.GetComponentInChildren<
                    // update the image of newObj
                    var imageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b6/Image_created_with_a_mobile_phone.png/220px-Image_created_with_a_mobile_phone.png";
                    var image = newObj.GetComponentInChildren<Image>();
                    StartCoroutine(LoadImageInternet(imageUrl, image));
                }
            }
        }
    }

    IEnumerator LoadImageInternet(string imageUrl,Image web_image)
    {
        Texture2D tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        WWW Link = new WWW(imageUrl);
        yield return Link;
        Link.LoadImageIntoTexture(tex);
        web_image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
    }
}

// [{"id":"d9e7a65a-1d79-4776-8249-93c3ddb4ddbb","name":"6k","description":"new museum","image":"url"}]