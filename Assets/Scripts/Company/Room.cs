using UnityEngine;
using System.Collections;

public enum RoomType
{
    Den,
    Portal
}

public class Room : CompanyObject
{
    public Room(RoomType _type, int _position)
    {
        m_type = _type;
        m_position = _position;

        if (_position >= Company.Instance().GetRooms().Length)
        {
            Debug.LogError("Invalid Room position");
        }
    }

    public int GetPosition()
    {
        return m_position;
    }

    public RoomType GetRoomType()
    {
        return m_type;
    }

    public string GetName()
    {
        return m_type.ToString() + " " + m_position;
    }

    private int m_position;
    private RoomType m_type;
}
