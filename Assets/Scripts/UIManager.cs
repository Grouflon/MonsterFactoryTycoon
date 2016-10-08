using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Prefabs")]
    public Button listButtonPrefab;

    [Header("Misc")]
    public Text moneyText;
    public Text timeText;

    [Header("Staff")]
    public ScrollRect staffListScrollRect;
    public RectTransform staffWindowContentPrefab;

    public UIManager()
    {
        m_staffButtons = new Dictionary<Guid, Button>();
    }

	void Awake()
	{
        m_company = Company.Instance();

        m_company.OnStaffHired += OnStaffHired;
        m_company.OnStaffFired += OnStaffFired;
	}
	
	void Update()
	{
        moneyText.text = m_company.GetMoney().ToString() + " $";
        timeText.text = m_company.GetCurrentTime().ToString("0.0") + " Cycles";
	}

    void OnStaffHired(Staff _staff)
    {
        Button button = Instantiate(listButtonPrefab);
        Text buttonText = button.GetComponentInChildren<Text>();
        buttonText.text = _staff.GetName();

        button.onClick.AddListener(() => {

            UIWindow window = null;
            if (UIWindowManager.Instance().CreateWindow(_staff.GetName(), out window))
            {
                RectTransform content = Instantiate(staffWindowContentPrefab);
                window.SetContent(content);

                Button fireButton = content.Find("_fireButton").GetComponent<Button>();
                fireButton.onClick.AddListener(() => {
                    m_company.FireStaff(_staff);
                });
            }
        });

        (button.transform as RectTransform).SetParent(staffListScrollRect.content);
        m_staffButtons.Add(_staff.GetID(), button);
    }

    void OnStaffFired(Staff _staff)
    {
        Button button = null;
        if (m_staffButtons.TryGetValue(_staff.GetID(), out button))
        {
            UIWindowManager.Instance().DestroyWindow(_staff.GetName());

            Destroy(button.gameObject);
            m_staffButtons.Remove(_staff.GetID());
        }
    }

    private Dictionary<Guid,Button> m_staffButtons;
    private Company m_company;
}
