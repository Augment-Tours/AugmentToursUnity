using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Vuforia;
using SimpleJSON;
using System;
using Siccity.GLTFUtility;

public class DownloadTargetImages : MonoBehaviour
{
    private const string URL = "https://augment-tours-backend.herokuapp.com/targets/type/museums";
    //private const string URL = "https://augment-tours-backend.herokuapp.com/targets/museums/c091fb5c-ae4c-407d-b41c-93beb335ce6d";

    public JSONNode targets;
    public JSONNode armodels;
  

    void Start()
    {
        
        StartCoroutine(ProcessRequest(URL));
        //StartCoroutine(CreateImageTargetFromDownloadedTexture());
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

        Debug.Log("in create image");
        for(int i = 0; i< targets.Count; i++)
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
                    runtimeImageSource.SetImage(texture, 1.4f, target.museums_id);

                    // create a new dataset and use the source to create a new trackable
                    var dataset = objectTracker.CreateDataSet();
                    var trackableBehaviour = dataset.CreateTrackable(runtimeImageSource, target.museums_id);

                    // add the DefaultTrackableEventHandler to the newly created game object
                    trackableBehaviour.gameObject.AddComponent<MyDefaultTrackableEventHandler>();
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

    IEnumerator CreateImageTargetFromDownloadedTexture()
    {
      
        Debug.Log("in create image");
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(targets[0]["model"]))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                Debug.Log("model: " + targets[0]["model"]);
                var objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();

                // Get downloaded texture once the web request completes
                var texture = DownloadHandlerTexture.GetContent(uwr);

                // get the runtime image source and set the texture
                var runtimeImageSource = objectTracker.RuntimeImageSource;
                runtimeImageSource.SetImage(texture, 1.4f, "myTargetName");

                // create a new dataset and use the source to create a new trackable
                var dataset = objectTracker.CreateDataSet();
                var trackableBehaviour = dataset.CreateTrackable(runtimeImageSource, "myTargetName");

                // add the DefaultTrackableEventHandler to the newly created game object
                trackableBehaviour.gameObject.AddComponent<DefaultTrackableEventHandler>();
                
                // activate the dataset
                objectTracker.ActivateDataSet(dataset);


                // TODO: add virtual content as child object(s)
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
             
                sphere.transform.SetParent(trackableBehaviour.gameObject.transform);
                sphere.transform.position  = new Vector3(0.3f, 0.5f, 0);

                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                cube.transform.SetParent(trackableBehaviour.gameObject.transform);
                cube.transform.position = new Vector3(0.5f, 0.3f, 0);
            }
        }
    }


    public void DownloadArmodel(GameObject model, string filePath, Armodel armodel, DataSetTrackableBehaviour trackable)
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
                model.transform.SetParent(trackable.gameObject.transform);
                model.name = armodel.id;
                Debug.Log("Debugging ");
                model.transform.localScale = new Vector3(20f, 20f, 20f);
                //Type bodyType = GameObject.Find("body").GetType();
                //Debug.Log("Type " + bodyType.ToString());
                GameObject.Find(armodel.id+"/body").AddComponent<BoxCollider>();
                //model.GetComponent < UnityEngine.GameObject.Find("tiger");
                //Debug.Log("Game Object "+)>());s


               model.gameObject.AddComponent<BoxCollider>();
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