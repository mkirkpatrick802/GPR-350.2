using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneTests : InputTestFixture
{
    Mouse mouse { get => Mouse.current; }
    const string testSceneId = "Assets/Scenes/TestScene.unity";

    public override void Setup()
    {
        base.Setup();
        InputSystem.AddDevice<Keyboard>();
        SceneManager.LoadScene(testSceneId);
    }

    // Test parser expects all tests to be iterated, so we add this
    // variable to make each test run once.
    public static int[] dummyData = new int[] { 0 };

    [UnityTest]
    public IEnumerator CameraTest([ValueSource("dummyData")] int _)
    {
        yield return null;
        Assert.That(Camera.main.orthographic, "Main camera is not orthographic.");
    }

    [UnityTest]
    public IEnumerator LeftRotateTest([ValueSource("dummyData")] int _)
    {
        Gun gun = Object.FindObjectOfType<Gun>();
        Assert.That(gun, Is.Not.Null, "No object of type 'Gun' found in scene {0}", testSceneId);
        Vector3 preMoveRotation = gun.transform.eulerAngles;
        Press(Keyboard.current.digit1Key);
        yield return null;
        Vector3 postMoveRotation = gun.transform.eulerAngles;
        Assert.That(preMoveRotation.x, Is.EqualTo(postMoveRotation.x), "Rotation occurred around X axis, but should only occur around Z axis.");
        Assert.That(preMoveRotation.y, Is.EqualTo(postMoveRotation.y), "Rotation occurred around Y axis, but should only occur around Z axis.");
        Assert.That(postMoveRotation.z, Is.GreaterThan(preMoveRotation.z), "Rotation around Z axis did not occur, or went backwards.");
    }

    [UnityTest]
    public IEnumerator RightRotateTest([ValueSource("dummyData")] int _)
    {
        Gun gun = Object.FindObjectOfType<Gun>();
        Assert.That(gun, Is.Not.Null, "No object of type 'Gun' found in scene {0}", testSceneId);
        Vector3 preMoveRotation = gun.transform.eulerAngles;
        Press(Keyboard.current.digit2Key);
        yield return null;
        Vector3 postMoveRotation = gun.transform.eulerAngles;
        Assert.That(preMoveRotation.x, Is.EqualTo(postMoveRotation.x), "Rotation occurred around X axis, but should only occur around Z axis.");
        Assert.That(preMoveRotation.y, Is.EqualTo(postMoveRotation.y), "Rotation occurred around Y axis, but should only occur around Z axis.");
        Assert.That(postMoveRotation.z, Is.LessThan(preMoveRotation.z), "Rotation around Z axis did not occur, or went backwards.");
    }

    [UnityTest]
    public IEnumerator FireTest([ValueSource("dummyData")] int _)
    {
        Gun gun = Object.FindObjectOfType<Gun>();
        Assert.That(gun, Is.Not.Null, "No object of type 'Gun' found in scene {0}", testSceneId);
        int preFireCount = Object.FindObjectsOfType<Particle2D>().Length;
        Press(Keyboard.current.enterKey);
        yield return null;
        int postFireCount = Object.FindObjectsOfType<Particle2D>().Length;
        Assert.That(postFireCount, Is.GreaterThan(preFireCount), "No additional Particle2D's created after 'enter' key pressed.");
        Assert.That(Object.FindObjectsOfType<Rigidbody>().Length, Is.EqualTo(0), "At least one Rigidbody exists in the scene.");
        Assert.That(Object.FindObjectsOfType<Rigidbody2D>().Length, Is.EqualTo(0), "Rigidbody2D exists in the scene.");
        Particle2D particle = Object.FindObjectsOfType<Particle2D>()[0];
        TestHelpers.AssertVector3sEqual(TestHelpers.GetValue<Vector2>(particle, "velocity").normalized, gun.FireDirection);
    }

    [UnityTest]
    public IEnumerator WeaponCycleTest([ValueSource("dummyData")] int _)
    {
        Gun gun = Object.FindObjectOfType<Gun>();
        Assert.That(gun, Is.Not.Null, "No object of type 'Gun' found in scene {0}", testSceneId);
        GameObject oldWeapon = gun.CurrentWeapon;
        Press(Keyboard.current.wKey);
        yield return null;
        GameObject newWeapon = gun.CurrentWeapon;
        Assert.That(oldWeapon, Is.Not.EqualTo(newWeapon), "CurrentWeapon did not change after 'w' key pressed.");
    }

    [UnityTest]
    public IEnumerator WeaponCountTest([ValueSource("dummyData")] int _)
    {
        // Need to be UnityTest to load scene. Need to return IEnumerator to
        // be UnityTest. Need to yield return at least once to return
        // IEnumerator. So here's a useless return just for you.
        yield return null;

        Gun gun = Object.FindObjectOfType<Gun>();
        Assert.That(gun, Is.Not.Null, "No object of type 'Gun' found in scene {0}", testSceneId);

        bool foundNormalProjectile = false;

        HashSet<GameObject> weapons = new();
        while (!weapons.Contains(gun.CurrentWeapon))
        {
            Assert.That(gun.CurrentWeapon, Is.Not.Null, "Gun's current weapon is null.");
            Assert.That(gun.CurrentWeapon.GetComponent<Particle2D>(), Is.Not.Null, "Gun contains a non-Particle2D weapon");
            weapons.Add(gun.CurrentWeapon);
            GameObject firedProjectile = gun.Fire();
            if (firedProjectile.GetComponentsInChildren<MonoBehaviour>().Length == 1)
            {
                Assert.That(foundNormalProjectile, Is.False, "More than one projectile has only a Particle2D custom component on it. Weapons should differ substantially in behavior, so another custom component should be attached.");
                foundNormalProjectile = true;
            }
            Particle2D particle = firedProjectile.GetComponent<Particle2D>();
            Assert.That(particle, Is.Not.Null, "Fired projectile with no Particle2D component.");
            TestHelpers.AssertVector3sEqual(particle.transform.position, gun.SpawnPosition);
            TestHelpers.AssertVector3sEqual(TestHelpers.GetValue<Vector2>(particle, "velocity").normalized, gun.FireDirection);
            gun.CycleNextWeapon();
        }

        Assert.That(weapons.Count, Is.GreaterThanOrEqualTo(4), "Fewer than 4 unique weapons created.");
    }
}
