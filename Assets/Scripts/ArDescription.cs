using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ArDescription : MonoBehaviour
{
    string arName;
    string arId;
    public GameObject descriptionPanel;
    public JSONNode armodels;
    public Button button; 

    // Start is called before the first frame update
    void Start()
    {
        descriptionPanel = GameObject.Find("DescriptionPanel");
        descriptionPanel.SetActive(false);

        //Debug.Log("Description panel " + descriptionPanel);
    }

    // Update is called once per frame
    void Update()
    {


        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
           
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            Debug.DrawRay(ray.origin,ray.direction* 10f, Color.red);
            RaycastHit Hit;
            
            Debug.Log("hit " + Physics.Raycast(ray, out Hit));

            if (Physics.Raycast(ray, out Hit))
            {
                //Debug.Log("touch count " + Input.touchCount);
                arId = Hit.transform.name;
                //Debug.Log("clicked AR Model " + btnName);
                //StartCoroutine(Upload(btnName));
                Debug.Log("ar ID " + arId);
                descriptionPanel.SetActive(true);
                GameObject modelTitle = GameObject.Find("ModelTitle");
                GameObject modelDescription = GameObject.Find("ModelDescription");
                Button favButton = descriptionPanel.GetComponentInChildren<Button>();
                favButton.onClick.AddListener(() => addFavorite(arId));
                //GameObject favebtn = GameObject.Find("Favbtn");
                //favebtn.AddComponent<>
                Debug.Log("model description " + modelDescription);
                StartCoroutine(setARDescription(arId,modelTitle,modelDescription));
                Debug.Log("armodels " + armodels);
               // modelTitle.GetComponent<TMPro.TextMeshProUGUI>().text = armodels["name"].Value;
                //modelDescription.GetComponent<TMPro.TextMeshProUGUI>().text = armodels["description"].Value;

            }
            //    for (int i = 0; i + touchCorrection < Input.touchCount; ++i)
            //{

            //    Debug.Log("touch");
            //    if (Input.GetTouch(i).phase.Equals(TouchPhase.Began))
            //    {
            //        // Construct a ray from the current touch coordinates
            //        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
            //        if (Physics.Raycast(ray, out hit))
            //        {

            //            //hit.transform.gameObject.SendMessage("OnMouseDown");
            //        }
            //    }
            //}

        }
    }


    void addFavorite(string id)
    {
        StartCoroutine(Upload(id));
    }

    IEnumerator Upload(string armodels_id)
    {
        Debug.Log("upload armodels id "+armodels_id);
        WWWForm form = new WWWForm();
        form.AddField("armodels_id", armodels_id);
        form.AddField("accounts_id", "2ec3a896-16db-4fa3-9129-5f1019e2d353");

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
    }

   IEnumerator setARDescription (string model_id, GameObject modelTitle, GameObject modelDescription)
    {
        string ARURL = $"https://augment-tours-backend.herokuapp.com/armodels/{model_id}";

        Debug.Log("url " + ARURL);

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
                //Debug.Log("Json object " + obj["id"].Value);
                modelTitle.GetComponent<TMPro.TextMeshProUGUI>().text = armodels["name"].Value;
                modelDescription.GetComponent<TMPro.TextMeshProUGUI>().text = armodels["description"].Value;

            }
        }
    }
}
