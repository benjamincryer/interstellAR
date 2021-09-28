using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//This is a utility class, called at the start of the game to generate each object in the game's Galaxy.
//Determines the locations of objects, which are then randomly assigned attributes by the PlanetGenerator script
public class GalaxyGenerator : MonoBehaviour
{
    public GameObject galaxy, galaxyBounds;
    public GameObject planetsParent;
    public MathController mathController;
    public static int scaleFactor = 10;

    public List<string> namesUsed = new List<string>();

    public int
        numberOfPlanets = 10,
        numberOfSuns = 1,
        positionAttemptLimit = 100;

    public float
        minPlanetSize = 1f,
        maxPlanetSize = 5f,
        minSunSize = 20f,
        maxSunSize = 40f,
        minPlanetSunDistf = 100f,
        minPlanetPlanetDistf = 100f,
        minSunSunDistf = 100f;

    //Keep the positive/negative maximum positions of the galaxy
    public List<Sun> CurrentSuns { get; set; }

    public List<Planet> CurrentPlanets { get; set; }
    public List<float> PlanetSizes { get; set; }
    public List<float> SunSizes { get; set; }
    public Vector3 MaxPlanetSunDist { get; set; }
    public float MaxGalaxyPosX { get; set; }
    public float MaxGalaxyNegX { get; set; }
    public float MaxGalaxyPosZ { get; set; }
    public float MaxGalaxyNegZ { get; set; }

    private void Update()
    {
        foreach (Planet planet in CurrentPlanets)
        {
            //Draws line to orbiting sun -- this is relative to the center of each sun atm
            //Debug.DrawLine(planet.transform.position, planet.OrbitObject.transform.position);
        }
    }

    public void Start()
    {
        CurrentSuns = new List<Sun>();
        CurrentPlanets = new List<Planet>();
        PlanetSizes = GenPlanetSizes();
        SunSizes = GenSunSizes();
        GenerateSuns();
        GeneratePlanets();
        //SetParents();
    }

    public void GeneratePlanets()
    {
        Vector3 position;
        PlanetGenerator planetGenerator;
        Planet newPlanet;
        Sun closestSun;
        //Ensure correct number of planet sizes has been calculated
        if (PlanetSizes.Count == numberOfPlanets)
        {
            for (int i = 0; i < numberOfPlanets; i++)
            {
                //Generate a galaxy position to instantiate the prefab
                position = FindPositionInGalaxyPlanet();
                newPlanet = PhotonNetwork.InstantiateSceneObject("Prefabs/SpaceObjects/Planet", position, Quaternion.identity).GetComponent<Planet>();

                //Generate planet type and get appropriate PlanetGenerator object
                string type = GenPlanetType();
                planetGenerator = PlanetGenFactory(type);

                //Orbit around closest sun
                closestSun = FindClosestSun(newPlanet.transform.position);
                newPlanet.OrbitObject = closestSun.gameObject;
                do
                {
                    newPlanet.transform.position = FindPositionFromSun(closestSun);
                    bool rotationTest = CalculateCorrectRotationPath(newPlanet);
                    bool minDistance = CheckMinDistancePlanet(newPlanet.transform.position, minPlanetPlanetDistf);
                    bool minDistanceSun = CheckMinDistanceSun(newPlanet.transform.position, minPlanetSunDistf);
                    if (rotationTest && minDistance && minDistanceSun) { break; }
                    else { IncreaseGalaxySize(1); }
                } while (true);

                newPlanet.name = "Planet (" + type + ")"; //show type of planet in editor

                //Set planet attributes based on results from its PlanetGenerator

                //(don't allow duplicates)
                bool unique = false;
                string newName = "error";

                while (!unique)
                {
                    newName = planetGenerator.GenName();
                    if (!namesUsed.Contains(newName))
                        unique = true;
                }

                namesUsed.Add(newName);
                newPlanet.PlanetName = newName;
                

                newPlanet.Health = planetGenerator.GenHealth();

                newPlanet.Metal = gameObject.AddComponent<UpgradeResource>();
                newPlanet.Energy = gameObject.AddComponent<UpkeepResource>();

                newPlanet.Energy.ResourceValue = planetGenerator.GenEnergy();
                newPlanet.Metal.ResourceValue = planetGenerator.GenMetal();

                newPlanet.PlanetMat = planetGenerator.MaterialName;
                newPlanet.UpdateColor(planetGenerator.Colour);
                newPlanet.SetEvents(planetGenerator.Events);
                newPlanet.transform.localScale *= PlanetSizes[i];

                //Set GPS origin coordinates
                GPSObject gps = newPlanet.GetComponent<GPSObject>();
                gps.latitude = GPS.latitude;
                gps.longitude = GPS.longitude;
                gps.altitude = 0;
                gps.UpdateOffset();
                gps.UpdateLocation();

                //Initialises all instances of the new planet with the values generated here across all clients, using an RPC
                newPlanet.InitRPC();
                
                CurrentPlanets.Add(newPlanet);
            }
        }
    }

