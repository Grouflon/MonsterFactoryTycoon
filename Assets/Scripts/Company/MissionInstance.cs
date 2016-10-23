using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionInstance
{
    public enum Status
    {
        Progress,
        Success,
        Failure
    }

    public MissionInstance()
    {
        m_monsters = new List<Monster>();
    }

    public void Start(Mission _mission, List<Monster> _monsters)
    {
        m_monsters.Clear();
        m_monsters.InsertRange(0, _monsters);
        m_mission = _mission;
        m_currentObstacle = 0;
        m_currentObstacleStrength = _mission.obstacles[0].strength;
    }

    public void CommitMonster(Monster _monster)
    {
        if (m_monsters.Find(delegate (Monster m) { return m == _monster; }) == null)
        {
            Debug.LogError("Unknown monster, WTF?");
            return;
        }

        int monsterStrength = _monster.GetCurrentStrength();
        _monster.Damage(m_currentObstacleStrength);
        m_currentObstacleStrength = Mathf.Max(0, m_currentObstacleStrength - monsterStrength);

        if (m_currentObstacleStrength == 0)
        {
            GoToNextObstacle();
        }

        if (m_status != Status.Success)
        {
            bool over = true;
            foreach (Monster monster in m_monsters)
            {
                if (monster.GetCurrentStrength() > 0)
                {
                    over = false;
                    break;
                }
            }

            if (over)
                m_status = Status.Failure;
        }
    }

    public Mission GetMission()
    {
        return m_mission;
    }

    public int GetCurrentObstacle()
    {
        return m_currentObstacle;
    }

    public int GetCurrentObstacleStrength()
    {
        return m_currentObstacleStrength;
    }

    public Status GetStatus()
    {
        return m_status;
    }

    public IList<Monster> GetMonsters()
    {
        return m_monsters.AsReadOnly();
    }

    void GoToNextObstacle()
    {
        ++m_currentObstacle;
        if (m_currentObstacle >= m_mission.obstacles.Length)
        {
            m_status = Status.Success;
        }
        else
        {
            m_currentObstacleStrength = m_mission.obstacles[m_currentObstacle].strength;
        }
    }

    Status m_status = Status.Progress;
    int m_currentObstacle = 0;
    int m_currentObstacleStrength;
    List<Monster> m_monsters;
    Mission m_mission;
}
