using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Models;
using SimpleJSON;
using Firebase.Auth;
using TMPro;
using UnityEngine.SceneManagement;

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

    public GameObject favObject;

    public JSONNode armodels;

    public JSONNode account;
    bool favorited = false;
    Sprite favFilledSprite;
    Sprite favHollowSprite;
    string accountID = "";
    UnityWebRequest www;
    // Start is called before the first frame update
    void Start()
    {
        favFilledSprite = Resources.Load<Sprite>("favorite_filled");
        favHollowSprite = Resources.Load<Sprite>("favorite_hollow");
        descriptionPanel = GameObject.Find("DescriptionPanel");
        closeObject = GameObject.Find("Closebtn");
        favObject = GameObject.Find("Favbtn");
        closeObject.SetActive(false);
        favObject.SetActive(false);
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
                favObject.SetActive(true);
                Button closeButton = GameObject.Find("Closebtn").GetComponent<Button>();
                Button favButton = GameObject.Find("Favbtn").GetComponent<Button>();
                closeButton.onClick.AddListener(() => closeDescription(descriptionPanel));
                toggleFavoriteButton(model_id, favButton);
                favButton.onClick.AddListener(() => addFavorite(model_id, favButton));

            }
        }
    }

    void addFavorite(string id, Button favButton)
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;

        if (user == null)
        {
            SceneManager.LoadScene("Signin");
        }

        else
        {
            Debug.Log("Add to Favorite user is " + accountID);
            StartCoroutine(Upload(id, favButton));
        }

    }

    IEnumerator Upload(string armodels_id, Button FavoriteButton)
    {
        Debug.Log("upload armodels id " + armodels_id);
        WWWForm form = new WWWForm();
        form.AddField("armodels_id", armodels_id);
        form.AddField("accounts_id", accountID);

        using (UnityWebRequest www = UnityWebRequest.Post("https://augment-tours-backend.herokuapp.com/favorites", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }

        if (favorited == true)
        {
            Debug.Log("favorited picture ");
            favorited = false;

            GameObject arModelObject = GameObject.Find("Canvas/Panel/Scroll View/Viewport/Content/" + armodels_id);
            Destroy(arModelObject);

           
        }
        else
        {
            GameObject arModelObject = GameObject.Find("Canvas/Panel/Scroll View/Viewport/Content/" + armodels_id);
            Destroy(arModelObject);
        }
    }

    void toggleFavoriteButton(string model_id, Button favButton)
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;

        if (user == null)
        {

        }

        else
        {
            Debug.Log("Favorite User is " + user.Email);
            StartCoroutine(checkUserFavorite(user.Email, model_id, favButton));
        }



    }

    IEnumerator checkUserFavorite(string email, string model_id, Button FavoriteButton)
    {
        string accountURL = $"https://augment-tours-backend.herokuapp.com/accounts/getBy/email/?email={email}";
        Debug.Log("Account URL " + accountURL);

        using (UnityWebRequest request = UnityWebRequest.Get(accountURL))
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
                account = JSON.Parse(request.downloadHandler.text);
                Debug.Log("Favorite Recieved account " + request.downloadHandler.text);
                accountID = account["id"].Value;

                string favURL = $"https://augment-tours-backend.herokuapp.com/favorites/{accountID}/{model_id}";
                Debug.Log("Favorite Url: " + favURL);
                using (UnityWebRequest favRequest = UnityWebRequest.Get(favURL))
                {

                    yield return favRequest.SendWebRequest();

                    if (favRequest.isNetworkError)
                    {
                        Debug.Log("Error: " + request.error);
                    }
                    else
                    {
                        //Debug.Log("Ar models"+ JSON.Parse(request.downloadHandler.text));
                        //Debug.Log("request data: "+request.downloadHandler.text);
                        Debug.Log("Favorite Response " + favRequest.downloadHandler.text);
                        //JSONNode favResponse = JSON.Parse(favRequest.downloadHandler.text);
                        //Debug.Log("Favorite JSON " + favResponse);
                        favorited = favRequest.downloadHandler.text == "true";
                        if (favorited == true)
                        {
                            Debug.Log("favorited picture ");


                            FavoriteButton.image.sprite = favFilledSprite;
                        }
                        else
                        {
                            FavoriteButton.image.sprite = favHollowSprite;
                        }

                        //Debug.Log("Json object " + obj["id"].Value);



                    }
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