    //Randomly generate Suns at random positions
    public void GenerateSuns()
    {
        //Ensure correct number of sun sizes has been calculated
        if (SunSizes.Count == numberOfSuns)
        {
            for (int i = 0; i < numberOfSuns; i++)
            {
                Vector3 sunPosition = FindPositionInGalaxySun();
                sunPosition += new Vector3(50, 0, 50);
                Sun newSun = PhotonNetwork.InstantiateSceneObject("Prefabs/SpaceObjects/Sun", sunPosition, Quaternion.identity).GetComponent<Sun>();
                newSun.transform.localScale *= SunSizes[i];
                newSun.OrbitObject = newSun.gameObject;
                CurrentSuns.Add(newSun);
            }
        }
    }

    //Randomly generates sizes for each planet
    public List<float> GenPlanetSizes()
    {
        List<float> sizes = new List<float>();
        for (int i = 0; i < numberOfPlanets; i++)
        {
            float size = Random.Range(minPlanetSize, maxPlanetSize);
            sizes.Add(size);
        }
        return sizes;
    }

    //Randomly generates sizes for each sun
    public List<float> GenSunSizes()
    {
        List<float> sizes = new List<float>();
        for (int i = 0; i < numberOfSuns; i++)
        {
            float size = Random.Range(minSunSize, maxSunSize);
            sizes.Add(size);
        }
        return sizes;
    }

    //Used to generate an entirely random Planet Type, and return its respective PlanetGenerator object
    private string GenPlanetType()
    {
        string[] types = new string[] { "green", "rocky", "desert", "ocean", "iron", "lava", "ice" };
        int[] weights = new int[] { 80, 65, 50, 50, 50, 15, 30 };
        int typeIndex = types.Length - 1; //set to return last index by default

        //Get the total sum of Event weights
        float weightSum = 0f;
        for (int i = 0; i < weights.Length; ++i)
            weightSum += weights[i];

        //Iterate through all possibilities and check if each is selected
        int index = 0;
        int lastIndex = types.Length - 1;
        while (index < lastIndex)
        {
            //Do a probability check with a likelihood of weights[index] / weightSum
            if (Random.Range(0, weightSum) < weights[index])
            {
                typeIndex = index;
                break;
            }
            //Remove the item's weight from the total weight sum and repeat with the rest
            weightSum -= weights[index];
            index++;
        }

        return types[typeIndex];
    }

    //Finds a general position in the allocated galaxy game space
    private Vector3 FindPositionInGalaxyPlanet()
    {
        float randomY = galaxyBounds.transform.position.y;
        float randomX, randomZ;
        Vector3 position;
        do
        {
            randomX = Random.Range(MaxGalaxyNegX, MaxGalaxyPosX);
            randomZ = Random.Range(MaxGalaxyNegZ, MaxGalaxyPosZ);
            position = new Vector3(randomX, randomY, randomZ);
            bool minDistTest = CheckMinDistancePlanet(position, minPlanetPlanetDistf);
            bool galaxyBounds = CheckInGalaxyBounds(position);
            if (minDistTest && galaxyBounds) { break; }
            else { IncreaseGalaxySize(1); }
        } while (true);
        return position;
    }

    //Finds a position of suns subject to the positions of other suns
    //Should be made to limit position, but for now its fine to have them generate randomly irrespective of other positions
    private Vector3 FindPositionInGalaxySun()
    {
        float randomY = galaxyBounds.transform.position.y;
        float randomX, randomZ;
        Vector3 position;
        do
        {
            randomX = Random.Range(MaxGalaxyNegX, MaxGalaxyPosX);
            randomZ = Random.Range(MaxGalaxyNegZ, MaxGalaxyPosZ);
            position = new Vector3(randomX, randomY, randomZ);
            bool minDistTest = CheckMinDistanceSun(position, minSunSunDistf);
            bool galaxyBounds = CheckInGalaxyBounds(position);
            if (minDistTest && galaxyBounds) { break; }
            else { IncreaseGalaxySize(1); }
        } while (true);
        return position;
    }

