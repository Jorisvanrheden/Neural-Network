using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class NeuralNetwork
{
    private Matrix weights_0;
    private Matrix weights_1;

    private Matrix biases_0;
    private Matrix biases_1;

    public NeuralNetwork(NeuralNetwork copy)
    {
        weights_0 = copy.weights_0.Copy();
        weights_1 = copy.weights_1.Copy();
        biases_0 = copy.biases_0.Copy();
        biases_1 = copy.biases_1.Copy();
    }
    public NeuralNetwork(int input, int hidden, int output)
    {
        weights_0 = new Matrix(hidden, input);
        weights_1 = new Matrix(output, hidden);

        biases_0 = new Matrix(hidden, 1);
        biases_1 = new Matrix(output, 1);

        weights_0.Randomize();
        weights_1.Randomize();

        biases_0.Randomize();
        biases_1.Randomize();
    }

    public NeuralNetwork(string filePath)
    {
        weights_0 = new Matrix(0, 0);
        weights_1 = new Matrix(0, 0);
        biases_0 = new Matrix(0, 0);
        biases_1 = new Matrix(0, 0);

        weights_0.LoadFromString(filePath + "_weights0.txt");
        weights_1.LoadFromString(filePath + "_weights1.txt");

        biases_0.LoadFromString(filePath + "_biases0.txt");
        biases_1.LoadFromString(filePath + "_biases1.txt");
    }

    public Matrix GetWeight0() { return weights_0; }

    public Matrix Estimate(float[] input)
    {
        Matrix inputMatrix = new Matrix(input);

        //Multiply with weights
        Matrix resultMatrix = weights_0.Multiply(inputMatrix);
        //Add bias
        resultMatrix = resultMatrix.Add(biases_0);

        //Activation function (Sigmoid)
        resultMatrix = resultMatrix.ToSigmoid();


        Matrix outputMatrix = weights_1.Multiply(resultMatrix);
        outputMatrix = outputMatrix.Add(biases_1);
        outputMatrix = outputMatrix.ToSigmoid();

        return outputMatrix;
    }

    public void Mutate()
    {
        weights_0.Mutate();
        weights_1.Mutate();

        biases_0.Mutate();
        biases_1.Mutate();
    }

    public void Save(string filePath)
    {
        File.WriteAllLines(filePath + "_weights0.txt", weights_0.ToString());
        File.WriteAllLines(filePath + "_weights1.txt", weights_1.ToString());

        File.WriteAllLines(filePath + "_biases0.txt", biases_0.ToString());
        File.WriteAllLines(filePath + "_biases1.txt", biases_1.ToString());     
    }
}
