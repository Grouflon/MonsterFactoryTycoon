using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class UIWindow : MonoBehaviour
{
    public delegate void UIWindowAction(UIWindow _window);
    public event UIWindowAction OnWindowClosed;

    public Text titleText;
    public RectTransform contentTransform;
    public Button closeButton;

    public void SetTitle(string _value)
    {
        titleText.text = _value;
    }

    public string GetTitle()
    {
        return titleText.text;
    }

    public void SetContent(RectTransform _content)
    {
        if (_content == null)
            return;

        // CLEAR PREVIOUS CONTENT
        UnityTools.DestroyAllChildren(contentTransform);

        // APPLY NEW CONTENT
        _content.SetParent(contentTransform);
    }

    public RectTransform GetContentTransform()
    {
        return contentTransform;
    }

    public void BringToFront()
    {
        m_windowRectTransform.SetAsLastSibling();
    }

    void Awake()
    {
        UIWindow self = this;
        m_windowRectTransform = transform as RectTransform;
        closeButton.onClick.AddListener(() =>
        {
            if (OnWindowClosed != null)
                OnWindowClosed(self);

            UIWindowManager.Instance().DestroyWindow(self);
        });
    }

    private RectTransform m_windowRectTransform;
}
