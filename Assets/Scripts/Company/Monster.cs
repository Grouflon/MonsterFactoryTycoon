using UnityEngine;
using System.Collections;

public class Monster : CompanyObject
{
    static int monsterCount = 1;

    public Monster()
    {
        m_balance = Balance.Instance();
        m_company = Company.Instance();
        m_name = "Monster " + monsterCount;
        ++monsterCount;
    }

    public string GetName()
    {
        return m_name;
    }

    public void Heal()
    {
        m_currentStrength = m_maxStrength;
    }

    public void Damage(int _value)
    {
        m_currentStrength = Mathf.Clamp(m_currentStrength - _value, 0, m_maxStrength);
    }

    public void SetMaxStrength(int _value)
    {
        m_maxStrength = _value;
        Heal();
    }

    public int GetMaxStrength()
    {
        return m_maxStrength;
    }

    public int GetCurrentStrength()
    {
        return m_currentStrength;
    }

    public override void Update(float _dt)
    {
        if (m_currentStrength < m_maxStrength)
        {
            m_recoverTimer += _dt;

            while (m_recoverTimer > m_balance.monsterRecoverTime)
            {
                m_recoverTimer -= m_balance.monsterRecoverTime;
                m_currentStrength = Mathf.Min(m_currentStrength + 1, m_maxStrength);
                m_company.NotifyMonsterHealed(this);
            }
        }
        else
        {
            m_recoverTimer = 0.0f;
        }
    }

    private Balance m_balance;
    private Company m_company;
    private string m_name;
    private int m_maxStrength;
    private int m_currentStrength;
    private float m_recoverTimer;
}
