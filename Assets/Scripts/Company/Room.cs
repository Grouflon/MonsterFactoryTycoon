using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum RoomType
{
    Den,
    Portal
}

public class Room : CompanyObject
{
    public Room(RoomType _type, int _maxStaff, int _position)
    {
        m_type = _type;
        m_position = _position;
        m_staff = new Staff[_maxStaff];
        m_company = Company.Instance();

        if (_position >= Company.Instance().GetRooms().Length)
        {
            Debug.LogError("Invalid Room position");
        }
    }

    // Do not call directly, should only be called by Company
    public bool AssignStaff(Staff _staff)
    {
        if (_staff == null)
        {
            Debug.LogWarning("Assigning null staff");
            return false;
        }

        for (int i = 0; i < m_staff.Length; ++i)
        {
            if (_staff == m_staff[i])
            {
                Debug.LogWarning("Reassigning staff to the same assignment");
                return false;
            }
        }

        for (int i = 0; i < m_staff.Length; ++i)
        {
            if (m_staff[i] == null)
            {
                m_staff[i] = _staff;
                return true;
            }
        }

        Debug.LogWarning("Can't add more staff to room. Room already fully staffed");
        return false;
    }

    public void UnassignStaff(Staff _staff)
    {
        if (_staff == null)
        {
            Debug.LogWarning("Unassigning null staff");
            return;
        }

        for (int i = 0; i < m_staff.Length; ++i)
        {
            if (m_staff[i] == _staff)
            {
                m_staff[i] = null;
                return;
            }
        }

        Debug.LogWarning("Can't unassign staff from room: not found");
    }

    public virtual void FillWindowContent(UIWindow _window)
    {
        m_windowContent = Object.Instantiate(UIManager.Instance().roomWindowContentPrefab);
        _window.SetContent(m_windowContent);

        GenerateStaffButtons(m_windowContent);

        m_company.OnStaffHired += OnStaffChanged2;
        m_company.OnStaffFired += OnStaffChanged2;
        m_company.OnStaffAssigned += OnStaffChanged;
        m_company.OnStaffUnassigned += OnStaffChanged;
        _window.OnWindowClosed += OnWindowClosed;

    }

    void OnWindowClosed(UIWindow _window)
    {
        m_windowContent = null;

        m_company.OnStaffHired -= OnStaffChanged2;
        m_company.OnStaffFired -= OnStaffChanged2;
        m_company.OnStaffAssigned -= OnStaffChanged;
        m_company.OnStaffUnassigned -= OnStaffChanged;
        _window.OnWindowClosed -= OnWindowClosed;
    }

    void OnStaffChanged(Room _room, Staff _staff) 
    {
        GenerateStaffButtons(m_windowContent);
    }

    void OnStaffChanged2(Staff _staff)
    {
        GenerateStaffButtons(m_windowContent);
    }

    

    public int GetPosition()
    {
        return m_position;
    }

    public RoomType GetRoomType()
    {
        return m_type;
    }

    public Staff[] GetStaff() // be nice, do not write in this pliz
    {
        return m_staff;
    }

    public string GetName()
    {
        return m_type.ToString() + " " + m_position;
    }

    void GenerateStaffButtons(RectTransform _parent)
    {
        // AGGREGATE ASSIGNABLE STAFF
        List<Staff> assignableStaff = new List<Staff>();
        foreach (Staff staff in m_company.GetStaff())
        {
            bool isAssigned = false;
            for (int i = 0; i < m_staff.Length; ++i)
            {
                if (staff == m_staff[i])
                {
                    isAssigned = true;
                    break;
                }
            }

            if (isAssigned)
                continue;

            if (staff.GetAssignment() == null)
                assignableStaff.Insert(0, staff);
            else
                assignableStaff.Add(staff);
        }

        ScrollRect assignable = _parent.Find("_left/_assignable").GetComponent<ScrollRect>();
        Text assignableText = _parent.Find("_left/_assignableText").GetComponent<Text>();
        ScrollRect assigned = _parent.Find("_right/_assigned").GetComponent<ScrollRect>();
        Text assignedText = _parent.Find("_right/_assignedText").GetComponent<Text>();

        UnityTools.DestroyAllChildren(assignable.content);
        foreach (Staff staff in assignableStaff)
        {
            Button button = Object.Instantiate(UIManager.Instance().staffButtonPrefab);
            Text buttonText = button.GetComponentInChildren<Text>();

            buttonText.text = staff.GetName();

            if (staff.GetAssignment() == null)
            {
                buttonText.text += " (Idle)";
            }
            else
            {
                buttonText.text += " (In " + staff.GetAssignment().GetName() + ")";
            }

            Staff s = staff;
            button.onClick.AddListener(() =>
            {
                m_company.AssignStaff(s, this);
            });

            button.transform.SetParent(assignable.content);
        }
        assignableText.text = "Assignable (" + assignableStaff.Count + ")";

        UnityTools.DestroyAllChildren(assigned.content);
        int assignedCount = 0;
        for (int i = 0; i < m_staff.Length; ++i)
        {
            if (m_staff[i] == null)
                continue;

            ++assignedCount;

            Button button = Object.Instantiate(UIManager.Instance().staffButtonPrefab);
            Text buttonText = button.GetComponentInChildren<Text>();

            buttonText.text = m_staff[i].GetName();

            Staff s = m_staff[i];
            button.onClick.AddListener(() =>
            {
                m_company.UnassignStaff(s, this);
            });

            button.transform.SetParent(assigned.content);
        }
        assignedText.text = "Assigned (" + assignedCount + "/" + m_staff.Length + ")";
    }

    RectTransform m_windowContent;
    private int m_position;
    private RoomType m_type;
    private Staff[] m_staff;

    private Company m_company;
}
