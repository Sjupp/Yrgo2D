using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LinqTester : MonoBehaviour
{
    private void Function()
    {
        var myList = new List<int> { 1, 2, 3 };

        myList.Select(x => x);
    }    
}
