using UnityEngine;
using System.Collections;

public class Portal : Room
{
	public Portal(int _position) : base(RoomType.Portal, Balance.Instance().maxPortalStaff, _position)
    {
        m_balance = Balance.Instance();
        m_company = Company.Instance();
    }

    public override void Update(float _dt)
    {
        base.Update(_dt);

        float generationTime = Mathf.Max(0.0f, m_balance.baseMonsterGenerationTime - (GetStaffCount() * m_balance.staffTimeDiscount));

        m_timer += _dt;

        while (m_timer >= generationTime)
        {
            Logger.Log("Our portal \"" + GetName() + "\" has summoned a new monster.");
            m_timer -= generationTime;
            m_company.HireMonster();
        }
    }

    private Company m_company;
    private Balance m_balance;
    private float m_timer;
}
