using UnityEngine;
using System.Collections;

public class Staff : CompanyObject
{
    static int staffCount = 1;

    public Staff()
    {
        m_name = "Staff " + staffCount;
        ++staffCount;

        m_balance = Balance.Instance();
        m_company = Company.Instance();
    }

    public string GetName()
    {
        return m_name;
    }

    public override void OnNewCycle()
    {
        // TODO: Add Log
        m_company.AddMoney(-m_balance.staffSalary);
    }

    private string m_name;
    private Balance m_balance;
    private Company m_company;
}
