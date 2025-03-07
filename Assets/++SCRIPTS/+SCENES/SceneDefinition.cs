using UnityEngine;

/// <summary>
/// ScriptableObject to define scene data
/// </summary>
[CreateAssetMenu(fileName = "New Scene Definition", menuName = "Gangsta Bean/Scene Definition", order = 2)]
public class SceneDefinition : ScriptableObject
{
    [Header("Scene Information")]
    [Tooltip("Scene type (must match an entry in GameScene.Type)")]
    public GameScene.Type sceneType;
    
    [Tooltip("Display name shown to players")]
    public string displayName;
    
    [Tooltip("Description of the scene")]
    [TextArea(3, 5)]
    public string description;
    
    [Header("Visuals")]
    [Tooltip("Icon or preview image for this scene")]
    public Sprite previewImage;
    
    [Tooltip("Image shown during level transition")]
    public Sprite sceneImage;
    
    [Header("Music & Atmosphere")]
    [Tooltip("Music track to play in this scene")]
    public AudioClip backgroundMusic;
    
    [Header("Debug")]
    [Tooltip("Notes for developers")]
    [TextArea(3, 5)]
    public string developerNotes;
}