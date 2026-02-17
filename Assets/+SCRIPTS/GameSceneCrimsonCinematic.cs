using __SCRIPTS;
using UnityEngine;
using UnityEngine.Video;

public class GameSceneCrimsonCinematic : GameScene
{
    VideoPlayer _videoPlayer;
    public SceneDefinition crimsonScene;
	void Start()
    {
	    _videoPlayer = GetComponent<VideoPlayer>();
	    _videoPlayer.Play();
	    _videoPlayer.loopPointReached += VideoPlayer_loopPointReached;
    }

	void VideoPlayer_loopPointReached(VideoPlayer source)
	{
		Services.sceneLoader.GoToScene(crimsonScene);
	}

	// Update is called once per frame
    void Update()
    {

    }
}
