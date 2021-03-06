using Firebase.Auth;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ArDescription : MonoBehaviour
{
    string arName;
    string arId;
    public GameObject descriptionPanel;
    public JSONNode armodels;
    public JSONNode account;
    public Button button;
    bool favorited = false;
    Sprite favFilledSprite;
    Sprite favHollowSprite;
    string star = "";
    string accountID = "";

    // Start is called before the first frame update
    void Start()
    {
        favFilledSprite = Resources.Load<Sprite>("favorite_filled");
        favHollowSprite = Resources.Load<Sprite>("favorite_hollow");
        descriptionPanel = GameObject.Find("DescriptionPanel");
        descriptionPanel.SetActive(false);

        //Debug.Log("Description panel " + descriptionPanel);
    }

    void OnEnable()
    {
        Lean.Touch.LeanTouch.OnFingerTap += HandleFingerTap;
    }

    void OnDisable()
    {
        Lean.Touch.LeanTouch.OnFingerTap -= HandleFingerTap;
    }

    void HandleFingerTap(Lean.Touch.LeanFinger finger)
    {
        if (finger.TapCount == 2)
        {
            RaycastHit Hit;
            Physics.Raycast(finger.GetRay(), out Hit);
            //arId = Hit.transform.parent.name;
            arId = Hit.transform.name;
            descriptionPanel.SetActive(true);

            GameObject modelTitle = GameObject.Find("ModelTitle");
            GameObject modelDescription = GameObject.Find("ModelDescription");
            Button favButton = GameObject.Find("Favbtn").GetComponent<Button>();
            toggleFavoriteButton(arId, favButton);
            favButton.onClick.AddListener(() => addFavorite(arId, favButton));
            Button closeButton = GameObject.Find("Closebtn").GetComponent<Button>();
            closeButton.onClick.AddListener(() => closeDescription(descriptionPanel));

            StartCoroutine(setARDescription(arId, modelTitle, modelDescription));
        }

    }

    // Update is called once per frame
    void Update()
    {


        //if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        //{

        //    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        //    Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);
        //    RaycastHit Hit;

        //    //Debug.Log("hit " + Physics.Raycast(ray, out Hit));

        //    if (Physics.Raycast(ray, out Hit))
        //    {
        //        //Debug.Log("touch count " + Input.touchCount);
        //        arId = Hit.transform.parent.name;
        //        //string arID = .ToString();
        //        //Debug.Log("clicked AR Model " + btnName);
        //        //StartCoroutine(Upload(btnName));
        //        Debug.Log("Description ar ID " + Hit.transform.name);
        //        //descriptionPanel.SetActive(true);
                
        //        GameObject modelTitle = GameObject.Find("ModelTitle");
        //        GameObject modelDescription = GameObject.Find("ModelDescription");
        //        Button favButton = GameObject.Find("Favbtn").GetComponent<Button>();
        //        toggleFavoriteButton(arId, favButton);
        //        favButton.onClick.AddListener(() => addFavorite(arId));

        //        Button closeButton = GameObject.Find("Closebtn").GetComponent<Button>();
        //        closeButton.onClick.AddListener(() => closeDescription(descriptionPanel));
        //        //GameObject favebtn = GameObject.Find("Favbtn");
        //        //favebtn.AddComponent<>
        //        Debug.Log("model description " + modelDescription);
        //        StartCoroutine(setARDescription(arId,modelTitle,modelDescription));
        //        Debug.Log("armodels " + armodels);
        //       // modelTitle.GetComponent<TMPro.TextMeshProUGUI>().text = armodels["name"].Value;
        //        //modelDescription.GetComponent<TMPro.TextMeshProUGUI>().text = armodels["description"].Value;

        //    }
        //    //    for (int i = 0; i + touchCorrection < Input.touchCount; ++i)
        //    //{

        //    //    Debug.Log("touch");
        //    //    if (Input.GetTouch(i).phase.Equals(TouchPhase.Began))
        //    //    {
        //    //        // Construct a ray from the current touch coordinates
        //    //        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
        //    //        if (Physics.Raycast(ray, out hit))
        //    //        {

        //    //            //hit.transform.gameObject.SendMessage("OnMouseDown");
        //    //        }
        //    //    }
        //    //}

        //}
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

    void closeDescription(GameObject panel)
    {
        panel.SetActive(false);
    }

    void toggleFavoriteButton (string model_id,Button favButton)
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;

        if(user == null)
        {

        }

        else
        {
            Debug.Log("Favorite User is " + user.Email);
            StartCoroutine(checkUserFavorite(user.Email, model_id ,favButton));
        }
       
        

    }
    
    IEnumerator Upload(string armodels_id, Button FavoriteButton)
    {
        Debug.Log("upload armodels id "+armodels_id);
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

            FavoriteButton.image.sprite = favHollowSprite;
        }
        else
        {
            favorited = true;
            FavoriteButton.image.sprite = favFilledSprite;
        }
    }

   IEnumerator checkUserFavorite (string email, string model_id, Button FavoriteButton)
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
                Debug.Log("Favorite Url: "+ favURL);
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

   IEnumerator setARDescription (string model_id, GameObject modelTitle, GameObject modelDescription)
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
                armodels= JSON.Parse(request.downloadHandler.text);
                Debug.Log("Description Armodel  " + armodels);
                //Debug.Log("Json object " + obj["id"].Value);
                modelTitle.GetComponent<TMPro.TextMeshProUGUI>().text = armodels["name"].Value;
                modelDescription.GetComponent<TMPro.TextMeshProUGUI>().text = armodels["description"].Value;

            }
        }
    }
}

