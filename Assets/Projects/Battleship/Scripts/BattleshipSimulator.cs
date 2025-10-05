using System.Collections.Generic;
using UnityEngine;

public class BattleshipSimulator : MonoBehaviour
{
    [SerializeField] private GameObject _obj1 = null;
    [SerializeField] private GameObject _obj2 = null;
    [SerializeField] private Vector3 _staticPos = Vector3.zero;

    private List<int> _myList = new();

    private void Start()
    {
        _myList.Add(4);
    }

    private void Update()
    {
        var obj1ToObj2Vector = _obj2.transform.position - _obj1.transform.position;
        _obj2.transform.position = _staticPos + obj1ToObj2Vector;
    }
}
