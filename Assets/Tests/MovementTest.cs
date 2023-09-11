using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Reflection;
using UnityEngine.InputSystem;
using UnityEngine.TestTools.Utils;
using static TestHelpers;

public class MovementTest : InputTestFixture
{
    public class VariableTestData
    {
        public string name;
        public System.Type type;
        public BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        public override string ToString()
        {
            return $"{name}";
        }
    }

    public static VariableTestData[] variableTestDatas = new VariableTestData[]
    {
        new VariableTestData{name = "velocity", type = typeof(Vector2)},
        new VariableTestData{name = "damping", type = typeof(float)},
        new VariableTestData{name = "inverseMass", type = typeof(float)},
        new VariableTestData{name = "acceleration", type = typeof(Vector2)},
    };
    
    [Test]
    public void VariablesExistTest([ValueSource("variableTestDatas")] VariableTestData testData)
    {
        System.Type type = typeof(Particle2D);
        System.Type variableType = testData.type;
        string variableName = testData.name;
        FieldInfo fieldInfo = type.GetField(variableName,
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        Assert.NotNull(fieldInfo, $"no instance variable named '{variableName}' is defined for class '{type.Name}'.");
        Assert.That(fieldInfo.FieldType == variableType, $"'{variableName}' variable is of type {fieldInfo.FieldType}, but should be {variableType.Name}.");
    }

    public struct IntegratorTestData
    {
        public Vector3 startPosition;
        public Vector2 startVelocity;
        public Vector2 acceleration;
        public float damping;
        public Vector3 expectedPosition;
        public float dt;

        public IntegratorTestData(Vector2 startPosition, Vector2 startVelocity, Vector2 expected, float dt):
            this(startPosition, startVelocity, Vector2.zero, 1f, expected, dt)
        {
            
        }

        public IntegratorTestData(Vector2 startPosition, Vector2 startVelocity, Vector2 acceleration, float damping, Vector2 expected, float dt)
        {
            this.startPosition = startPosition;
            this.startVelocity = startVelocity;
            this.expectedPosition = expected;
            this.dt = dt;
            this.acceleration = acceleration;
            this.damping = damping;
        }
    }

    public static IntegratorTestData[] integratorTestDatas = new IntegratorTestData[]
    {
        new IntegratorTestData(Vector2.zero, new Vector2(3, 0), new Vector3(3, 0), 1),
        new IntegratorTestData(Vector2.zero, new Vector2(0, 0), new Vector2(0, -1f), 1f, new Vector3(0, -12.44999f), 5),
        new IntegratorTestData(Vector2.one, new Vector2(-3, 2), new Vector2(0, 0), 0.5f, new Vector3(-2.81338f, 3.54226f), 3),
        new IntegratorTestData(Vector2.zero, new Vector2(3, 0), new Vector2(0, -9.8f), .99f, new Vector3(2.98528f, -4.78660f), 1),
        new IntegratorTestData(new Vector2(1.5f, 5.3f), new Vector2(0, 0), new Vector2(2.44f, 1.3f), 1f, new Vector3(9.064f, 9.33f), 2.5f),
    };

    [Test]
    public void IntegratorTest([ValueSource("integratorTestDatas")] IntegratorTestData testData)
    {
        System.Type integratorType = System.Type.GetType("Integrator, Project");
        Assert.IsNotNull(integratorType, "Integrator type does not exist.");
        MethodInfo integrateMethod = integratorType.GetMethod("Integrate", BindingFlags.Static | BindingFlags.Public);
        Assert.IsNotNull(integrateMethod, "Integrator.Integrate does not exist.");
        Particle2D particle = new GameObject().AddComponent<Particle2D>();

        int iterations = Mathf.RoundToInt(testData.dt / Time.fixedDeltaTime);

        particle.transform.position = testData.startPosition;
        SetValue(particle, "velocity", testData.startVelocity);
        SetValue(particle, "acceleration", testData.acceleration);
        SetValue(particle, "damping", testData.damping);
        for (int i = 0; i < iterations; i++)
        {
            integrateMethod.Invoke(null, new object[] { particle, Time.fixedDeltaTime });
        }
        AssertVector3sEqual(particle.transform.position, testData.expectedPosition);
        Object.Destroy(particle);
    }

    [UnityTest]
    public IEnumerator VelocityTest([ValueSource("integratorTestDatas")] IntegratorTestData testData)
    {
        Particle2D particle = new GameObject().AddComponent<Particle2D>();

        int iterations = Mathf.RoundToInt(testData.dt / Time.fixedDeltaTime);

        particle.transform.position = testData.startPosition;
        SetValue(particle, "velocity", testData.startVelocity);
        SetValue(particle, "acceleration", testData.acceleration);
        SetValue(particle, "damping", testData.damping);
        for (int i = 0; i < iterations; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        AssertVector3sEqual(particle.transform.position, testData.expectedPosition);

        Object.Destroy(particle);
    }
}
