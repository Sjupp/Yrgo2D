using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerPrefsExample : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _textLabel = null;

    private string _stringKey0 = "SaveKey0";

    private void Start()
    {
        string loadedValue = PlayerPrefs.GetString(_stringKey0);
        if (!string.IsNullOrEmpty(loadedValue))
        {
            Debug.Log("Printing saved value: " +  loadedValue);
            _textLabel.text = loadedValue;
        }
        else
        {
            Debug.Log("Nothing saved.");
            _textLabel.text = "N/A";
        }

        if (PlayerPrefs.HasKey("IntegerSaveKey"))
        {
            var loadAnotherValue = PlayerPrefs.GetInt("IntegerSaveKey", -1);
            Debug.Log("Loading saved int: " + loadAnotherValue);
        }
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            PlayerPrefs.SetString(_stringKey0, "hej hej hallå");
            Debug.Log("saved something");
        }
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            PlayerPrefs.SetInt("IntegerSaveKey", 67);
            Debug.Log("saved another thing");
        }
    }
}

public class UnitSpawner
{
    List<int> _savedUnitIDs = new List<int>();
    List<UnitClass> _instantiatedUnits = new();

    private void SpawnUnit(int id = -1)
    {
        if (id != -1)
        {
            // load 
            _instantiatedUnits.Add(new UnitClass(id));
            UnitClass unit = null;
            // Instantiate(_unitPrefab) etc.
            unit.LoadData();
        }
        else
        {
            // spawn new
            var unit = new UnitClass(Random.Range(0, 100000));
            // Instantiate(_unitPrefab) etc.
            _instantiatedUnits.Add(new UnitClass(id));
            _savedUnitIDs.Add(unit.ID);
            unit.SaveData();
        }
    }

    private void SaveAllThings()
    {
        PlayerPrefs.SetInt("numberOfUniqueIDs", _savedUnitIDs.Count);
        for (int i = 0; i < _savedUnitIDs.Count; i++)
        {
            PlayerPrefs.SetInt("unit_" + i, _savedUnitIDs[i]);
        }
    }

    private void LoadAllThings()
    {
        var loadedItems = PlayerPrefs.GetInt("numberOfUniqueIDs");
        for (int i = 0; i < loadedItems; i++)
        {
            if (PlayerPrefs.HasKey("unit_" + i))
            {
                var thingId = PlayerPrefs.GetInt("unit_" + i);
                SpawnUnit(thingId);
            }
        }
    }
}

public class UnitClass
{
    public int ID;
    public string Name = "";
    public int Health = 0;

    public UnitClass(int id)
    {
        ID = id;
    }

    public void SaveData()
    {
        PlayerPrefs.SetString(ID.ToString() + "name", Name);
        PlayerPrefs.SetInt(ID.ToString() + "health", Health);
    }

    public void LoadData()
    {
        Name = PlayerPrefs.GetString(ID.ToString() + "name", Name);
        Health = PlayerPrefs.GetInt(ID.ToString() + "health", Health);
    }
}
