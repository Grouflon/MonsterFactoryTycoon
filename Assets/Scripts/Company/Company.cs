using UnityEngine;
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

    public delegate void MonsterAction(Monster _monster);
    public event MonsterAction OnMonsterHired;
    public event MonsterAction OnMonsterHealed;

    public delegate void MissionInstanceAction(MissionInstance _mission);
    public event MissionInstanceAction OnMissionStarted;
    public event MissionInstanceAction OnMissionEnded;

    public void SetPause(bool _value)
    {
        m_pause = _value;
    }

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
        m_objects.Add(room);

        if (OnRoomBuilt != null)
            OnRoomBuilt(room);

        Logger.Log("Built \"" + room.GetName() + "\" for " + roomCost + "$");
    }

    public void HireMonster()
    {
        Monster monster = new Monster();
        monster.SetMaxStrength(Random.Range(m_balance.minStrength, m_balance.maxStrength));

        m_objects.Add(monster);
        m_monsters.Add(monster);

        if (OnMonsterHired != null)
            OnMonsterHired(monster);

        Logger.Log("Hired new monster \"" + monster.GetName() + "\".");
    }

    public void NotifyMonsterHealed(Monster _monster)
    {
        if (_monster.GetCurrentStrength() == _monster.GetMaxStrength())
            Logger.Log("Monster \"" + _monster.GetName() + "\" has recovered from its injuries.");

        if (OnMonsterHealed != null)
            OnMonsterHealed(_monster);
    }

    public void StartMission(Mission _mission, List<Monster> _monsters)
    {
        if (m_currentMission != null)
        {
            Debug.LogError("Can't start two missions at the same time.");
            return;
        }

        m_currentMission = new MissionInstance();
        m_currentMission.Start(_mission, _monsters);

        if (OnMissionStarted != null)
            OnMissionStarted(m_currentMission);

        SetPause(true);

        Logger.Log("Started mission.");
    }

    public void EndCurrentMission()
    {
        if (m_currentMission == null)
        {
            Debug.LogError("Can't end mission, there is no mission.");
            return;
        }

        if (m_currentMission.GetStatus() == MissionInstance.Status.Success)
        {
            AddMoney(m_currentMission.GetMission().reward);
            Logger.Log("Succesfully completed mission: gained " + m_currentMission.GetMission().reward + "$.");
        }
        else
        {
            Logger.Log("Mission Failed");
        }

        if (OnMissionEnded != null)
            OnMissionEnded(m_currentMission);

        SetPause(false);

        m_currentMission = null;
    }

    public IList<Staff> GetStaff()
    {
        return m_staff.AsReadOnly();
    }

    public IList<Monster> GetMonsters()
    {
        return m_monsters.AsReadOnly();
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
        return m_time;
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
        m_objectsWorkingList = new List<CompanyObject>();
        m_staff = new List<Staff>();
        m_monsters = new List<Monster>();

        m_rooms = new Room[12];
    }

    void Awake()
	{
        m_balance = Balance.Instance();

        m_money = m_balance.startingMoney;
	}
	
	void Update()
	{
        if (m_currentMission != null && (m_currentMission.GetStatus() == MissionInstance.Status.Failure || m_currentMission.GetStatus() == MissionInstance.Status.Success))
            EndCurrentMission();

        float deltaTime = Time.deltaTime * timeRatio;

        if (m_pause)
            deltaTime = 0.0f;

        float cycleDeltaTime = deltaTime / m_balance.cycleDuration;

        m_time += cycleDeltaTime;
        m_timer += cycleDeltaTime;

        m_objectsWorkingList.Clear();
        m_objectsWorkingList.AddRange(m_objects);
        foreach (CompanyObject obj in m_objectsWorkingList)
        {
            obj.Update(cycleDeltaTime);
        }

        while (m_timer > 1.0f)
        {
            m_timer -= 1.0f;
            foreach (CompanyObject obj in m_objectsWorkingList)
            {
                obj.OnNewCycle();
            }
        }
    }

    int m_money;

    Balance m_balance;

    List<CompanyObject> m_objects;
    List<CompanyObject> m_objectsWorkingList;
    List<Staff> m_staff;
    List<Monster> m_monsters;
    Room[] m_rooms;

    MissionInstance m_currentMission;

    float m_timer = 0.0f;
    float m_time = 0.0f;
    bool m_pause = false;
}
