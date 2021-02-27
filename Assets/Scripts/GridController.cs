using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GridController : MonoBehaviour
{

    public int width;
    public int height;
    public GameObject groundNormalPrefub;
    public GameObject groundHardPrefub;
    public GameObject wallPrefub;
    public GameObject waterPrefub;
    public GameObject startPrefub;
    public GameObject endPrefub;
    public GameObject footPrefab;
    List<GameObject> listFootPrefab;

    public GameObject mybutton;

    private float space = 1.1f;

    private int[,] gridArr; //musi byc dwuwymiarowa - przechowuje wartości pól 

    private AStarAlgorithm astar;

    //h(v) - szacowana odległość od wierzchołka docelowego 
    //k(v) - koszt do końca
    //h(v) + k(v) - 
    private GameObject[,] boxes;

    public GameObject[] prefabs;
    bool ifExistBox; //zmienan sprawdzajaca czy zwracany wektor powinien byc uzyty, czy klikniecie muyszka bylo uznane

    void Start()
    {
        //stworzenie pola 
        // GridController grid = new GridController(height, width, cellPrefab);

        gridArr = new int[height, width];
        listFootPrefab = new List<GameObject>();

        boxes = new GameObject[height,width];

        //inicjowanie tablicy z gameObjectami; odwoływanie się do ich indeksów pozwala na zmianę pola
        prefabs = new GameObject[] { groundNormalPrefub, groundHardPrefub, waterPrefub, wallPrefub };

        //zainicjować dwa pola START i END
        boxes[0, 0] = Instantiate(startPrefub, new Vector3(0, 0, -1) * space, Quaternion.identity) as GameObject;
        boxes[height-1, width-1] = Instantiate(endPrefub, new Vector3(height-1, width-1, -1) * space, Quaternion.identity) as GameObject;
        //funkcja wyświetlająca tablicę
        MakeArrayVisible();


        //ustawienie kamery:
        //punkt środkowy
        Camera.main.transform.position = new Vector3(height * 0.5f, width * 0.5f,  - 10f);

        //skala
        Camera.main.orthographicSize = width/2f + 1f; //skala

        
        //spr
        /*float a = 5.2f;
        float b = 5.5f;

        float a_1 = (float)Math.Round(a, 0);
        float a_2 = (float)Math.Round(b, 0);

        Debug.Log(a_1 + " " + a_2);*/
    }

    // Update is called once per frame
    void Update()
    {
        

        // sprawdzamy kliknięcie lewego przycisku myszy
        if (Input.GetMouseButtonDown(0))
        {
            foreach(GameObject g in listFootPrefab)
            {
                GameObject.Destroy(g);
            }
            //Vector3 vecPosition = Input.mousePosition; - źle nie pobiera wartości scentralizowanej na kamerę

            //PObiera wartość wektora kliknięcia w stosunku do środka kamery
            Vector3 vecPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            //przypisanie wartości klikniętego boxa do nowego wektora
            Vector3 vecPositionBox = IsTherePrefab(vecPosition);
            //if (Physics.CheckSphere(vecPosition,1f) == true)
            //{
            //    Debug.Log("znalazlas" + vecPosition);
            //}
            //else
            //{
            //    Debug.Log("NIE" );
            //}

            //Debug.Log("vecPosition " + vecPosition); 
            
            //wywołuję metodę sprawdzającą tylko jeżeli kliknięcie było w polu
            //oraz jezlei klikniecie nie nalezy do pola początkowego ani ostatniego
            if (ifExistBox) {
                if(vecPositionBox.x == 0 && vecPositionBox.y == 0){}
                else if(vecPositionBox.x == width-1 && vecPositionBox.y == height - 1) { }
                else ChangePrefab(vecPositionBox);
            }

            //sprawdzamy czy został kliknięty przycisk "oblicz"
            //jego wymiary: x 13; y 5 srodka
            //czyli vector musi byc pomiędzy 11,5 a 14,5 ; 4,5 a 5,5
            
            if (vecPosition.x >= 11.5 && vecPosition.x <= 14.5)
            {
                if(vecPosition.y >= 4.5 && vecPosition.y <= 5.5)
                {
                    //został kliknięty przycisko
                    //sprawdzamy czy zadziała algorytm
                    //zerujemy listę z prefabami stópek
                    
                    astar = new AStarAlgorithm(width, height, boxes);
                    List<PathBox> finalList = astar.GetFinalList();

                    if (finalList != null)       //tzn jeżeli znalazło ścieżkę to ją wyświetl na konsoli
                    {
                        float allCost = 0;
                        foreach (PathBox p in finalList)
                        {
                            Debug.Log(p.ToString());

                            // ścieżkę wyswietlam wyświetlając stópki w odpowiednich polach
                            listFootPrefab.Add(Instantiate(footPrefab, new Vector3(p.i, p.j, -1.5f) * 1.1f, Quaternion.identity) as GameObject);

                            //sprawdzenie kosztu
                            
                                allCost = allCost + p.k_vCost;
                                //Debug.Log("Koszt " + p.isHardGround + " " +  p.k_vCost);
                            
                            
                        }
                        Debug.Log("Koszt cały: " + allCost);
                    }

                    


                }
            }
        }
    }

    public void MakeArrayVisible()
    {
        //iteruję po całej tablicy i wyświetlam
        //**** gridArr.GetLength(0) --> 1 wartosc w tablicy dwuargumentowej - row = height
        //**** gridArr.GetLength(1) --> 2 wartość w tablicy dwuargumentowej - column = width
        for (int i=0; i < height; i++)
        {
            for(int j=0; j < width; j++)
            {
                //Debug.Log(i + " " + j);
                //wyświetlam pole TYLKo jeżeli nie są [0,0] i [height-1, width-1] - na tych polach są punkty start i end
                if(i == 0 && j == 0) {}
                else if (i == height-1 && j == width - 1) {}
                else
                {
                    boxes[i, j] = Instantiate(groundNormalPrefub, new Vector3(i, j, -1) * space, Quaternion.identity) as GameObject;
                    gridArr[i, j] = 1;
                }
            }
        }

    }

    //metoda podmieniająca prefub TYLKO WIZUALIZACJA
    //Pamiętaj -- [0,0] i [last, last] NIE MOGĄ BYĆ PODMIENIONE
    /*public void ChangePrefub(Vector3 vector, int howManyClicks) 
    {
        int i = (int)vector.x;
        int j = (int)vector.y;
        //jeżeli jedno kliknięcie to zmień na trudny teren


        switch (howManyClicks)
        {
            case 1:

                GameObject.Destroy(boxes[i, j]); //najpierw usuwamy wczesniejszy GameObject znajdujący się w danym miejscu
                boxes[i, j] = Instantiate(groundHardPrefub, new Vector3(i, j, -1) * space, Quaternion.identity) as GameObject; //zapisujemy nowy
                break;
            case 2:
                GameObject.Destroy(boxes[i, j]); //najpierw usuwamy wczesniejszy GameObject znajdujący się w danym miejscu
                boxes[i, j] = Instantiate(waterPrefub, new Vector3(i, j, -1) * space, Quaternion.identity) as GameObject; //zapisujemy nowy
                break;
            case 3:
                GameObject.Destroy(boxes[i, j]); //najpierw usuwamy wczesniejszy GameObject znajdujący się w danym miejscu
                boxes[i, j] = Instantiate(wallPrefub, new Vector3(i, j, -1) * space, Quaternion.identity) as GameObject; //zapisujemy nowy
                break;
        }


        /*
        if(howManyClicks == 1)
        {
            GameObject.Destroy(boxes[i, j]); //najpierw usuwamy wczesniejszy GameObject znajdujący się w danym miejscu
            boxes[i,j] = Instantiate(groundHardPrefub, new Vector3(i, j, -1) * space, Quaternion.identity) as GameObject; //zapisujemy nowy
        }
        else if (howManyClicks == 2)
        {
            GameObject.Destroy(boxes[i, j]); //najpierw usuwamy wczesniejszy GameObject znajdujący się w danym miejscu
            boxes[i, j] = Instantiate(waterPrefub, new Vector3(i, j, -1) * space, Quaternion.identity) as GameObject; //zapisujemy nowy
        }
        else if (howManyClicks == 3)
        {
            GameObject.Destroy(boxes[i, j]); //najpierw usuwamy wczesniejszy GameObject znajdujący się w danym miejscu
            boxes[i, j] = Instantiate(wallPrefub, new Vector3(i, j, -1) * space, Quaternion.identity) as GameObject; //zapisujemy nowy
        }*/
    /*
    }*/

    //metoda odczytująca jaka wartość jest na danym polu
    public void GetBoxValue(Vector2Int vector)
    {
        
    }

    //metoda sprawdzające istnieje obiektu na danym wektorze (klikniętej myszce)
    //zwraca środek klikniętego kwadratu
    public Vector3 IsTherePrefab(Vector3 vector)
    {
        //przypisanie podstawowych wartości
        float x = vector.x;
        float y = vector.y;
        /*
        //dodatkowa zmienna sprawdzająca potencjalny środek: (zaokrąglająca) - długość wynosi 0.5
        // float x
        int x_middle = (int) Math.Round(x, 0);
        int y_middle = (int) Math.Round(y, 0);

        //wyznaczamy środek "potencjalnego" pola z zaokrągleniem w górę
        float x_midd = x_middle * 1.1f;
        float y_midd = y_middle * 1.1f;

        //wyznaczamy środek pola z zaokrągleniem w dół
        float x_midd_down = x_midd - 1f;
        float y_midd_down = y_midd - 1f;

        //następnie sprawdzamy czy vector znajduje się wewnątrz któregoś z pola
        //środek zawsze jest na X.X Y.Y !!
        float x_result = x_midd - Math.Abs(x);
        float y_result = y_midd - Math.Abs(y);

        //dla pola niżej
        float x_result_down = x_midd_down - Math.Abs(x);
        float y_result_down = y_midd_down - Math.Abs(y);

        //sprawdzenie czy roznica wynosi mniej niż zero oraz czy x oraz y nie przekracza długości ani wysokości pola
        if (Math.Abs(x_result) <= 0.5f && Math.Abs(y_result) <= 0.5f)
        {
            Debug.Log("istnieje wewnątrz " + x_midd + " " + y_midd + "dla" + x + " " + y + "srodek" + x_middle + " " + y_middle);
        }
        else if (Math.Abs(x_result_down) <= 0.5f && Math.Abs(y_result_down) <= 0.5f) {
            Debug.Log("istnieje wewnątrz down " + x_midd_down + " " + y_midd_down + "dla" + x + " " + y + "srodek" + x_middle + " " + y_middle);
        }
        else
        {
            Debug.Log("ktore" + x + " " + y + "wyniki: " + x_result + " " + y_result + "kkkkkk" + x_midd + " " + y_midd + "srodek" + x_middle + " " + y_middle);

        }*/


        //jeszcze jedna próba
        //bool dla sprawdzenia czy obydwa pasuja
        bool x_true = false, y_true = false;
        float x_middle = 0, y_middle = 0;
        /*
         //spr dla szerokosci
         for(int i = 0; i < width; i++)
         {
             x_middle = i * 1.1f;
             if(Math.Abs(x_middle - x) <= 0.5f)
             {
                 x_true = true;
             }
         }
         //dla wysokosci
         for (int i = 0; i < height; i++)
         {
             y_middle = i * 1.1f;
             if (Math.Abs(y_middle - y) <= 0.5f)
             {
                 y_true = true;
             }
         }
        */
        
       
        //podejście z while
        //ponieważ x, y wektoru pasuje tylko do jednego środka kwadratu
        int i = 0; //do iteracji

        //dla x
        while(x_true == false && i < width)
        {
            x_middle = i * 1.1f;
            if (Math.Abs(x_middle - x) <= 0.5f)
            {
                x_true = true;
            }
            i++;
        }

        i = 0;
        //dla y
        while (y_true == false && i < height)
        {
            y_middle = i * 1.1f;
            if (Math.Abs(y_middle - y) <= 0.5f)
            {
                y_true = true;
            }
            i++;
        }

        if (x_true == true && y_true == true)
        {
            Debug.Log("pasuje dla: " + x_middle + " " + y_middle);
            ifExistBox = true;
        }
        else
        {
            Debug.Log("NIE: " + x + " " + y);
            ifExistBox = false;
        }

        //aby otrzymać wektor środka danego kwadratu: 
        Vector3 vectorMiddle = new Vector3(x_middle, y_middle, 0);

        return vectorMiddle;
    }

    public void ChangePrefab(Vector3 vector)
    {
        //sprawdzenie jaki prefub istnieje pod danym polem i zapisanie jego zmiennej
        // dodtkowo wartości wektora dzielimy przez space aby otrzymać indeksy do tabeli pod którym znajduje się ten GameObject - potrzebuję stałych wartości, stąd int
        vector.x = vector.x / space;
        vector.y = vector.y / space;
        int index_i = (int)vector.x; //gdy tutaj dałam int to problem w konwersji (z 5.5 robiło 4)
        int index_j = (int)vector.y;

        GameObject checkObject = boxes[index_i, index_j]; //łapiemy game object

        //usuwam dopis Clone zebym mogla porównac
        checkObject.name = checkObject.name.Replace("(Clone)", "");
        

        //Debug.Log("vector x :" + index_i);
        //Debug.Log("vector y :" + index_j);

        //sprawdzamy który to jest i który ma indeks w tabeli i przechowujemy jego wartość w index_prefab
        int index_prefab = 0;

        for (int i = 0; i < prefabs.Length; i++)
        {
            if (checkObject.name == prefabs[i].name)
            {
                
                index_prefab = i;
            }
        }
        
        // potrzebujemy nowego prefaba dla klikniętego więc dodajemy +1 do index_prefab
        //BARDZO wazne jeżeli index_prefab == 3 (czyli ostatni - wall, to zamieniamy go spowrotem na 0)
        if (index_prefab >= 3)
        {
            index_prefab = 0; 
        }
        else
        {
            index_prefab++;
        }
        //Debug.Log("indeks prefabNEW" + index_prefab);

        //usuń stary prefab
        GameObject.Destroy(boxes[index_i, index_j]);

        //zapisz nowy prefab
        boxes[index_i, index_j] = Instantiate(prefabs[index_prefab], new Vector3(index_i, index_j, -1) * space, Quaternion.identity) as GameObject;
    }



}
