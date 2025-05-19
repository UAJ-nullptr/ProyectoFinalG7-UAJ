using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoUIPlayer : MonoBehaviour
{
    [SerializeField]
    private VideoClip video;

    private RawImage displayerImage;
    private VideoPlayer videoPlayer;
    private RenderTexture texture;

    private void CreateTexture()
    {
        texture = new RenderTexture(1344, 756, 0);
        texture.name = "videoAuxTexture";
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        videoPlayer.Play();
    }

    void Start()
    {
        displayerImage = GetComponent<RawImage>();
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        if (video != null || displayerImage == null)
        {
            CreateTexture();
            videoPlayer.targetTexture = texture;
            videoPlayer.clip = video;
            displayerImage.texture = texture;
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += OnVideoPrepared;
        }
    }

    void OnDestroy()
    {
        // Limpieza
        if (texture != null)
        {
            texture.Release();
            Destroy(texture);
        }
    }
}
