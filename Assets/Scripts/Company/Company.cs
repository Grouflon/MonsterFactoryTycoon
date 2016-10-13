using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Company : MonoBehaviour
{
    public float timeRatio = 1.0f;

    public delegate void StaffAction(Staff _staff);
    public event StaffAction OnStaffHired;
    public event StaffAction OnStaffFired;

    public delegate void RoomAction(Room _room);
    public event RoomAction OnRoomBuilt;

    public delegate void RoomAssignmentAction(Room _room, Staff _staff);
    public event RoomAssignmentAction OnStaffAssigned;
    public event RoomAssignmentAction OnStaffUnassigned;

    public void AddMoney(int _value)
    {
        m_money += _value;
    }

    public void HireStaff()
    {
        Staff staff = new Staff();

        AddMoney(-m_balance.staffHiringCost);

        m_objects.Add(staff);
        m_staff.Add(staff);

        if (OnStaffHired != null)
            OnStaffHired(staff);

        Logger.Log("Hired new staff \"" + staff.GetName() + "\" for " + m_balance.staffHiringCost + "$");
    }

    public void FireStaff(Staff _staff)
    {
        m_objects.Remove(_staff);
        m_staff.Remove(_staff);

        if (_staff.GetAssignment() != null)
        {
            UnassignStaff(_staff, _staff.GetAssignment());
        }

        if (OnStaffFired != null)
            OnStaffFired(_staff);

        Logger.Log("Fired staff \"" + _staff.GetName() + "\"");
    }

    public void AssignStaff(Staff _staff, Room _room)
    {
        if (_staff.GetAssignment() != null)
        {
            UnassignStaff(_staff, _staff.GetAssignment());
        }

        if (!_room.AssignStaff(_staff))
            return;

        _staff.SetAssignment(_room);

        if (OnStaffAssigned != null)
            OnStaffAssigned(_room, _staff);

        Logger.Log("\"" + _staff.GetName() + "\" Assigned to Room \"" + _room.GetName() + "\".");
    }

    public void UnassignStaff(Staff _staff, Room _room)
    {
        _room.UnassignStaff(_staff);
        _staff.SetAssignment(null);

        if (OnStaffUnassigned != null)
            OnStaffUnassigned(_room, _staff);

        Logger.Log("\"" + _staff.GetName() + "\" Unassigned from Room \"" + _room.GetName() + "\".");
    }

    public void BuildRoom(RoomType _type, int _position)
    {
        if (m_rooms[_position] != null)
        {
            Logger.Log("Can't build new room in slot " + _position + ": Already taken");
            return;
        }

        Room room = null;
        int roomCost = 0;

        switch(_type)
        {
            case RoomType.Den:
                {
                    room = new Den(_position);
                    roomCost = m_balance.denCost;
                }
                break;

            case RoomType.Portal:
                {
                    room = new Portal(_position);
                    roomCost = m_balance.portalCost;
                }
                break;

            default: break;
        }

        AddMoney(-roomCost);
        m_rooms[_position] = room;

        if (OnRoomBuilt != null)
            OnRoomBuilt(room);

        Logger.Log("Built \"" + room.GetName() + "\" for " + roomCost + "$");
    }

    public IList<Staff> GetStaff()
    {
        return m_staff.AsReadOnly();
    }

    public Room[] GetRooms()
    {
        return m_rooms;
    }

    public int GetMoney()
    {
        return m_money;
    }

    public float GetCurrentTime()
    {
        return (float)m_cycleCount + (m_timer / m_balance.cycleDuration);
    }



    public static Company Instance()
    {
        Company instance = FindObjectOfType<Company>();
        if (instance == null)
        {
            Debug.LogError("You need to have one instance of Company in the scene.");
        }
        return instance;
    }

    public Company()
    {
        m_objects = new List<CompanyObject>();
        m_staff = new List<Staff>();

        m_rooms = new Room[12];
    }

    void Awake()
	{
        m_balance = Balance.Instance();

        m_money = m_balance.startingMoney;
	}
	
	void Update()
	{
        float deltaTime = Time.deltaTime * timeRatio;

        foreach (CompanyObject obj in m_objects)
        {
            obj.Update(deltaTime);
        }

        m_timer += deltaTime;
        while (m_timer > m_balance.cycleDuration)
        {
            m_timer -= m_balance.cycleDuration;
            ++m_cycleCount;
            foreach (CompanyObject obj in m_objects)
            {
                obj.OnNewCycle();
            }
        }
    }

    int m_money;

    Balance m_balance;

    List<CompanyObject> m_objects;
    List<Staff> m_staff;
    Room[] m_rooms;

    float m_timer = 0.0f;
    int m_cycleCount = 0;
}
