/*
  This sample code is for demonstrating and testing the functionality
  of Unity Capture, and is placed in the public domain.
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CubesSwayBeeps : MonoBehaviour
{
    public float RotationAmount = 2f, RotationSpeedX = 0.5f, RotationSpeedY = 0.75f;
    public int CubeCount = 1;
    public float BeepDuration = 1/60f*3;
    public Color[] PerFrameBackgroundColors = new Color[] { Color.gray };

    private WebCamTexture WebCamTexture = null;
    public RawImage RawImage;

    Camera CameraComp;
    Transform[] CubeTransforms;
    Vector3[] CubeSpeeds;

    public GameObject Plane;
    public GameObject DropdownGO;

    private bool isInitWorldDone = false;

    void Start()
    {
        var devicelist = new List<string>();
        devicelist.Add("select a camera");
        WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log(devices[i].name);
            devicelist.Add(devices[i].name);
        }
        var dropdown = this.DropdownGO.GetComponent<Dropdown>();

        dropdown.AddOptions(devicelist);
        dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(dropdown);
        });

        CameraComp = GetComponent<Camera>();

    }


    private void InitWorld()
    {
        int TotalCubeCount = CubeCount * CubeCount * CubeCount;
        CubeTransforms = new Transform[TotalCubeCount];
        CubeSpeeds = new Vector3[TotalCubeCount];
        GameObject CubeHolder = new GameObject("CubeHolder");
        for (int i = 0; i < TotalCubeCount; i++)
        {
            int x = (i / (CubeCount * CubeCount)), y = ((i / CubeCount) % CubeCount), z = (i % CubeCount);
            GameObject o = GameObject.CreatePrimitive(PrimitiveType.Cube);
            o.transform.parent = CubeHolder.transform;
            o.transform.position = new Vector3(5 * (x - CubeCount / 2), 5 * (y - CubeCount / 2), 5 * (z - CubeCount / 2));
            //Textures[i] = o.GetComponent<Renderer>().material.mainTexture;
            o.GetComponent<Renderer>().material.mainTexture = RawImage.texture;
            CubeTransforms[i] = o.transform;
            CubeSpeeds[i] = Random.insideUnitSphere * 50;
        }

        //helps with keeping the rendered when producing the appication
        //otherwise we get a pink texture
        Plane.GetComponent<Renderer>().material.mainTexture = RawImage.texture;

        isInitWorldDone = true;
    }

    void DropdownValueChanged(Dropdown change)
    {
        //Debug.Log(change.value);
        //Debug.Log(change.captionText.text);

        this.WebCamTexture = new WebCamTexture(change.captionText.text, 480, 640, 30);
        this.RawImage.texture = this.WebCamTexture;
        if (this.WebCamTexture != null)
        {
            this.WebCamTexture.Play();

            Debug.Log(change.captionText.text + " connected");
        }

        if(!isInitWorldDone)
            this.InitWorld();
    }


    void Update()
    {
        if (CubeTransforms != null)
        {
            transform.localRotation = Quaternion.Euler(new Vector3(Mathf.Cos(Time.time * RotationSpeedX), Mathf.Sin(Time.time * RotationSpeedY)) * RotationAmount);
        
            for (int i = 0; i < CubeTransforms.Length; i++)
            {
                CubeTransforms[i].rotation = Quaternion.Euler(CubeSpeeds[i] * Time.time);
            }
        }


        CameraComp.backgroundColor = PerFrameBackgroundColors[Time.frameCount % PerFrameBackgroundColors.Length];

    }
}