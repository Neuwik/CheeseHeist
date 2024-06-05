using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public enum ECheeseMassStats { None = 0, Molten = 1, Spicy = 2, Greasy = 3 }

[Serializable]
public class CheeseMass
{
    public float Mass = 1;
    public Vector3 Stats = Vector3.zero;
    public Vector3 NormalizedStats { get => Stats.normalized; }

    public CheeseMass() { }

    public CheeseMass(float mass, Vector3 stats)
    {
        SetStartCheeseMass(mass, stats);
    }

    public CheeseMass(CheeseMass other)
    {
        CopyOtherCheeseMass(other);
    }

    public void CopyOtherCheeseMass(CheeseMass other)
    {
        Mass = other.Mass;
        Stats = other.Stats;
    }

    public void GainMassWithSameStats(float amount)
    {
        float oldMass = Mass;
        Mass += amount;
        Stats *= (Mass / oldMass);
    }

    public void LooseMass(float amount)
    {
        float oldMass = Mass;
        Mass -= amount;
        Stats *= (Mass / oldMass);
    }

    public void SetStartCheeseMass(float mass, Vector3 stats)
    {
        Mass = mass;
        if (stats.magnitude > 1)
        {
            stats = stats.normalized;
        }
        Stats = stats * Mass;
    }

    public void MergeWithOtherCheeseMass(CheeseMass other)
    {
        Stats += other.Stats * (other.Mass / Mass);
        Mass += other.Mass;

        other.SetStartCheeseMass(0, Vector3.zero);
    }

    public void AddStat(ECheeseMassStats stat, float amount)
    {
        // Percent ?
        Stats += StatEnumToVector3(stat) * (amount / Mass);
    }

    private Vector3 StatEnumToVector3(ECheeseMassStats stat)
    {
        switch (stat)
        {
            case ECheeseMassStats.None:
                return Vector3.zero;
            case ECheeseMassStats.Molten:
                return Vector3.up;
            case ECheeseMassStats.Spicy:
                return Vector3.right;
            case ECheeseMassStats.Greasy:
                return Vector3.forward;
            default:
                return Vector3.zero;
        }
    }

    public float GetStatAmount(ECheeseMassStats stat)
    {
        Vector3 statAsV3 = StatEnumToVector3(stat);
        if (statAsV3.x > 0)
        {
            return Stats.x;
        }
        if (statAsV3.y > 0)
        {
            return Stats.y;
        }
        if (statAsV3.z > 0)
        {
            return Stats.z;
        }
        return 0;
    }
}
