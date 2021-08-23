using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Vuforia;
using SimpleJSON;
using TMPro;

public class MyDefaultTrackableEventHandler : DefaultTrackableEventHandler
{

    public JSONNode targets;
    public TMP_Text status;


    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
        if (Global.trackedTarget != base.gameObject.name)
        {
            Debug.Log(base.gameObject.name);
            string URL = $"https://augment-tours-backend.herokuapp.com/targets/museums/{base.gameObject.name}";
            StartCoroutine(ProcessRequest(URL));
            Global.trackedTarget = base.gameObject.name;
            status = GameObject.Find("Status").GetComponent<TMP_Text>();
            status.text = "Museum target Scanned";
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

        Debug.Log("in create image 2");
        for (int i = 0; i < targets.Count; i++)
        {
            Target target = new Target(targets[i]["id"], targets[i]["information"], targets[i]["model"], targets[i]["x_location"], targets[i]["y_location"], targets[i]["floor"], targets[i]["museums_id"]);
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(target.model))
            {
                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    Debug.Log("model: " + target.model);
                    var objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();

                    // Get downloaded texture once the web request completes
                    var texture = DownloadHandlerTexture.GetContent(uwr);

                    // get the runtime image source and set the texture
                    var runtimeImageSource = objectTracker.RuntimeImageSource;
                    runtimeImageSource.SetImage(texture, 1.4f, target.id);

                    // create a new dataset and use the source to create a new trackable
                    var dataset = objectTracker.CreateDataSet();
                    var trackableBehaviour = dataset.CreateTrackable(runtimeImageSource, target.id);

                    // add the DefaultTrackableEventHandler to the newly created game object
                    
                    trackableBehaviour.gameObject.AddComponent<ARModelTrackableBehaviour>();
                    //trackableBehaviour.gameObject.AddComponent<BoxCollider>();
                    //trackableBehaviour.gameObject.AddComponent<ArDescription>();


                    // activate the dataset
                    objectTracker.ActivateDataSet(dataset);
                    

                    //string targetURL = "https://augment-tours-backend.herokuapp.com/targets/museums/c091fb5c-ae4c-407d-b41c-93beb335ce6d";



                    //string ARURL = $"https://augment-tours-backend.herokuapp.com/armodels/c091fb5c-ae4c-407d-b41c-93beb335ce6d/{target.floor}";

                    //Debug.Log("url " + ARURL);

                    //using (UnityWebRequest request = UnityWebRequest.Get(ARURL))
                    //{

                    //    yield return request.SendWebRequest();

                    //    if (request.isNetworkError)
                    //    {
                    //        Debug.Log("Error: " + request.error);
                    //    }
                    //    else
                    //    {
                    //        armodels = JSON.Parse(request.downloadHandler.text);


                    //    }
                    //}

                    //for (int j = 0; j < armodels.Count; j++)
                    //{
                    //    Armodel armodel = new Armodel(armodels[j]["id"], armodels[j]["name"], armodels[j]["description"], armodels[j]["model"], armodels[j]["x_location"], armodels[j]["y_location"], armodels[j]["floor"], armodels[j]["museums_id"]);
                    //    string filePath = $"{Application.persistentDataPath}/Files/{armodel.id}.gltf";


                    //    Debug.Log("Armodel "+j +" " + armodel.model);

                    //    // TODO: add virtual content as child object(s)
                    //    GameObject sphere = new GameObject();
                    //    //sphere.gameObject.AddComponent<BoxCollider>();
                    //    //sphere.AddComponent<BoxCollider>();
                    //    DownloadArmodel(sphere, filePath, armodel, trackableBehaviour);


                    //    //sphere.AddComponent<BoxCollider>();
                    //    //sphere.transform.SetParent(trackableBehaviour.gameObject.transform);
                    //    sphere.transform.position = new Vector3(armodel.x_location, armodel.y_location, 679.2593f);
                    //    sphere.transform.parent = GameObject.Find("/Canvas").transform;
                    //    //sphere.name = armodel.id;
                    //    Debug.Log("sphere " + sphere);
                    //    //cube.AddComponent<BoxCollider>();





                    //}


                }
            }

        }



    }
}

