using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureImage : MonoBehaviour
{
    public int superSize = 2;
    private int _shotIndex = 0;
    public GameObject panel;
    public GameObject captureObject; 
    // Start is called before the first frame update
    void Start()
    {
        panel = GameObject.Find("Panel");
        captureObject = GameObject.Find("CaptureButton");
        Button captureButton = GameObject.Find("CaptureButton").GetComponent<Button>();
        Debug.Log("capture image");
        captureButton.onClick.AddListener(() => capture());
    }

    // Update is called once per frame
    void Update()
    {
       

    }
    void capture()
    {
        panel.SetActive(false);
        //captureObject.SetActive(false);

        StartCoroutine(TakeScreenshotAndSave());

        panel.SetActive(true);
        //captureObject.SetActive(true);
    }

    //void capture()
    //{
    //    panel.SetActive(false);
    //    captureObject.SetActive(false);
    //    Application.CaptureScreenshot("Screeenshot{_shotIndex}.png");
    //    ScreenCapture.CaptureScreenshot($"Screeenshot{_shotIndex}.png", superSize);
    //    _shotIndex++;
    //    Debug.Log("capture image");
    //    panel.SetActive(true);
    //    captureObject.SetActive(true);

    //}
    private IEnumerator TakeScreenshotAndSave()
    {
       
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();
        
        // Save the screenshot to Gallery/Photos
        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(ss, "GalleryTest", "Image.png", (success, path) => Debug.Log("Media save result: " + success + " " + path));

        Debug.Log("Permission result: " + permission);

        // To avoid memory leaks
        Destroy(ss);


    }

}
