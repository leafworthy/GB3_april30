using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace __SCRIPTS
{
    public enum isoDirection
    {
        NE,
        SE,
        SW,
        NW,
        none
    }
    [CreateAssetMenu(menuName = "My Assets/HouseAssets")]
    public class HouseAssets : ScriptableObject
    {
        
        public GameObject WallPrefab;


    }
}