    //Positions a planet at a random position around a sun
    private Vector3 FindPositionFromSun(Sun sun)
    {
        Vector3 center = sun.transform.position;
        float angle = Random.value * 360;
        float multiX = Mathf.Sin(angle * Mathf.Deg2Rad);
        float multiZ = Mathf.Cos(angle * Mathf.Deg2Rad);
        float distanceFromCenterX = center.x + minPlanetSunDistf;
        float distanceFromCenterZ = center.z + minPlanetSunDistf;
        Vector3 positionMinRadius = new Vector3(distanceFromCenterX * multiX, center.y, distanceFromCenterZ * multiZ);
        float positionXMax;
        float positionZMax;
        if (positionMinRadius.x < 0)
        {
            positionXMax = MaxGalaxyNegX + Mathf.Abs(positionMinRadius.x);
        }
        else
        {
            positionXMax = MaxGalaxyPosX - positionMinRadius.x;
        }
        if (positionMinRadius.z < 0)
        {
            positionZMax = MaxGalaxyNegZ + Mathf.Abs(positionMinRadius.z);
        }
        else
        {
            positionZMax = MaxGalaxyPosZ - positionMinRadius.z;
        }
        float posX = Random.Range(positionMinRadius.x, positionXMax);
        float posZ = Random.Range(positionMinRadius.z, positionZMax);
        Vector3 pos = new Vector3(posX, center.y, posZ);
        return pos;
    }

    //Checks that a position is the min distance from all suns
    private bool CheckMinDistanceSun(Vector3 generatedPosition, float minDistance)
    {
        foreach (Sun sun in CurrentSuns)
        {
            if (Vector3.Distance(generatedPosition, sun.transform.position) < minDistance)
            {
                return false;
            }
        }
        return true;
    }

    //Checks that a position is the min distance from all planets
    private bool CheckMinDistancePlanet(Vector3 generatedPosition, float minDistance)
    {
        foreach (Planet planet in CurrentPlanets)
        {
            if (Vector3.Distance(generatedPosition, planet.transform.position) < minDistance)
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckInGalaxyBounds(Vector3 generatedPosition)
    {
        float posX = generatedPosition.x;
        float posZ = generatedPosition.z;
        if (posX >= MaxGalaxyNegX && posX <= MaxGalaxyPosX && posZ >= MaxGalaxyNegZ && posZ <= MaxGalaxyPosZ)
        {
            return true;
        }
        return false;
    }

    //Iterate through all suns to find a position's closest neighbouring sun
    private Sun FindClosestSun(Vector3 pos)
    {
        Sun closest = CurrentSuns[0];
        foreach (Sun sun in CurrentSuns)
        {
            //Account for size of the sun
            Vector3 sunPos = sun.transform.position;
            sunPos.x += sun.transform.localScale.x;
            sunPos.z += sun.transform.localScale.z;
            if (Vector3.Distance(sunPos, pos) < Vector3.Distance(closest.transform.position, pos))
            {
                closest = sun;
            }
        }
        return closest;
    }

    //Sets the galaxy size to accomodate for the planets and suns
    private void IncreaseGalaxySize(int scale)
    {
        galaxy.transform.position *= 0;
        galaxyBounds.transform.position = galaxy.transform.position;
        galaxyBounds.transform.localScale += new Vector3(scale, 0, scale);
        Vector3 center = galaxy.transform.position;
        Vector3 scaleGalaxy = galaxyBounds.transform.localScale;
        MaxGalaxyPosX = scaleFactor * (center.x + (scaleGalaxy.x / 2));
        MaxGalaxyNegX = scaleFactor * (center.x - (scaleGalaxy.x / 2));
        MaxGalaxyPosZ = scaleFactor * (center.z + (scaleGalaxy.z / 2));
        MaxGalaxyNegZ = scaleFactor * (center.z - (scaleGalaxy.z / 2));
    }

    //Sets parents for planets and suns
    private void SetParents()
    {
        foreach (Planet planet in CurrentPlanets)
        {
            planet.transform.parent = planetsParent.transform;
        }
        foreach (Sun sun in CurrentSuns)
        {
            sun.transform.parent = galaxy.transform;
        }
    }

    //Tests the orbit path of a planet to see if it remains within the galaxy
    private bool CalculateCorrectRotationPath(Planet planet)
    {
        planet.OrbitSpeed = 1000f;
        int count = 0;
        do
        {
            count++;
            planet.OrbitObject = FindClosestSun(planet.transform.position).gameObject;
            planet.Orbit();

            if (!CheckInGalaxyBounds(planet.transform.position))
            {
                planet.OrbitSpeed = HeavenlyObject.orbitConstant;
                return false;
            }
        } while (count < positionAttemptLimit);
        planet.OrbitSpeed = HeavenlyObject.orbitConstant;
        return true;
    }

    //Cube for testing purposes
    private void CreateTestCube(Vector3 position)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        cube.transform.localScale *= 3;
    }

    //PlanetGenerator Factory
    private PlanetGenerator PlanetGenFactory(string type)
    {
        switch (type)
        {
            case ("green"): return new GreenPlanetGenerator();
            case ("rocky"): return new RockyPlanetGenerator();
            case ("desert"): return new DesertPlanetGenerator();
            case ("ocean"): return new OceanPlanetGenerator();
            case ("iron"): return new IronPlanetGenerator();
            case ("lava"): return new LavaPlanetGenerator();
            case ("ice"): return new IcePlanetGenerator();
            default: return new GreenPlanetGenerator();
        }
    }
}