﻿using System;
public class Dice<T>
{
    private T[] values;

    private Dice() { }

    public Dice(T[] values)
    {
        this.values = values;
    }

    public T GetRandomValue() => values[new Random().Next(0, values.Length)];

}
