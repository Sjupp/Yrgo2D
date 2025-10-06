using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Battleship;

public class TeoBay : IBattleship
{
    bool[,] myPlayField;
    Vector2Int gridSize;

    Vector2Int target;
    Vector2Int boatStartPos = new Vector2Int();
    List<Vector2Int> toPlaceShipsAt = new List<Vector2Int>();

    List<Vector2Int> hasBeenShotIndex;

    bool[,] hasBeenShot;


    bool lastShotHit = false;
    bool lastShotSunk = false;

    bool searchMode = true;
    bool horizontalShip = false;

    bool verticalShip = false;

    int hasRanFor = 0;

    int direction;

    bool lockedInRight = false;

    bool lockedInLeft = false;

    bool lockedInUp = false;

    bool lockedInDown = false;

    bool checkingValidPos = false;

    int blockedSpaces = 0;

    List<int> directions = new List<int> { 0, 1, 2, 3};

    Vector2Int givenPosition;

    Vector2Int shipPartCords;

    bool[,] cantPlaceAt;


    float evenSlotsShot = 0;

    


    //int x, y = 0;


    public string GetName()
    {
        return "Teoman";
    }


    public bool[,] NewGame(Vector2Int gridSize, string opponentName)
    {
        //Create our field
        myPlayField = new bool[gridSize.x, gridSize.y];
        hasBeenShot = new bool[gridSize.x, gridSize.y];
        cantPlaceAt = new bool[gridSize.x + 1, gridSize.y + 1];
        this.gridSize = gridSize;
        Debug.Log(gridSize + " this is the grid size");

        /*for (x = 0; x < gridSize.x; x++)
        {
            for (y = 0; y < gridSize.y; y++)
            {
                slots.Add(new Vector2Int(x, y));
                Debug.Log(slots);
            }
        }*/

        //You don't need to do anything with opponent Name
        //this is more if you want to keep track of your 
        //opponents names and tactics.

        //we now need to place our ships, lets just do one for the demo.

        //Here is the placement of our battleship.

        // myPlayField[0, 2] = true;
        // myPlayField[0, 3] = true;
        // myPlayField[0, 4] = true;
        // myPlayField[0, 5] = true;

        // myPlayField[7, 4] = true;
        // myPlayField[7, 5] = true;

        // myPlayField[1, 7] = true;
        PlaceShip(gridSize, 4, 1);
        PlaceShip(gridSize, 3, 2);
        PlaceShip(gridSize, 2, 1);
        PlaceShip(gridSize, 1, 1);
        
        
        

        //Since we haven't placed all our ships, this would not validate.
        Fire();
        // foreach (var item in cantPlaceAt)
        //     {
        //         if (item)
        //         Debug.Log(++blockedSpaces);
                
        //     }
        // //Debug.Log("amount of places you can't place at " + cantPlaceAt.Length);
        cantPlaceAt = new bool[gridSize.x + 1, gridSize.y + 1];
        return myPlayField;
    }

    private void PlaceShip(Vector2Int gridSize, int boatLength, int amountOfShips)
    {
        for (int i = 0; i < amountOfShips; i++)
        {
            checkingValidPos = true;
            while (checkingValidPos && hasRanFor < 10000)
            {
                hasRanFor++;
                checkingValidPos = false;
                NullPlacementVariables();
                direction = Random.Range(0, 3);
                boatStartPos.x = Random.Range(0, gridSize.x);
                boatStartPos.y = Random.Range(0, gridSize.y);
                if (!(myPlayField[boatStartPos.x, boatStartPos.y] && cantPlaceAt[boatStartPos.x, boatStartPos.y]))
                {
                    //Debug.Log("first part" + boatStartPos);
                    toPlaceShipsAt.Add(boatStartPos);

                    for (int repeats = boatLength - 1; repeats > 0; repeats--)
                    {

                        shipPartCords = GetNeighbour(toPlaceShipsAt.Last(), direction); //also sets direction variable to the random direction selected before
                        //Debug.Log("Is about to check if it can place at " + givenPosition + gridSize);
                        if (shipPartCords == boatStartPos)
                        {
                            checkingValidPos = true;
                            break;
                        }
                        else if (!myPlayField[shipPartCords.x, shipPartCords.y] && !cantPlaceAt[shipPartCords.x, shipPartCords.y])
                        {
                            toPlaceShipsAt.Add(shipPartCords);

                        }
                        else
                        {
                            checkingValidPos = true;
                            break;
                        }
                    }


                    if (toPlaceShipsAt.Count == boatLength)
                    {
                        Debug.Log("Placing thing of " + boatLength + " length");
                        foreach (var item in toPlaceShipsAt)
                        {
                            Debug.Log("placed at " + item);
                            myPlayField[item.x, item.y] = true;
                            cantPlaceAt[shipPartCords.x, shipPartCords.y] = true;
                            cantPlaceAt[shipPartCords.x + 1, shipPartCords.y] = true;
                            cantPlaceAt[shipPartCords.x, shipPartCords.y + 1] = true;
                            cantPlaceAt[Mathf.Clamp(shipPartCords.x - 1, 0, gridSize.x), shipPartCords.y] = true;
                            cantPlaceAt[shipPartCords.x, Mathf.Clamp(shipPartCords.y - 1, 0, gridSize.y)] = true;
                        }
                        checkingValidPos = false;
                    }
                }

            }
        }
    }

