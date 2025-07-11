#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    //[CreateAssetMenu(fileName = "Sample_Attribute_InLineEditor", menuName = "Gaskellgames/GgSamplePage")]
    public class Sample_Attribute_InLineEditor : ScriptableObject
    {
        // ---------- InLineEditor ----------

        [SerializeField, InLineEditor]
        private ScriptableObject inLineEditor;

        [field: SerializeField, InLineEditor]
        private ScriptableObject InLineEditor { get; set; }

    } // class end
}

#endif