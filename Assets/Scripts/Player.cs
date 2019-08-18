using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public delegate void Trigger(Player player);
    public event Trigger OnTrigger; 

    private int speed = 18;
    private int rotationSpeed = 5;

    private const int INPUTS_COUNT = 7;

    public float fitness = 0;
    public float score = 0;
    public float ExposedRotation = 0;

    private NeuralNetwork neuralnetwork;

    public float WeightA = 0;

    private void Awake()
    {
        Physics.IgnoreLayerCollision(8, 8);
    }

    // Use this for initialization
    void Start ()
    {
            
	}

    // Update is called once per frame
    void Update()
    {
        if (neuralnetwork == null) return;

        score += Time.deltaTime * 10;

        float[] inputs = GetInputs();

        Matrix result = neuralnetwork.Estimate(inputs);

        WeightA = neuralnetwork.GetWeight0().GetValueAt(0,0);


        float rotation = Map(result.GetValueAt(0, 0), 0, 1, -1, 1);
        ExposedRotation = rotation;

        transform.Rotate(Vector3.up, rotation);

        transform.position += transform.forward * Time.deltaTime * speed;   
    }

    private float Map(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        float percentage = (value - oldMin) / (oldMax - oldMin);
        float newValue = newMin + (newMax - newMin) * percentage;

        return newValue;
    }

    public void Initialize(NeuralNetwork network)
    {
        if (network == null) neuralnetwork = new NeuralNetwork(INPUTS_COUNT, 10, 1);
        else neuralnetwork = network;
    }

    public NeuralNetwork GetBrain()
    {
        return new NeuralNetwork(neuralnetwork);
    }

    public float[] GetInputs()
    {
        float[] inputs = new float[INPUTS_COUNT];

        //int circle = 360;
        //int iteration = circle / INPUTS_COUNT;
        //for (int i = 0; i < INPUTS_COUNT; i++)
        //{
        //    int angle = i * iteration;
        //    float z = Mathf.Sin(angle * Mathf.Deg2Rad);
        //    float x = Mathf.Cos(angle * Mathf.Deg2Rad);
        //    Vector3 dir = new Vector3(x, 0, z).normalized;
        //    Vector3 d = transform.forward;
        //    inputs[i] = CalculateHitDistance(transform.position, dir);
        //}

        inputs[0] = CalculateHitDistance(transform.position, -transform.right);
        inputs[1] = CalculateHitDistance(transform.position, GetRotationFromAngle(transform.forward, 315));
        inputs[2] = CalculateHitDistance(transform.position, GetRotationFromAngle(transform.forward, 338));

        inputs[3] = CalculateHitDistance(transform.position, transform.forward);

        inputs[4] = CalculateHitDistance(transform.position, GetRotationFromAngle(transform.forward, 22));
        inputs[5] = CalculateHitDistance(transform.position, GetRotationFromAngle(transform.forward, 45));
        inputs[6] = CalculateHitDistance(transform.position, transform.right);

        return inputs;
    }

    private Vector3 GetRotationFromAngle(Vector3 originalForward, float angle)
    {
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
        Vector3 right = rotation * originalForward;

        return right;
    }

    private float CalculateHitDistance(Vector3 position, Vector3 direction)
    {
        float distance = -1;

        RaycastHit hit;

        Vector3 startPosition = position + direction*2;

        int layerMask = 1 << 8;
        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        if (Physics.Raycast(startPosition, direction, out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(startPosition, direction * hit.distance, Color.yellow);

            distance = hit.distance;
        }

        return distance;
    }

    public void Save(string filePath)
    {
        neuralnetwork.Save(filePath);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (OnTrigger != null) OnTrigger(this);   
    }

    public void Destroy()
    {
        Destroy(gameObject);
        Destroy(this);
    }

    public void MakeInivisible()
    {
        gameObject.SetActive(false);
    }
}
