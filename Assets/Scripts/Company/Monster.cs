using UnityEngine;
using System.Collections;

public class Monster : CompanyObject
{
    static int monsterCount = 1;

    public Monster()
    {
        m_name = "Monster " + monsterCount;
        ++monsterCount;
    }

    public string GetName()
    {
        return m_name;
    }

    public void SetStrength(int _value)
    {
        m_strength = _value;
    }

    public int GetStrength()
    {
        return m_strength;
    }

    private string m_name;
    private int m_strength;
}
