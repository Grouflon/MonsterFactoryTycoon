using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Logger : MonoBehaviour
{
    public ScrollRect logScrollRect;
    public Text logEntryPrefab;

    void Awake()
    {
        m_company = Company.Instance();
    }

    public static void Log(string _value)
    {
        Instance().AddLog(_value);
    }

    public void AddLog(string _value)
    {
        Text logEntry = Instantiate(logEntryPrefab);
        logEntry.text = "[" + m_company.GetCurrentTime().ToString("0.0") + "] " + _value;
        logEntry.transform.SetParent(logScrollRect.content);

        (logEntry.transform as RectTransform).SetAsFirstSibling();
    }

    public static Logger Instance()
    {
        Logger instance = FindObjectOfType<Logger>();
        if (instance == null)
        {
            Debug.LogError("You need to have one instance of UIWindowManager in the scene.");
        }
        return instance;
    }

    private Company m_company;
}
