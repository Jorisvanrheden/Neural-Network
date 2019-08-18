using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using UnityEngine;

public class Matrix
{
    public int Rows { get { return rows; } }
    public int Columns { get { return columns; } }

    private int rows;
    private int columns;
    private float[,] array;

    public Matrix(int rows, int columns)
    {
        this.rows = rows;
        this.columns = columns;

        array = new float[rows, columns];
    }

    public Matrix(float[] input)
    {
        this.rows = input.Length;
        this.columns = 1;

        array = new float[rows, columns];

        for (int i = 0; i < rows; i++) array[i, 0] = input[i];
    }

    public void Randomize()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                array[i, j] = -1 + RandomGenerator.GetRandomNumber()*2;
            }
        }
    }

    public Matrix Add(Matrix matrix)
    {
        Matrix resultMatrix = new Matrix(Rows, Columns);

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                resultMatrix.array[i, j] = array[i, j] + matrix.array[i, j];
            }
        }

        return resultMatrix;
    }

    public Matrix Multiply(Matrix matrix)
    {
        if (Columns != matrix.Rows)
        {
            Console.WriteLine("Rows and columns are not correct");
            return null;
        }

        Matrix resultMatrix = new Matrix(Rows, matrix.Columns);
        for (int i = 0; i < resultMatrix.Rows; i++)
        {
            for (int j = 0; j < resultMatrix.Columns; j++)
            {
                float sum = 0;
                for (int s = 0; s < Columns; s++)
                {
                    sum += array[i, s] * matrix.array[s, j];
                }
                resultMatrix.array[i, j] = sum;
            }
        }
        return resultMatrix;
    }

    public Matrix ToSigmoid()
    {
        Matrix resultMatrix = new Matrix(Rows, Columns);

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                resultMatrix.array[i, j] = Sigmoid(array[i, j]);
            }
        }

        return resultMatrix;
    }

    public float GetValueAt(int i, int j)
    {
        return array[i, j];
    }

    private float Sigmoid(float x)
    {
        return (float)1 / (float)(1 + Math.Pow(Math.E, -x));
    }

    public void Mutate()
    {
        const float mutationRate = 0.1f;

        //int total = Rows * Columns;
       // Debug.Log("Theoretical mutation rate: " + mutationRate);

        //int mutatedAmount = 0;



        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                float threshold = RandomGenerator.GetRandomNumber();
                if (threshold <= mutationRate)
                {
                    array[i, j] = -1 + RandomGenerator.GetRandomNumber() * 2;

                    //mutatedAmount++;
                }


                //float randomA = -1 + RandomGenerator.GetRandomNumber() * 2;
                //float randomB = randomA / 10;

                //float random = 1 + randomB;

                //array[i, j] *= random;
            }
        }

       // Debug.Log("Practical mutation rate: " + (float)mutatedAmount/(float)total);

    }

    public Matrix Copy()
    {
        Matrix matrix = new Matrix(Rows, Columns);

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                matrix.array[i, j] = array[i, j];
            }
        }

        return matrix;
    }

    public string[] ToString()
    {
        List<string> lines = new List<string>();

        for (int i = 0; i < Rows; i++)
        {
            string line = "";
            for (int j = 0; j < Columns; j++)
            {
                line += array[i, j].ToString();
                if (j < Columns - 1) line += " ";
            }

            lines.Add(line);
        }

        return lines.ToArray();
    }

    public void LoadFromString(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);

        int rows = lines.Length;
        int columns = lines[0].Split(' ').Length;

        this.rows = rows;
        this.columns = columns;

        array = new float[rows, columns];

        for (int i = 0; i < lines.Length; i++)
        {
            string columnString = lines[i];
            string[] columnData = columnString.Split(' ');

            for(int j = 0; j < columnData.Length; j++)
            {
                float value = float.Parse(columnData[j]);

                array[i, j] = value;
            }
        }
    }
}
