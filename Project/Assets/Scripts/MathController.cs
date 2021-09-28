using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathController : MonoBehaviour
{
    //Operator delegate function, allows us to apply any arbitrary operator method to 2 float parameters
    public delegate float OperatorMethod(float v1, float v2);

    public float Operation(OperatorMethod op, float v1, float v2)
    {
        return op(v1, v2);
    }

    //Wrapper method that allows user to pass in their operator as a char
    public float OperationCharOp(char op, float v1, float v2)
    {
        return Operation(GetOperator(op), v1, v2);
    }

    //A 'Factory' method that returns a corresponding method instead of a class
    public OperatorMethod GetOperator(char op)
    {
        switch (op)
        {
            case ('+'): return Add;
            case ('*'): return Mult;
        }
        return null;
    }

    public float CalculateRadiusSphere(GameObject sphere)
    {
        //Should calculate the radius of the sphere
        float radius = 0;
        if (sphere != null)
        {
            radius = 0.5f * sphere.transform.localScale.x;
        }
        return radius;
    }

    public float CalculateVolumeSphere(GameObject sphere)
    {
        //Should calculate the volume of the sphere
        float volume = 4 / 3 * Mathf.PI * Mathf.Pow(CalculateRadiusSphere(sphere), 3);
        return volume;
    }

    public List<Vector3> CalculateSpherePoints(GameObject sphere)
    {
        //Points organised in NESW format
        List<Vector3> points = new List<Vector3>();
        float radius = CalculateRadiusSphere(sphere);
        Vector3 centralPos = sphere.transform.position;
        //North
        points.Add(new Vector3(centralPos.x, centralPos.y + radius, centralPos.z));
        //East
        points.Add(new Vector3(centralPos.x + radius, centralPos.y, centralPos.z));
        //South
        points.Add(new Vector3(centralPos.x, centralPos.y - radius, centralPos.z));
        //West
        points.Add(new Vector3(centralPos.x - radius, centralPos.y, centralPos.z));
        return points;
    }

    //Basic operations
    public float Mult(float v1, float v2)
    {
        return v1 * v2;
    }

    public float Add(float v1, float v2)
    {
        return v1 + v2;
    }

    public Unit CalculateCentralUnit(List<Unit> units)
    {
        if (units.Count > 0)
        {
            ////Item 2 is the Unit we want to compare distances for
            List<Tuple<Unit, Unit, float>> distances = new List<Tuple<Unit, Unit, float>>();
            foreach (Unit unit in units)
            {
                Tuple<Unit, Unit, float> shortestDistance = FindShortestDistance(unit, units);
                distances.Add(shortestDistance);
            }
            int highestCount = 0;
            Unit frequentUnit = units[0];
            foreach (Tuple<Unit, Unit, float> tpl in distances)
            {
                int temp = CalculateNumberOfOccurrences(tpl, distances);
                if (temp > highestCount)
                {
                    highestCount = temp;
                    frequentUnit = tpl.Item2;
                }
            }
            return frequentUnit;
        }
        return null;
    }

    //Finds shortest distance and unit with that shortest distane of each unit
    private Tuple<Unit, Unit, float> FindShortestDistance(Unit unit, List<Unit> unitList)
    {
        float unitDistance;
        float shortestDistance = 0;
        Unit shortestDistUnit = unit;
        foreach (Unit unitA in unitList)
        {
            if (unit != unitA)
            {
                unitDistance = Vector3.Distance(unit.transform.position, unitA.transform.position);
                //Initialise shortest distance to be greater for first iteration
                if (shortestDistance == 0) { shortestDistance = unitDistance + 1; }
                if (shortestDistance > unitDistance)
                {
                    shortestDistance = unitDistance;
                    shortestDistUnit = unitA;
                }
            }
        }
        //Add all shortest distances between each unit into a tuple
        Tuple<Unit, Unit, float> dist = new Tuple<Unit, Unit, float>(unit, shortestDistUnit, shortestDistance);
        return dist;
    }

    //Finds the number of occurrences of a unit in the closest unit tuple
    private int CalculateNumberOfOccurrences(Tuple<Unit, Unit, float> tpl, List<Tuple<Unit, Unit, float>> list)
    {
        int count = 0;
        foreach (Tuple<Unit, Unit, float> tuple in list)
        {
            if (tuple.Item2 == tpl.Item2)
            {
                count++;
            }
        }
        return count;
    }
}