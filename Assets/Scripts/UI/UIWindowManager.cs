using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIWindowManager : MonoBehaviour
{
    public RectTransform canvasTransform;
    public UIWindow UIWindowPrefab;

    public UIWindowManager()
    {
        m_windows = new Dictionary<string, UIWindow>();
    }

    public bool CreateWindow(string _name, out UIWindow _window)
    {
        bool created = false;
        if (!m_windows.TryGetValue(_name, out _window))
        {
            created = true;
            _window = Instantiate(UIWindowPrefab);
            RectTransform windowTransform = (_window.transform as RectTransform);
            windowTransform.SetParent(canvasTransform);
            windowTransform.anchoredPosition = new Vector3(0.0f, 0.0f, 0.0f);
            _window.SetTitle(_name);

            m_windows.Add(_name, _window);
        }

        _window.BringToFront();

        return created;
    }

    public void DestroyWindow(UIWindow _window)
    {
        if (_window == null)
            return;

        if (!m_windows.Remove(_window.GetTitle()))
        {
            Debug.LogError("Failed to remove window \"" + _window.GetTitle() + "\".");
        }

        Destroy(_window.gameObject);
    }

    public void DestroyWindow(string _key)
    {
        UIWindow window;
        if (m_windows.TryGetValue(_key, out window))
        {
            DestroyWindow(window);
        }
    }

    public static UIWindowManager Instance()
    {
        UIWindowManager instance = FindObjectOfType<UIWindowManager>();
        if (instance == null)
        {
            Debug.LogError("You need to have one instance of UIWindowManager in the scene.");
        }
        return instance;
    }

    private Dictionary<string, UIWindow> m_windows;
}
