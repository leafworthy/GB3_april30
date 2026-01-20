using TMPro;
using UnityEngine;

namespace __SCRIPTS
{
    [ExecuteInEditMode]
    public class CopyText : MonoBehaviour
    {
        public TextMeshProUGUI TextToCopy;
        TextMeshProUGUI _text;
        void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        void Update()
        {
            if(_text != null && TextToCopy != null)
            {
                _text.text = TextToCopy.text;
            }
        }
    }
}
