using TMPro;
using UnityEngine;

namespace GangstaBean.Utilities
{
    [ExecuteInEditMode]
    public class CopyText : MonoBehaviour
    {
        public TextMeshProUGUI TextToCopy;
        private TextMeshProUGUI _text;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
            if(_text != null && TextToCopy != null)
            {
                _text.text = TextToCopy.text;
            }
        }
    }
}
