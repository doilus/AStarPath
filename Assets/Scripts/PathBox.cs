using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBox
{
    //zapisuje informacje co do pola i ścieżki
    public int i;
    public int j;

    //public int width;
    //public int height;

    //public List<Transform> boxes;
    //przechowuję koszty dla poszczególnych GameObjectow  
    public float h_vCost; //koszt heurystyczny szacowana odleglosc od wierzchołka docelowego (końcowego)
    public float k_vCost; //szacowana odleglosc od początku
    public float h_k_vCost; //suma

    public PathBox cameFromBox; //wartosc pola z ktorego przyslismy

    //pomaga określić typ naszego pola
    public bool isHardGround = false;
    public bool isWater = false;
    public bool isWall = false;
    


    //konstruktor
    public PathBox(GameObject box, int i, int j)
    {
        this.i = i;
        this.j = j;

        //spr czym jest nasz obiekt
        //poprawiamy jego nazwę
        box.name = box.name.Replace("(Clone)", "");

        //Debug.Log("object: " + box.name);
        if (box.name == "wall")
        {
           // Debug.Log("object: " + box.name);
            isWall = true;
        }
        else if (box.name == "water")
        {
            isWater = true;
        }
        else if (box.name == "groundHard")
        {
            Debug.Log("DZIALA " + box.name);
            isHardGround = true;
        }
        //boxes.Add(box.GetComponent<Transform>());
    }

    //funkcja sumująca koszty h(v) + k(v)
    public void CalculateCost()
    {
        h_k_vCost = h_vCost + k_vCost;
    }

    //metoda wyświetlająca wynik
    public override string ToString()
    {
        return i + " , " + j;
    }



}
