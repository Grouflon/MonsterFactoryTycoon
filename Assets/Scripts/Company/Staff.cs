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

    // Do not call this method directly, this should only be called by Company
    public void SetAssignment(Room _assignment)
    {
        m_assignment = _assignment;
    }

    public Room GetAssignment()
    {
        return m_assignment;
    }

    public string GetName()
    {
        return m_name;
    }

    public override void OnNewCycle()
    {
        m_company.AddMoney(-m_balance.staffSalary);

        Logger.Log("Payed " + m_balance.staffSalary + "$ as staff salary");
    }

    private Room m_assignment;
    private string m_name;
    private Balance m_balance;
    private Company m_company;
}
