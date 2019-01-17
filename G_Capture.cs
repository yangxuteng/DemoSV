using UnityEngine;
using System.Text;
using System;
using System.Collections;
using System.IO;

namespace G_Engine { 
public class G_Capture : MonoBehaviour {

        public static IEnumerator ScreenShoot(Rect rect)
        {
            //Wait for graphics to render
            yield return new WaitForEndOfFrame();

            //Create a texture to pass to encoding
            Texture2D texture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);

            //Put buffer into texture
            texture.ReadPixels(rect, 0, 0);

            //Split the process up--ReadPixels() and the GetPixels() call inside of the encoder are both pretty heavy
            yield return 0;

            byte[] bytes = texture.EncodeToPNG();

            string imagePath = "";
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    imagePath = Application.dataPath + "/TAQRCode.png";
                    break;
                case RuntimePlatform.Android:
                    imagePath = "/sdcard" + "/TAQRCode.png";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    imagePath = Application.dataPath + "/TAQRCode.png";
                    break;
            }

            //Save our test image (could also upload to WWW)
            File.WriteAllBytes(imagePath, bytes);

            //Tell unity to delete the texture, by default it seems to keep hold of it and memory crashes will occur after too many screenshots.
            DestroyObject(texture);

            MSMgr.Instance().StartShareSDK(MSMgr.ShareType.TravelAlbum, imagePath);
        }

        private IEnumerator ScreenShoot(string filePath)
        {
            //Wait for graphics to render
            yield return new WaitForEndOfFrame();

            RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
            Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

            //Render from all!
            foreach (Camera cam in Camera.allCameras)
            {
                cam.targetTexture = rt;
                cam.Render();
                cam.targetTexture = null;
            }

            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            Camera.main.targetTexture = null;

            //Added to avoid errors
            RenderTexture.active = null;
            Destroy(rt);

            //Split the process up--ReadPixels() and the GetPixels() call inside of the encoder are both pretty heavy
            yield return 0;

            byte[] bytes = screenShot.EncodeToPNG();
            File.WriteAllBytes(filePath, bytes);
        }


        /// <summary>  
        /// Captures the screenshot2.  
        /// </summary>  
        /// <returns>The screenshot2.</returns>  
        /// <param name="rect">Rect.截图的区域，左下角为o点</param>  
        public static Texture2D CaptureScreenshot(Rect rect)
        {
            // 先创建一个的空纹理，大小可根据实现需要来设置  
            Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);

            // 读取屏幕像素信息并存储为纹理数据，  
            screenShot.ReadPixels(rect, 0, 0);
            screenShot.Apply();

            // 然后将这些纹理数据，成一个png图片文件  
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = Application.dataPath + "/TAQRCode.png";
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("截屏了一张图片: {0}", filename));

            // 最后，我返回这个Texture2d对象，这样我们直接，所这个截图图示在游戏中，当然这个根据自己的需求的。  
            return screenShot;
        }

        /// <summary>  
        /// 对相机截图。   
        /// </summary>  
        /// <returns>The screenshot2.</returns>  
        /// <param name="camera">Camera.要被截屏的相机</param>  
        /// <param name="rect">Rect.截屏的区域</param>  
        public static Texture2D CaptureCamera(Camera camera, Rect rect)
        {
            // 创建一个RenderTexture对象  
            RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);
            // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
            camera.targetTexture = rt;
            camera.Render();
            //ps: --- 如果这样加上第二个相机，可以实现只截图某几个指定的相机一起看到的图像。  
            //ps: camera2.targetTexture = rt;  
            //ps: camera2.Render();  
            //ps: -------------------------------------------------------------------  

            // 激活这个rt, 并从中中读取像素。  
            RenderTexture.active = rt;
            Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素  
            screenShot.Apply();

            // 重置相关参数，以使用camera继续在屏幕上显示  
            camera.targetTexture = null;
            //ps: camera2.targetTexture = null;  
            RenderTexture.active = null; // JC: added to avoid errors  
            GameObject.Destroy(rt);
            // 最后将这些纹理数据，成一个png图片文件  
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = Application.dataPath + "/Screenshot.png";
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("截屏了一张照片: {0}", filename));

            return screenShot;
        }
    }
}