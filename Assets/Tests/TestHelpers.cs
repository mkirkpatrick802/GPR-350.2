using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.TestTools.Utils;

public class TestHelpers {

    public const float TOLERANCE = 1e-5f;
    public static readonly Vector3EqualityComparer Vec3Comparer = new Vector3EqualityComparer(TOLERANCE);
    public static void AssertVector3sEqual(Vector3 actual, Vector3 expected, Vector3EqualityComparer comparer = null)
    {
        if (comparer == null)
            comparer = Vec3Comparer;

        Assert.That(actual, Is.EqualTo(expected).Using(comparer),
            "End position is ({0:0.00000}, {1:0.00000}, {2:0.00000}) but expected ({3:0.00000}, {4:0.00000}, {5:0.00000}) (are you using FixedUpdate?)",
            actual.x, actual.y, actual.z, expected.x, expected.y, expected.z);
    }

    public static IEnumerator TestVelocity(Particle2D particle, Vector2 expectedVelocity, int iterations = 3)
    {
        Vector3 startPos = particle.transform.position;

        float accumulatedTime = 0;
        for (int i = 0; i < iterations; i++)
        {
            yield return new WaitForFixedUpdate();
            accumulatedTime += Time.fixedDeltaTime;
        }
        float dt = accumulatedTime;

        Vector3 endPos = particle.transform.position;
        Assert.AreEqual(startPos.z, endPos.z);

        Vector3 expectedEndPos = startPos + new Vector3(expectedVelocity.x, expectedVelocity.y, 0) * dt;
        AssertVector3sEqual(endPos, expectedEndPos);
    }

    public static void SetValue(object obj, string field, object value)
    {
        FieldInfo fieldInfo = obj.GetType().GetField(field);
        Assert.IsNotNull(fieldInfo, $"Can't set field '{field}' of type '{obj.GetType()}' because it does not exist.");
        fieldInfo.SetValue(obj, value);
    }

    public static T GetValue<T>(object obj, string field)
    {
        FieldInfo fieldInfo = obj.GetType().GetField(field);
        Assert.IsNotNull(fieldInfo, $"Can't get field '{field}' of type '{obj.GetType()}' because it does not exist.");
        Assert.IsAssignableFrom<T>(fieldInfo.GetValue(obj));
        T value = (T)fieldInfo.GetValue(obj);
        return value;
    }
}