    public Vector2Int Fire()
    {
        if (lastShotHit & searchMode)
        {
            searchMode = false;
        }

        if (searchMode)
        {
            GetSlot();
            //prevTargets.Add(target);
        }
        else
        {
            int rand = Random.Range(0, 3);
            if (rand == 0) //Fire Right 
            {
                target = new Vector2Int();
            }
            if (rand == 1) //Fire Left 
            {
                target = new Vector2Int();
            }
            if (rand == 2) //Fire Up 
            {
                target = new Vector2Int();
            }
            if (rand == 3) //Fire Down
            {
                target = new Vector2Int();
            }

        }
        return target;
    }

    public void Result(Vector2Int position, bool hit, bool sunk)
    {

        if (hit == true)
        {
            lastShotHit = true;
        }
        else
        {
            lastShotHit = false;
        }
        if (sunk == true)
        {
            lastShotSunk = true;
        }
        else
        {
            lastShotSunk = false;
        }
    }

    public Vector2Int GetSlot()
    {
        target.x = Random.Range(0, gridSize.x);
        target.y = Random.Range(0, gridSize.y);
        // if (evenSlotsShot >= (gridSize.x * gridSize.y) / 2)
        // {
        //     evenOrOdd = 1;
        // }
        if (hasBeenShot[target.x, target.y] == false)
        {
            hasBeenShot[target.x, target.y] = true;

            return target;
        }
        else
        {
            GetSlot();
            return target;
        }

    }
    public Vector2Int GetNeighbour(Vector2Int position, int direction)
    {
        hasRanFor++;
        if (hasRanFor > 500)
        {
            checkingValidPos = false;
            return position;
        }
        givenPosition = position;
        switch (direction)
        {
            case 0:
                givenPosition = new Vector2Int(givenPosition.x + 1, givenPosition.y);
                //Debug.Log("right works");
                break;
            case 1:
                givenPosition = new Vector2Int(givenPosition.x - 1, givenPosition.y);
                //Debug.Log("left works");
                break;
            case 2:
                givenPosition = new Vector2Int(givenPosition.x, givenPosition.y + 1);
                //Debug.Log("up works");
                break;

            case 3:
                givenPosition = new Vector2Int(givenPosition.x, givenPosition.y - 1);
                //Debug.Log("down works");

                break;

            default:
                // code block
                break;
        }

        //Debug.Log(givenPosition);

        if (!(givenPosition.x < 0 || givenPosition.x > gridSize.x - 1|| givenPosition.y < 0 || givenPosition.y > gridSize.y - 1 || checkingValidPos))
        {
            //Debug.Log("can place at " + givenPosition);
            return givenPosition;
        }
        else
        {
            //Debug.Log("out of range at " + givenPosition);
            return position;
        }

    }


    void NullPlacementVariables()
    {
        lockedInDown = lockedInLeft = lockedInRight = lockedInUp = false;
        directions = new List<int> { 0, 1, 2, 3 };
        toPlaceShipsAt.Clear();
    }
}