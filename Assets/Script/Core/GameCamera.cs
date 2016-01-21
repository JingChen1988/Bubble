using UnityEngine;
/// <summary>
/// 手机分辨率自适应
/// 480*768	  10:16 	1.6
/// 800*1280  10:16	    1.6
/// 320*480	  2:3	    1.5
/// 480*800	  1:1.66	1.66
/// 540*960	  1:1.77	1.77
/// </summary>
public class GameCamera : MonoBehaviour
{
    public int Height = 768;
    public int Width = 480;
    public float Pixels = 10;//Pixels To Units

    float devHeight = 76.8f;
    float devWidth = 48f;//游戏有效宽度
    Camera Cam;

    void Start()
    {
        devHeight = Height / Pixels;
        devWidth = Width / Pixels;
        Cam = GetComponent<Camera>();
        float orthographicSize = Cam.orthographicSize;
        //计算高宽比
        float aspectRatio = (float)Screen.width / Screen.height;
        //计算相机宽度
        float cameraWidth = orthographicSize * 2 * aspectRatio;
        //相机宽度无法显示所有有效内容，则更改orthographicSize
        if (cameraWidth < devWidth)
        {
            orthographicSize = devWidth / (2 * aspectRatio);
            Cam.orthographicSize = orthographicSize;
        }
        Debug.Log(string.Format("CameraWidth={0}/{1} OrthographicSize={2}", cameraWidth, devWidth, orthographicSize));
    }
}
