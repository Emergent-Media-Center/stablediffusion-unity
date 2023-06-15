using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using Cinemachine;

public class TexturePainter : MonoBehaviour
{
    public Texture2D texture;
    public Material skyboxMat;
    public float brushScale = 1f;
    public BrushBase currentBrush;

    private Camera cam;
    [SerializeField] private CinemachineVirtualCamera vCamera;

    [SerializeField] private UnityEngine.UI.Image testImage;

    [SerializeField] float horizontalSampleAngleOffset = 90f;
    [SerializeField] float verticalSampleAngleOffset = 0f;
    [SerializeField] bool invertHorizontalUVSample = true;
    [SerializeField] bool invertVerticalUVSample = false;
    
    // Start is called before the first frame update
    // todo: load the last image painted by the user
    void Start()
    {
        texture = Instantiate(texture);
        currentBrush = new CircularBrush();
        cam = Camera.main;

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2));
        testImage.sprite = sprite;
    }

    // todo: pass brush structure
    void Paint(Vector2Int center, float scale, BrushBase brush)
    {
        brush.Draw(texture, center, scale);
        texture.Apply();
    }
    public void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector2Int texCoords = Vector2Int.zero;

            float x = ((Mathf.Repeat((Vector3.SignedAngle(Vector3.forward, Vector3.ProjectOnPlane(ray.direction, Vector3.up), Vector3.up) + 180) + horizontalSampleAngleOffset, 360)) / 360);
            if(invertHorizontalUVSample)
            {
                x = 1 - x;
            }

            //todo: Calculate y coordinate
            float y = (Vector3.SignedAngle(Vector3.up, ray.direction, Vector3.Cross(ray.direction, Vector3.up)) + verticalSampleAngleOffset) / 180;
            if (invertVerticalUVSample)
            {
                y = 1 - y;
            }

            Debug.DrawRay(transform.position, ray.direction);
            Debug.DrawRay(transform.position, Vector3.Cross(ray.direction, Vector3.up));
            Debug.Log(y);

            if(skyboxMat != null)
            {
                float bottom = skyboxMat.GetFloat("_BottomCapHeight");
                float top = skyboxMat.GetFloat("_TopCapHeight");

                //y = Remap(y, bottom, top, 0, 1);
            }
            else
            {
                Debug.LogError("Skybox material is null");
            }

            texCoords.x = (int)(x * texture.width);
            texCoords.y = (int)(y * texture.height);

            //Debug.Log(texCoords.y);

            if(texCoords.y > 0 && texCoords.y < texture.height)
            {
                Paint(texCoords, brushScale, currentBrush);
            }
        }

        if(Input.GetMouseButton(1))
        {
            vCamera.enabled = true;
        }
        else
        {
            vCamera.enabled = false;
        }
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return ((value - from1) / (to1 - from1) * (to2 - from2)) + from2;
    }
}
