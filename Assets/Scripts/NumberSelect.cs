using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumberSelect : MonoBehaviour
{
    public int number;
    public int min;
    public int max;
    public TextMeshProUGUI text;

    public void increment()
    {
	if(number < max)
	{
	    number++;
	}
	else
	{
	    number = max;
	}
	text.text = number.ToString();
    }

    public void decrement()
    {
	if(number > min)
	{
	    number--;
	}
	else
	{
	    number = min;
	}
	text.text = number.ToString();
    }

    public void set(int nb)
    {
	if(min <= nb && nb <= max)
	{
	    number = nb;
	    text.text = number.ToString();
	}
    }
}
