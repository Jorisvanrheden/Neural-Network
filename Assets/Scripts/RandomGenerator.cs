using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class RandomGenerator
{
    private static Random random = new Random();

    public static float GetRandomNumber()
    {
        const int DECIMALS = 1000;
        float randomValue = random.Next(0, DECIMALS);
        randomValue /= DECIMALS;

        return randomValue;
    }
}
