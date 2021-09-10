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

    public GameObject descriptionPanel;

    public GameObject closeObject;

    public JSONNode armodels;

    UnityWebRequest www;
    // Start is called before the first frame update
    void Start()
    {
        descriptionPanel = GameObject.Find("DescriptionPanel");
        closeObject = GameObject.Find("Closebtn");
        closeObject.SetActive(false);
        descriptionPanel.SetActive(false);
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
                    newObj.name = museums[i]["id"];
                    newObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = museums[i]["name"];
                    string arId = museums[i]["id"];
                    newObj.GetComponent<Button>().onClick.AddListener(() => setDescription(descriptionPanel,arId));
                    // newObj.GetComponentInChildren<
                    // update the image of newObj
                    var image = newObj.GetComponentInChildren<Image>();
                    StartCoroutine(LoadImageInternet(museums[i]["image"], image));
                }
            }
        }
    }

    IEnumerator setARDescription(string model_id, GameObject modelTitle, GameObject modelDescription)
    {
        string ARURL = $"https://augment-tours-backend.herokuapp.com/armodels/{model_id}";

        Debug.Log("url Description " + ARURL);

        using (UnityWebRequest request = UnityWebRequest.Get(ARURL))
        {

            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                Debug.Log("Error: " + request.error);
            }
            else
            {
                //Debug.Log("Ar models"+ JSON.Parse(request.downloadHandler.text));
                //Debug.Log("request data: "+request.downloadHandler.text);
                armodels = JSON.Parse(request.downloadHandler.text);
                Debug.Log("Description Armodel  " + armodels);
                //Debug.Log("Json object " + obj["id"].Value);
                modelTitle.GetComponent<TMPro.TextMeshProUGUI>().text = armodels["name"].Value;
                modelDescription.GetComponent<TMPro.TextMeshProUGUI>().text = armodels["description"].Value;
                closeObject.SetActive(true);
                Button closeButton = GameObject.Find("Closebtn").GetComponent<Button>();
                closeButton.onClick.AddListener(() => closeDescription(descriptionPanel));

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

    void setDescription(GameObject panel, string arId)
    {

        panel.SetActive(true);
        Animator animator = panel.GetComponent<Animator>();
        bool isOpen = animator.GetBool("Open");
        animator.SetBool("Open", !isOpen);
        
        GameObject modelTitle = GameObject.Find("ModelTitle");
        GameObject modelDescription = GameObject.Find("ModelDescription");

        
        StartCoroutine(setARDescription(arId, modelTitle, modelDescription));
        
    }

    void closeDescription(GameObject panel)
    {
        Animator animator = panel.GetComponent<Animator>();
        bool isOpen = animator.GetBool("Open");
        animator.SetBool("Open", !isOpen);
        panel.SetActive(false);
       
    }
}

// [{"id":"d9e7a65a-1d79-4776-8249-93c3ddb4ddbb","name":"6k","description":"new museum","image":"url"}]