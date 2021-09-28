﻿using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class GalaxyGenerationTests
    {
        private GalaxyGenerator generator;
        private GameObject testObject;

        [SetUp]
        public void SetUp()
        {
            SceneManager.LoadScene("0_LOAD", LoadSceneMode.Single);
            GameObject.Find("INSERT SCENE NAME HERE").GetComponent<NetworkConnectionManager>().sceneLoad = "GalaxyGenerationScene";
        }

        [UnityTest]
        [Description("Number of planets generated by the generator is correct")]
        public IEnumerator CorrectNumberOfGeneratedPlanets()
        {
            generator = GameObject.Find("GameController").GetComponent<GalaxyGenerator>();
            int count = 0;
            foreach (Planet planet in generator.CurrentPlanets)
            {
                count++;
            }
            Debug.Log("Correct number to generate = " + generator.numberOfPlanets + "\nNumber of planets generated = " + count);
            Assert.AreEqual(count, generator.CurrentPlanets.Count);
            Assert.AreEqual(generator.numberOfPlanets, generator.CurrentPlanets.Count);
            yield return null;
        }

        [UnityTest]
        [Description("Number of suns generated by the generator is correct")]
        public IEnumerator CorrectNumberOfGeneratedSuns()
        {
            generator = GameObject.Find("GameController").GetComponent<GalaxyGenerator>();
            int count = 0;
            foreach (Sun sun in generator.CurrentSuns)
            {
                count++;
            }
            Debug.Log("Correct number to generate = " + generator.numberOfSuns + "\nNumber of suns generated = " + count);
            Assert.AreEqual(count, generator.CurrentSuns.Count);
            Assert.AreEqual(generator.numberOfSuns, generator.CurrentSuns.Count);
            yield return null;
        }

        [UnityTest]
        [Description("Each planet is spaced correctly from each other planet above their minimum distance")]
        public IEnumerator CorrectPlanetPlanetMinDistance()
        {
            generator = GameObject.Find("GameController").GetComponent<GalaxyGenerator>();
            foreach (Planet planetA in generator.CurrentPlanets)
            {
                foreach (Planet planetB in generator.CurrentPlanets)
                {
                    if (planetA != planetB)
                    {
                        float distance = Vector3.Distance(planetA.transform.position, planetB.transform.position);
                        Debug.Log(planetA.name + " is " + distance + " units from " + planetB.name
                            + " (Correct distance: " + generator.minPlanetPlanetDistf + ")");
                        Assert.GreaterOrEqual(distance, generator.minPlanetPlanetDistf);
                    }
                }
            }
            yield return null;
        }

        [UnityTest]
        [Description("Each planet is spaced correctly from each sun above their min/max distance")]
        public IEnumerator CorrectPlanetSunMinDistance()
        {
            generator = GameObject.Find("GameController").GetComponent<GalaxyGenerator>();
            foreach (Planet planet in generator.CurrentPlanets)
            {
                foreach (Sun sun in generator.CurrentSuns)
                {
                    float distance = Vector3.Distance(planet.transform.position, sun.transform.position);
                    Debug.Log(planet.name + " is " + distance + " units from " + sun.name
                        + " (Correct Distance: " + generator.minPlanetSunDistf + ")");
                    Assert.GreaterOrEqual(distance, generator.minPlanetSunDistf);
                }
            }
            yield return null;
        }

        [UnityTest]
        [Description("Each sun is spaced correctly from each other sun above their min distance")]
        public IEnumerator CorrectSunSunMinDistance()
        {
            generator = GameObject.Find("GameController").GetComponent<GalaxyGenerator>();
            foreach (Sun sunA in generator.CurrentSuns)
            {
                foreach (Sun sunB in generator.CurrentSuns)
                {
                    if (sunA != sunB)
                    {
                        float distance = Vector3.Distance(sunA.transform.position, sunB.transform.position);
                        Debug.Log(sunA.name + " is " + distance + " units from " + sunB.name
                           + " (Correct distance: " + generator.minSunSunDistf + ")");
                        Assert.GreaterOrEqual(distance, generator.minSunSunDistf);
                    }
                }
            }
            yield return null;
        }

        [UnityTest]
        [Description("Tests that each galaxy object is placed within the bounds of the galaxy")]
        public IEnumerator CorrectGalaxyPosition()
        {
            generator = GameObject.Find("GameController").GetComponent<GalaxyGenerator>();
            foreach (Sun sun in generator.CurrentSuns)
            {
                Vector3 sunPos = sun.transform.position;
                float sunX = sunPos.x;
                float sunZ = sunPos.z;
                Debug.Log("\nSun position = " + sunPos
                    + "\nDistance from positive x (" + generator.MaxGalaxyPosX + ") = " + Mathf.Abs(generator.MaxGalaxyPosX - sunX)
                    + "\nDistance from negative x (" + generator.MaxGalaxyNegX + ") = " + Mathf.Abs(generator.MaxGalaxyNegX - sunX)
                    + "\nDistance from positive z (" + generator.MaxGalaxyPosZ + ") = " + Mathf.Abs(generator.MaxGalaxyPosZ - sunZ)
                    + "\nDistance from negative z (" + generator.MaxGalaxyNegX + ") = " + Mathf.Abs(generator.MaxGalaxyNegZ - sunZ));
                Assert.LessOrEqual(sunPos.x, generator.MaxGalaxyPosX);
                Assert.GreaterOrEqual(sunPos.x, generator.MaxGalaxyNegX);
                Assert.LessOrEqual(sunPos.z, generator.MaxGalaxyPosZ);
                Assert.GreaterOrEqual(sunPos.z, generator.MaxGalaxyNegZ);
            }
            foreach (Planet planet in generator.CurrentPlanets)
            {
                Vector3 planetPos = planet.transform.position;
                float planetX = planetPos.x;
                float planetZ = planetPos.z;
                Debug.Log("\nPlanet position = " + planetPos
                    + "\nDistance from positive x (" + generator.MaxGalaxyPosX + ") = " + Mathf.Abs(generator.MaxGalaxyPosX - planetX)
                    + "\nDistance from negative x (" + generator.MaxGalaxyNegX + ") = " + Mathf.Abs(generator.MaxGalaxyNegX - planetX)
                    + "\nDistance from positive z (" + generator.MaxGalaxyPosZ + ") = " + Mathf.Abs(generator.MaxGalaxyPosZ - planetZ)
                    + "\nDistance from negative z (" + generator.MaxGalaxyNegX + ") = " + Mathf.Abs(generator.MaxGalaxyNegZ - planetZ));
                Assert.LessOrEqual(planetPos.x, generator.MaxGalaxyPosX);
                Assert.GreaterOrEqual(planetPos.x, generator.MaxGalaxyNegX);
                Assert.LessOrEqual(planetPos.z, generator.MaxGalaxyPosZ);
                Assert.GreaterOrEqual(planetPos.z, generator.MaxGalaxyNegZ);
            }
            yield return null;
        }

        [UnityTest]
        [Description("Tests that sizes generated are for the right number of galaxy objects")]
        public IEnumerator CorrectNumberOfSizesGenerated()
        {
            generator = GameObject.Find("GameController").GetComponent<GalaxyGenerator>();
            int total = 0;
            foreach (int size in generator.PlanetSizes)
            {
                total++;
            }
            foreach (int size in generator.SunSizes)
            {
                total++;
            }
            int correctSum = generator.numberOfPlanets + generator.numberOfSuns;
            Debug.Log("Correct number to generate = " + correctSum + "\nNumber of sizes generated = " + total);
            Assert.AreEqual(total, correctSum);
            yield return null;
        }

        [UnityTest]
        [Description("Tests that sizes generated are in the correct range")]
        public IEnumerator CorrectRangeOfSizesGenerated()
        {
            generator = GameObject.Find("GameController").GetComponent<GalaxyGenerator>();
            foreach (int size in generator.PlanetSizes)
            {
                Assert.LessOrEqual(size, generator.maxPlanetSize);
                Assert.GreaterOrEqual(size, generator.minPlanetSize);
            }
            foreach (int size in generator.SunSizes)
            {
                Assert.LessOrEqual(size, generator.maxSunSize);
                Assert.GreaterOrEqual(size, generator.minSunSize);
            }
            yield return null;
        }
    }
}