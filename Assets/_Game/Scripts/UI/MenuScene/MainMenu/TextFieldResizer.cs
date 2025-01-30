using TMPro;
using UnityEngine;

public class TextFieldResizer : MonoBehaviour
{
    [SerializeField] private TMP_Text _textField;
    [SerializeField] private RectTransform _rectTransform;

    public void UpdateText(string newText)
    {
        _textField.text = newText;
        ResizeToFitText();
    }

    private void ResizeToFitText()
    {
        Vector2 newSize = new(_textField.preferredWidth, _textField.preferredHeight);
        _rectTransform.sizeDelta = newSize;
    }
}
