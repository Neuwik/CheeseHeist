using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static CustomerPreferences;

public enum ECheeseMassStats { None = 0, Molten = 1, Spicy = 2, Greasy = 3 }

[Serializable]
public class CheeseMass
{
    public float Mass = 1;
    [SerializeField]
    private Vector3 _stats;
    public Vector3 Stats
    {
        get { return _stats; }
        set
        {
            float sum = value.x + value.y + value.z;
            if (sum > 1)
                value /= sum;
            _stats = value;
        }
    }

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
    }

    public void LooseMass(float amount)
    {
        Mass -= amount;
    }

    public void SetStartCheeseMass(float mass, Vector3 stats)
    {
        Mass = mass;
        Stats = stats;
    }

    public void MergeWithOtherCheeseMass(CheeseMass other)
    {
        float oldMass = Mass;
        Mass += other.Mass;
        Stats = (((Stats * oldMass) + (other.Stats) * other.Mass)) / Mass;

        other.SetStartCheeseMass(0, Vector3.zero);
    }

    public void AddStat(ECheeseMassStats stat, float percent)
    {
        Stats = Stats + (StatEnumToVector3(stat) * percent);
    }

    public static Vector3 StatEnumToVector3(ECheeseMassStats stat)
    {
        switch (stat)
        {
            case ECheeseMassStats.None:
                return Vector3.zero;
            case ECheeseMassStats.Molten:
                return Vector3.right;
            case ECheeseMassStats.Spicy:
                return Vector3.up;
            case ECheeseMassStats.Greasy:
                return Vector3.forward;
            default:
                return Vector3.zero;
        }
    }

    public float GetStatPercentAmount(ECheeseMassStats stat)
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

    public override string ToString()
    {
        return $"Mass: {Mathf.RoundToInt(10*Mass)/10}dag, Spicy: {Mathf.RoundToInt(100 * GetStatPercentAmount(ECheeseMassStats.Spicy))}%, Molten: {Mathf.RoundToInt(100 * GetStatPercentAmount(ECheeseMassStats.Molten))}%, Fat: {Mathf.RoundToInt(100 * GetStatPercentAmount(ECheeseMassStats.Greasy))}%";
    }

    public float StatsMatch(CheeseMass other)
    {
        return 1 - Vector3.Distance(Stats, other.Stats);
    }

}
