using System;
using System.Collections.Generic;
using Debug = System.Diagnostics.Debug;

public enum Force
{
    Red,
    Blue,
}

public class WarriorComparer : IComparer<Warrior>
{
    private readonly int _round;

    public WarriorComparer(int round)
    {
        _round = round;
    }

    public int Compare(Warrior a, Warrior b)
    {
        Debug.Assert(a != null, nameof(a) + " != null");
        Debug.Assert(b != null, nameof(b) + " != null");

        var k1 = a.Coefficient;
        var k2 = b.Coefficient;

        {
            // в чётном раунде преимущество у синих
            if (a._team == Force.Blue && _round % 2 == 0)
            {
                k1 += 10;
            }

            if (b._team == Force.Blue && _round % 2 == 0)
            {
                k2 += 10;
            }
        }

        {
            // в нечётном раунде преимущество у красных
            if (a._team == Force.Red && _round % 2 != 0)
            {
                k1 += 10;
            }

            if (b._team == Force.Red && _round % 2 != 0)
            {
                k2 += 10;
            }
        }

        return k1.CompareTo(k2) * -1;
    }
}

[Serializable]
public class WarriorData
{
    public List<Warrior> _warriors;

    // test data
    public List<Warrior> _first;
    public List<Warrior> _second;
}

[Serializable]
public class Warrior
{
    public Force _team;
    public int _initiative;
    public int _speed;
    public int _number;

    public int Coefficient => _initiative * 1000 + _speed * 100 + (10 - _number);

    public bool IsSame(Warrior elem)
    {
        return _team == elem._team &&
               _initiative == elem._initiative &&
               _speed == elem._speed &&
               _number == elem._number;
    }

    public Warrior(Force team, int initiative, int speed, int number)
    {
        _team = team;
        _speed = speed;
        _number = number;
        _initiative = initiative;
    }
}