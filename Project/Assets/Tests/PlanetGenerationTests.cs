using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class PlanetGenerationTests
    {
        public Planet planetPrefab;
        private Planet testPlanet;
        private DesertPlanetGenerator dpGenerator;
        private IcePlanetGenerator ipGenerator;
        private IronPlanetGenerator irpGenerator;
        private GreenPlanetGenerator gpGenerator;
        private OceanPlanetGenerator opGenerator;
        private LavaPlanetGenerator lpGenerator;
        private RockyPlanetGenerator rpGenerator;

        public void SetUpPlanet(Planet planet, PlanetGenerator generator)
        {
            planet.Health = generator.GenHealth();
            planet.Metal.ResourceValue = generator.GenMetal();
            planet.Food.ResourceValue = generator.GenFood();
            planet.Energy.ResourceValue = generator.GenEnergy();
            planet.PlanetMat = generator.MaterialName;
            planet.UpdateColor(generator.Colour);
        }

        [SetUp]
        public void SetUp()
        {
            planetPrefab = Resources.Load<Planet>("Prefabs/SpaceObjects/Planet");
            dpGenerator = new DesertPlanetGenerator();
            ipGenerator = new IcePlanetGenerator();
            irpGenerator = new IronPlanetGenerator();
            gpGenerator = new GreenPlanetGenerator();
            opGenerator = new OceanPlanetGenerator();
            lpGenerator = new LavaPlanetGenerator();
            rpGenerator = new RockyPlanetGenerator();
            testPlanet = Object.Instantiate(planetPrefab);
        }

        [UnityTest]
        [Description("Desert planet Health is specified correctly in its min/max range")]
        public IEnumerator CorrectHealthInRangeDesert()
        {
            PlanetGenerator pg = dpGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Health, pg.MAX_HEALTH);
            Assert.GreaterOrEqual(testPlanet.Health, pg.MIN_HEALTH);
            Debug.Log("Desert Planet Health is " + testPlanet.Health + "\nCorrect Range: " + pg.MIN_HEALTH + "-" + pg.MAX_HEALTH);
            yield return null;
        }

        [UnityTest]
        [Description("Ice planet Health is specified correctly in its min/max range")]
        public IEnumerator CorrectHealthInRangeIce()
        {
            PlanetGenerator pg = ipGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Health, pg.MAX_HEALTH);
            Assert.GreaterOrEqual(testPlanet.Health, pg.MIN_HEALTH);
            Debug.Log("Ice Planet Health is " + testPlanet.Health + "\nCorrect Range: " + pg.MIN_HEALTH + "-" + pg.MAX_HEALTH);
            yield return null;
        }

        [UnityTest]
        [Description("Iron planet Health is specified correctly in its min/max range")]
        public IEnumerator CorrectHealthInRangeIron()
        {
            PlanetGenerator pg = irpGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Health, pg.MAX_HEALTH);
            Assert.GreaterOrEqual(testPlanet.Health, pg.MIN_HEALTH);
            Debug.Log("Iron Planet Health is " + testPlanet.Health + "\nCorrect Range: " + pg.MIN_HEALTH + "-" + pg.MAX_HEALTH);
            yield return null;
        }

        [UnityTest]
        [Description("Green planet Health is specified correctly in its min/max range")]
        public IEnumerator CorrectHealthInRangeGreen()
        {
            PlanetGenerator pg = gpGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Health, pg.MAX_HEALTH);
            Assert.GreaterOrEqual(testPlanet.Health, pg.MIN_HEALTH);
            Debug.Log("Green Planet Health is " + testPlanet.Health + "\nCorrect Range: " + pg.MIN_HEALTH + "-" + pg.MAX_HEALTH);
            yield return null;
        }

        [UnityTest]
        [Description("Ocean planet Health is specified correctly in its min/max range")]
        public IEnumerator CorrectHealthInRangeOcean()
        {
            PlanetGenerator pg = opGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Health, pg.MAX_HEALTH);
            Assert.GreaterOrEqual(testPlanet.Health, pg.MIN_HEALTH);
            Debug.Log("Ocean Planet Health is " + testPlanet.Health + "\nCorrect Range: " + pg.MIN_HEALTH + "-" + pg.MAX_HEALTH);
            yield return null;
        }

        [UnityTest]
        [Description("Lava planet Health is specified correctly in its min/max range")]
        public IEnumerator CorrectHealthInRangeLava()
        {
            PlanetGenerator pg = lpGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Health, pg.MAX_HEALTH);
            Assert.GreaterOrEqual(testPlanet.Health, pg.MIN_HEALTH);
            Debug.Log("Lava Planet Health is " + testPlanet.Health + "\nCorrect Range: " + pg.MIN_HEALTH + "-" + pg.MAX_HEALTH);
            yield return null;
        }

        [UnityTest]
        [Description("Rocky planet Health is specified correctly in its min/max range")]
        public IEnumerator CorrectHealthInRangeRocky()
        {
            PlanetGenerator pg = rpGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Health, pg.MAX_HEALTH);
            Assert.GreaterOrEqual(testPlanet.Health, pg.MIN_HEALTH);
            Debug.Log("Rocky Planet Health is " + testPlanet.Health + "\nCorrect Range: " + pg.MIN_HEALTH + "-" + pg.MAX_HEALTH);
            yield return null;
        }

        [UnityTest]
        [Description("Desert planet Metal is specified correctly in its min/max range")]
        public IEnumerator CorrectMetalInRangeDesert()
        {
            PlanetGenerator pg = dpGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Metal.ResourceValue, pg.MAX_METAL);
            Assert.GreaterOrEqual(testPlanet.Metal.ResourceValue, pg.MIN_METAL);
            Debug.Log("Desert Planet Metal is " + testPlanet.Metal + "\nCorrect Range: " + pg.MIN_METAL + "-" + pg.MAX_METAL);
            yield return null;
        }

        [UnityTest]
        [Description("Ice planet Metal is specified correctly in its min/max range")]
        public IEnumerator CorrectMetalInRangeIce()
        {
            PlanetGenerator pg = ipGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Metal.ResourceValue, pg.MAX_METAL);
            Assert.GreaterOrEqual(testPlanet.Metal.ResourceValue, pg.MIN_METAL);
            Debug.Log("Ice Planet Metal is " + testPlanet.Metal + "\nCorrect Range: " + pg.MIN_METAL + "-" + pg.MAX_METAL);
            yield return null;
        }

        [UnityTest]
        [Description("Iron planet Metal is specified correctly in its min/max range")]
        public IEnumerator CorrectMetalInRangeIron()
        {
            PlanetGenerator pg = irpGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Metal.ResourceValue, pg.MAX_METAL);
            Assert.GreaterOrEqual(testPlanet.Metal.ResourceValue, pg.MIN_METAL);
            Debug.Log("Iron Planet Metal is " + testPlanet.Metal + "\nCorrect Range: " + pg.MIN_METAL + "-" + pg.MAX_METAL);
            yield return null;
        }

        [UnityTest]
        [Description("Green planet Metal is specified correctly in its min/max range")]
        public IEnumerator CorrectMetalInRangeGreen()
        {
            PlanetGenerator pg = gpGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Metal.ResourceValue, pg.MAX_METAL);
            Assert.GreaterOrEqual(testPlanet.Metal.ResourceValue, pg.MIN_METAL);
            Debug.Log("Green Planet Metal is " + testPlanet.Metal + "\nCorrect Range: " + pg.MIN_METAL + "-" + pg.MAX_METAL);
            yield return null;
        }

        [UnityTest]
        [Description("Ocean planet Metal is specified correctly in its min/max range")]
        public IEnumerator CorrectMetalInRangeOcean()
        {
            PlanetGenerator pg = opGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Metal.ResourceValue, pg.MAX_METAL);
            Assert.GreaterOrEqual(testPlanet.Metal.ResourceValue, pg.MIN_METAL);
            Debug.Log("Ocean Planet Metal is " + testPlanet.Metal + "\nCorrect Range: " + pg.MIN_METAL + "-" + pg.MAX_METAL);
            yield return null;
        }

        [UnityTest]
        [Description("Lava planet Metal is specified correctly in its min/max range")]
        public IEnumerator CorrectMetalInRangeLava()
        {
            PlanetGenerator pg = lpGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Metal.ResourceValue, pg.MAX_METAL);
            Assert.GreaterOrEqual(testPlanet.Metal.ResourceValue, pg.MIN_METAL);
            Debug.Log("Lava Planet Metal is " + testPlanet.Metal + "\nCorrect Range: " + pg.MIN_METAL + "-" + pg.MAX_METAL);
            yield return null;
        }

        [UnityTest]
        [Description("Rocky planet Metal is specified correctly in its min/max range")]
        public IEnumerator CorrectMetalInRangeRocky()
        {
            PlanetGenerator pg = rpGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Metal.ResourceValue, pg.MAX_METAL);
            Assert.GreaterOrEqual(testPlanet.Metal.ResourceValue, pg.MIN_METAL);
            Debug.Log("Rocky Planet Metal is " + testPlanet.Metal + "\nCorrect Range: " + pg.MIN_METAL + "-" + pg.MAX_METAL);
            yield return null;
        }

        [UnityTest]
        [Description("Desert planet Food is specified correctly in its min/max range")]
        public IEnumerator CorrectFoodInRangeDesert()
        {
            PlanetGenerator pg = dpGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Food.ResourceValue, pg.MAX_FOOD);
            Assert.GreaterOrEqual(testPlanet.Food.ResourceValue, pg.MIN_FOOD);
            Debug.Log("Desert Planet Food is " + testPlanet.Food + "\nCorrect Range: " + pg.MIN_FOOD + "-" + pg.MAX_FOOD);
            yield return null;
        }

        [UnityTest]
        [Description("Ice planet Food is specified correctly in its min/max range")]
        public IEnumerator CorrectFoodInRangeIce()
        {
            PlanetGenerator pg = ipGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Food.ResourceValue, pg.MAX_FOOD);
            Assert.GreaterOrEqual(testPlanet.Food.ResourceValue, pg.MIN_FOOD);
            Debug.Log("Ice Planet Food is " + testPlanet.Food + "\nCorrect Range: " + pg.MIN_FOOD + "-" + pg.MAX_FOOD);
            yield return null;
        }

        [UnityTest]
        [Description("Iron planet Food is specified correctly in its min/max range")]
        public IEnumerator CorrectFoodInRangeIron()
        {
            PlanetGenerator pg = irpGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Food.ResourceValue, pg.MAX_FOOD);
            Assert.GreaterOrEqual(testPlanet.Food.ResourceValue, pg.MIN_FOOD);
            Debug.Log("Iron Planet Food is " + testPlanet.Food + "\nCorrect Range: " + pg.MIN_FOOD + "-" + pg.MAX_FOOD);
            yield return null;
        }

        [UnityTest]
        [Description("Green planet Food is specified correctly in its min/max range")]
        public IEnumerator CorrectFoodInRangeGreen()
        {
            PlanetGenerator pg = gpGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Food.ResourceValue, pg.MAX_FOOD);
            Assert.GreaterOrEqual(testPlanet.Food.ResourceValue, pg.MIN_FOOD);
            Debug.Log("Green Planet Food is " + testPlanet.Food + "\nCorrect Range: " + pg.MIN_FOOD + "-" + pg.MAX_FOOD);
            yield return null;
        }

        [UnityTest]
        [Description("Ocean planet Food is specified correctly in its min/max range")]
        public IEnumerator CorrectFoodInRangeOcean()
        {
            PlanetGenerator pg = opGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Food.ResourceValue, pg.MAX_FOOD);
            Assert.GreaterOrEqual(testPlanet.Food.ResourceValue, pg.MIN_FOOD);
            Debug.Log("Ocean Planet Food is " + testPlanet.Food + "\nCorrect Range: " + pg.MIN_FOOD + "-" + pg.MAX_FOOD);
            yield return null;
        }

        [UnityTest]
        [Description("Lava planet Food is specified correctly in its min/max range")]
        public IEnumerator CorrectFoodInRangeLava()
        {
            PlanetGenerator pg = lpGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Food.ResourceValue, pg.MAX_FOOD);
            Assert.GreaterOrEqual(testPlanet.Food.ResourceValue, pg.MIN_FOOD);
            Debug.Log("Lava Planet Food is " + testPlanet.Food + "\nCorrect Range: " + pg.MIN_FOOD + "-" + pg.MAX_FOOD);
            yield return null;
        }

        [UnityTest]
        [Description("Rocky planet Food is specified correctly in its min/max range")]
        public IEnumerator CorrectFoodInRangeRocky()
        {
            PlanetGenerator pg = rpGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Food.ResourceValue, pg.MAX_FOOD);
            Assert.GreaterOrEqual(testPlanet.Food.ResourceValue, pg.MIN_FOOD);
            Debug.Log("Rocky Planet Food is " + testPlanet.Food + "\nCorrect Range: " + pg.MIN_FOOD + "-" + pg.MAX_FOOD);
            yield return null;
        }

        [UnityTest]
        [Description("Desert planet Energy is specified correctly in its min/max range")]
        public IEnumerator CorrectEnergyInRangeDesert()
        {
            PlanetGenerator pg = dpGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Energy.ResourceValue, pg.MAX_ENERGY);
            Assert.GreaterOrEqual(testPlanet.Energy.ResourceValue, pg.MIN_ENERGY);
            Debug.Log("Desert Planet Energy is " + testPlanet.Energy + "\nCorrect Range: " + pg.MIN_ENERGY + "-" + pg.MAX_ENERGY);
            yield return null;
        }

        [UnityTest]
        [Description("Ice planet Energy is specified correctly in its min/max range")]
        public IEnumerator CorrectEnergyInRangeIce()
        {
            PlanetGenerator pg = ipGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Energy.ResourceValue, pg.MAX_ENERGY);
            Assert.GreaterOrEqual(testPlanet.Energy.ResourceValue, pg.MIN_ENERGY);
            Debug.Log("Ice Planet Energy is " + testPlanet.Energy + "\nCorrect Range: " + pg.MIN_ENERGY + "-" + pg.MAX_ENERGY);
            yield return null;
        }

        [UnityTest]
        [Description("Iron planet Energy is specified correctly in its min/max range")]
        public IEnumerator CorrectEnergyInRangeIron()
        {
            PlanetGenerator pg = irpGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Energy.ResourceValue, pg.MAX_ENERGY);
            Assert.GreaterOrEqual(testPlanet.Energy.ResourceValue, pg.MIN_ENERGY);
            Debug.Log("Iron Planet Energy is " + testPlanet.Energy + "\nCorrect Range: " + pg.MIN_ENERGY + "-" + pg.MAX_ENERGY);
            yield return null;
        }

        [UnityTest]
        [Description("Green planet Energy is specified correctly in its min/max range")]
        public IEnumerator CorrectEnergyInRangeGreen()
        {
            PlanetGenerator pg = gpGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Energy.ResourceValue, pg.MAX_ENERGY);
            Assert.GreaterOrEqual(testPlanet.Energy.ResourceValue, pg.MIN_ENERGY);
            Debug.Log("Green Planet Energy is " + testPlanet.Energy + "\nCorrect Range: " + pg.MIN_ENERGY + "-" + pg.MAX_ENERGY);
            yield return null;
        }

        [UnityTest]
        [Description("Ocean planet Energy is specified correctly in its min/max range")]
        public IEnumerator CorrectEnergyInRangeOcean()
        {
            PlanetGenerator pg = opGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Energy.ResourceValue, pg.MAX_ENERGY);
            Assert.GreaterOrEqual(testPlanet.Energy.ResourceValue, pg.MIN_ENERGY);
            Debug.Log("Ocean Planet Energy is " + testPlanet.Energy + "\nCorrect Range: " + pg.MIN_ENERGY + "-" + pg.MAX_ENERGY);
            yield return null;
        }

        [UnityTest]
        [Description("Lava planet Energy is specified correctly in its min/max range")]
        public IEnumerator CorrectEnergyInRangeLava()
        {
            PlanetGenerator pg = lpGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Energy.ResourceValue, pg.MAX_ENERGY);
            Assert.GreaterOrEqual(testPlanet.Energy.ResourceValue, pg.MIN_ENERGY);
            Debug.Log("Lava Planet Energy is " + testPlanet.Energy + "\nCorrect Range: " + pg.MIN_ENERGY + "-" + pg.MAX_ENERGY);
            yield return null;
        }

        [UnityTest]
        [Description("Rocky planet Energy is specified correctly in its min/max range")]
        public IEnumerator CorrectEnergyInRangeRocky()
        {
            PlanetGenerator pg = rpGenerator;
            SetUpPlanet(testPlanet, pg);
            Assert.LessOrEqual(testPlanet.Energy.ResourceValue, pg.MAX_ENERGY);
            Assert.GreaterOrEqual(testPlanet.Energy.ResourceValue, pg.MIN_ENERGY);
            Debug.Log("Rocky Planet Energy is " + testPlanet.Energy + "\nCorrect Range: " + pg.MIN_ENERGY + "-" + pg.MAX_ENERGY);
            yield return null;
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(testPlanet);
        }
    }
}