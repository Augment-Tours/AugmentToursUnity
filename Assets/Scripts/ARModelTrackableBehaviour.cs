using Siccity.GLTFUtility;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Vuforia;

public class ARModelTrackableBehaviour : DefaultTrackableEventHandler
{

    public JSONNode targets;
    public JSONNode armodels;
    public TMP_Text status;
    //DataSetTrackableBehaviour trackableBehaviour;



    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
        if(Global.trackedTarget != base.gameObject.name)
        {
            string targateId = base.gameObject.name;
            string URL = $"https://augment-tours-backend.herokuapp.com/targets/{base.gameObject.name}";
            StartCoroutine(ProcessRequest(URL));
            Global.trackedTarget = base.gameObject.name;
            //GameObject gp = GameObject.Find("Ground Plane Stage");
            //gp.transform.SetParent(GameObject.Find(Global.trackedTarget).transform);
            status = GameObject.Find("Status").GetComponent<TMP_Text>();
            status.text = "AR target Scanned";
        }
       
        

    }

    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();
        //transform.SetParent(gameObject.GetComponent<>(),true );
    }

    private IEnumerator ProcessRequest(string uri)
    {
        Debug.Log("in download");
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {

            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                Debug.Log("Error: " + request.error);
            }
            else
            {
                targets = JSON.Parse(request.downloadHandler.text);

            }
        }
        string museumId = targets["museums_id"];
        string museumFloor = targets["floor"];
        string ARURL = $"https://augment-tours-backend.herokuapp.com/armodels/{museumId}/{museumFloor}";

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
                armodels = JSON.Parse(request.downloadHandler.text);


            }
        }

        for (int j = 0; j < armodels.Count; j++)
        {
            Armodel armodel = new Armodel(armodels[j]["id"], armodels[j]["name"], armodels[j]["description"], armodels[j]["model"], armodels[j]["x_location"], armodels[j]["y_location"], armodels[j]["z_location"],armodels[j]["x_scale"], armodels[j]["y_scale"],armodels[j]["z_scale"], armodels[j]["floor"], armodels[j]["museums_id"]);
            string filePath = $"{Application.persistentDataPath}/Files/{armodel.id}.gltf";


            Debug.Log("Armodel " + j + " " + armodel.model);

            // TODO: add virtual content as child object(s)
           
            GameObject sphere = new GameObject();
            //sphere.gameObject.AddComponent<BoxCollider>();
            //sphere.AddComponent<BoxCollider>();
            DownloadArmodel(sphere, filePath, armodel);


            //sphere.AddComponent<BoxCollider>();
            //sphere.transform.SetParent(trackableBehaviour.gameObject.transform);
            Debug.Log("Model isss" + sphere);
            
            //sphere.transform.parent = GameObject.Find("Ground Plane Stage").transform;
            //sphere.name = armodel.id;
            Debug.Log("sphere " + sphere);
            //cube.AddComponent<BoxCollider>();





        }


    }

    public void DownloadArmodel(GameObject model, string filePath, Armodel armodel)
    {
        Debug.Log("in download file");
        StartCoroutine(GetFileRequest(filePath, armodel.model, (UnityWebRequest req) =>
        {
            if (req.isNetworkError || req.isHttpError)
            {
                // Log any errors that may happen
                Debug.Log($"{req.error} : {req.downloadHandler.text}");
            }
            else
            {
                model = Importer.LoadFromFile(filePath);
                model.transform.SetParent(GameObject.Find(base.gameObject.name).transform);
                model.name = armodel.id;
                Debug.Log("Debugging ");
                //model.transform.position = new Vector3(100, 100, 679.2593f);
                model.transform.position = new Vector3(armodel.x_location, armodel.y_location, armodel.z_location);
                model.transform.localScale = new Vector3(armodel.x_scale, armodel.y_scale, armodel.z_scale );
                Debug.Log("scale: " + armodel.x_scale);
                //Type bodyType = GameObject.Find("body").GetType();
                //Debug.Log("Type " + bodyType.ToString());

                GameObject.Find(base.gameObject.name +"/"+ armodel.id).AddComponent<BoxCollider>();

                GameObject.Find(base.gameObject.name + "/" + armodel.id).AddComponent<Lean.Touch.LeanSelectableByFinger>();
                GameObject.Find(base.gameObject.name + "/" + armodel.id).AddComponent<Lean.Touch.LeanDragTranslate>();
                GameObject.Find(base.gameObject.name + "/" + armodel.id).AddComponent<Lean.Touch.LeanTwistRotate>();
                GameObject.Find(base.gameObject.name + "/" + armodel.id).AddComponent<Lean.Touch.LeanPinchScale>();
                GameObject.Find(base.gameObject.name + "/" + armodel.id).AddComponent<Lean.Touch.LeanTwistRotateAxis>();
                //GameObject.Find("Ground Plane Stage/" + armodel.id).AddComponent<Lean.Touch.LeanTouch>();
                //model.GetComponent < UnityEngine.GameObject.Find("tiger");
                //Debug.Log("Game Object "+)>());s


                //model.gameObject.AddComponent<BoxCollider>();
            }
        }));

    }

    IEnumerator GetFileRequest(string filePath, string url, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.downloadHandler = new DownloadHandlerFile(filePath);
            yield return req.SendWebRequest();
            callback(req);
        }
    }
}
