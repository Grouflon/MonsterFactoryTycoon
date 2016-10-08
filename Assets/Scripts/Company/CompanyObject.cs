using UnityEngine;
using System.Collections;
using System;

public class CompanyObject
{
    public CompanyObject()
    {
        m_id = Guid.NewGuid();
    }

    public Guid GetID()
    {
        return m_id;
    }

    virtual public void Update(float _dt)
    {

    }

    virtual public void OnNewCycle()
    {

    }

    private Guid m_id;
}
