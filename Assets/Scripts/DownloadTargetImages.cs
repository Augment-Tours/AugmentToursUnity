using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Vuforia;

public class DownloadTargetImages : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(CreateImageTargetFromDownloadedTexture());
    }

    IEnumerator CreateImageTargetFromDownloadedTexture()
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture("https://firebasestorage.googleapis.com/v0/b/augmenttours.appspot.com/o/Targets%2Ftarget1.jpg?alt=media&token=9eefce3d-3d2a-45a0-8f3d-77525dd9df97"))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
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
}