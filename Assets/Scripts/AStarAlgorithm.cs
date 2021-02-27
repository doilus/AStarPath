using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAlgorithm
{
    private int width;
    private int height;
    private GameObject[,] boxes;
    private PathBox path;
    public GameObject footprefab;

    //private int i;
    //private int j;

    private List<PathBox> openList;
    private List<PathBox> closedList;

   

    //definicja koosztów
    private const float move_normal = 1f;               //zwykly ruch lewo/prawo/gora/dol
    private const float move_diagonal = 1.41f;          //ruch skośny
    private const float move_hard = 2f;                 //ruch na pole trudne
    private const float move_water = 2f;                //ruch z normalne/trudne na  pole wody 
    private const float move_waterToWater = 1f;         //ruch z wody na wodę
    private const float move_from_water_normal = 2f;    //ruch z wody na normalne
    private const float move_from_water_hard = 4f;      //ruch z wody na trudne


    public List<PathBox> finalList;        //lista ostateczna

    //tablica przechowująca obiekty klasy PAthBox
    private PathBox[,] listOfBoxes;
    //konstruktor pobiera długosc i szerkokosc
    public AStarAlgorithm(int width, int height, GameObject[,] boxes)
    {
        this.width = width;
        this.height = height;
        this.boxes = boxes;
        //path = new PathBox(boxes, i, j);
        listOfBoxes = new PathBox[width, height];

        //wywołanie metody znalezienie ścieżki
        finalList = FindPath(0, 0, width - 1, height - 1);
        Debug.Log("Ścieżka: " + finalList.Count);
        
    }

    //funkcja zwracająca listę składającą się z pól z całej drogi
    //start i end są tymi samymi polami zawsze bez zmian
    private List<PathBox> FindPath(int start_x, int start_y, int end_x, int end_y)
    {
        
        //GameObject startBox = boxes[start_x, start_y]; //początek 
        //GameObject endBox = boxes[end_x, end_y]; //koniec

       

        //sprawdzenie całej listy i przypisanie im odpowiednich kosztów
        for(int i=0; i < height; i++)
        {
            for(int j=0; j < width; j++)
            {
               //k_vCost[i, j] = int.MaxValue; //ustawiam koszt wszystkich pol na najwieksza mozliwa wartosc
               // CalculateCost(i,j);

              
                        PathBox pathBox = new PathBox(boxes[i, j], i, j);  //przypisanie kolejnych gameobjectow 
                        listOfBoxes[i, j] = pathBox;                    //przypisanie pathBoxów do tablicy
                        pathBox.k_vCost = int.MaxValue;                 //przypisanie maksywalnej mozliwej wartosci dla k(v) dla wszystkich pól
                        //pathBox.k_vCost = CountK_V(pathBox);


                

                        //przypisanie poprawnej wartości k_vCost dla pola
                       /* if(pathBox.isHardGround == true)
                         {
                            pathBox.k_vCost = move_hard;
                         }
                        else if(pathBox.isWater == true)
                         {
                            pathBox.k_vCost = move_water;
                }
                else
                {
                    pathBox.k_vCost = move_normal;
                }*/
                        pathBox.CalculateCost();                        //obliczenie wartości k(v) + h(v)
                        pathBox.cameFromBox = null;                     //aby zainicjowac listę (nie zawiera zadnych danych o wczsniejszej ściezce)

               
                
            }
        }

                //PathBox startBox = new PathBox(boxes[start_x, start_y], start_x, start_y);
                PathBox startBox = listOfBoxes[start_x, start_y];
        //PathBox endBox = new PathBox(boxes[end_x, end_y], end_x, end_y);
        PathBox endBox = listOfBoxes[end_x, end_y];
        //listOfBoxes[start_x, start_y] = startBox;
        //listOfBoxes[end_x, end_y] = endBox;
        openList = new List<PathBox>() { startBox }; //pola oczekujące na przeszukanie + obiekt startowy podpięty jako pierwszy
        closedList = new List<PathBox>(); //pola już przeszukane

        //opisanie kosztu dla pola startowego startBox
        //dla k(v) = 0
        startBox.k_vCost = 0;
        // dla h(v) = FUNKCJA OBLICZAJĄCA
        startBox.h_vCost = CalculateDistance(startBox, endBox);
        //obliczenie h(v) + k(v)
        startBox.CalculateCost();

        //pętla sprawdzająca dopóki openList zawiera jeszcze jakieś elementy
        while(openList.Count > 0)
        {
            //bierzemy aktualne pole, które ma najniższy koszt
            PathBox current = GetLowestCostHK(openList);
            //spr czy jest to pole końcowe 
            if(current == endBox)
            {
                //Debug.Log("ZNALEZIONO!!!!!!!!!!!!!!!!!!!!!!!!!" + current);
                //doszlismy do konca pobieramy tablicę pól w odpowiedniej kolejnosci
                return CalculatePath(endBox);
            }
            //jeżei to nie jest końcowe pole
            openList.Remove(current);      //tzn został już przeszukany
            closedList.Add(current);

            //Debug.Log("current" + current);
            //sprawdzenie sąsiadów dla obecnie wybranego pola
            //pętla dla sąsiadów
            foreach (PathBox neighbourBox in GetNeighbour(current))
            {
               
                //jeżeli nasz sąsiad jest ścianą
                if (neighbourBox.isWall)
                {
                    closedList.Add(neighbourBox);

                    //sprawdzenie warunku prześcia dla ścian w pobliżu, jeżeli pole znajduje się na tym samym X, tzn dóra dół
                    if(current.i == neighbourBox.i)
                    {
                        //to sprawdzamy lewe i prawe pole Musi byc wieksze niz 0 i mniejsze niz width height
                        //dodac metodę dla naroznych
                        if(current.i >= 0 && current.i <= width) 
                        {
                            if (current.i != 0) //nie sprawdzam warunku dla lewej gdy znajduje się po 
                            {
                                if (listOfBoxes[current.i - 1, current.j].isWall) //spr lewą
                                {
                                    
                                    closedList.Add(listOfBoxes[current.i - 1, neighbourBox.j]); //dodaj lewy gorny/dolny skos

                                }
                            }
                            if (current.i != width)
                            {
                                if (listOfBoxes[current.i + 1, current.j].isWall) //spr prawą
                                {
                                    closedList.Add(listOfBoxes[current.i + 1, neighbourBox.j]); //dodaj lewy gorny skos
                                }
                            }
                            continue;

                        }
                    }
                    if (current.j == neighbourBox.j) //spr dla gory i dolu wybrane: lewy lub prawy
                    { 
                        if(current.j >= 0 && current.i <= height)
                        {
                            if (current.j != height)
                            {
                                if (listOfBoxes[current.i, current.j + 1].isWall)
                                {
                                    closedList.Add(listOfBoxes[neighbourBox.i, current.j + 1]); // dodaj gorny
                                }
                            }
                            if (current.j != 0)
                            {
                                if (listOfBoxes[current.i, current.j - 1].isWall)
                                {
                                    closedList.Add(listOfBoxes[neighbourBox.i, current.j - 1]); //dodaj dolne
                                }
                            }
                        }
                        continue;
                    }
                  continue;
                }
                //spr czy neigbourBox znajduje się już na zamkniętej liście
                if (closedList.Contains(listOfBoxes[neighbourBox.i, neighbourBox.j]))
                {
                    continue;       //jeżeli się znajduje to znaczy że już patrzyliśmy na to pole
                }



                //jeżeli nie patrzyliśmy już na dane pole to obliczamy
                float expK_vCost = current.k_vCost + CalculateDistance(current, neighbourBox);      //obliczamy i sprawdzamy odleglosc od poczatku, k(v) koszt obecnego pola + koszt za przesunięcie na następne pole
                //sprawdzam czy expK_vCost jest mniejsze niż akutalny koszt k(v) na polu
                //spr czy posiadamy szybszą trasę z obecnego pola na sąsiednie
                //Debug.Log("sąsiad" + neighbourBox +" " + neighbourBox.k_vCost + " " +  expK_vCost);
                if (expK_vCost < neighbourBox.k_vCost)
                {
                    //jeżeli tak:
                    neighbourBox.cameFromBox = listOfBoxes[current.i, current.j]; //ustawiam obecne pole jako to z którego przybyto
                    //neighbourBox.k_vCost = expK_vCost;  //podmiana k(V) kosztu
                    
                    
                    //muszę sprawdzić z jakim polem mam doczynienia
                    //są różne alternatwy przejścia i zmieniają się przy tym koszty, tak więc: 
                    //przejście normalne na normalne - koszt bez zmian

                    //przejscie z normalnego na trudny ----> x2

                    //spr current dla każdego
                    if(current.isHardGround == true)        //jeżeli obecnie znajdujemy się na trudnym terenie
                    {
                        //co w przypadku gdy nasz nowy "sąsiad" jest:
                        if(neighbourBox.isHardGround == true)       //trudny teren x2
                        {
                            //neighbourBox.k_vCost = expK_vCost * move_hard;
                            neighbourBox.k_vCost = expK_vCost + move_hard;
                            //Debug.Log("działa neighbour last i next" + expK_vCost + "inne" + neighbourBox.k_vCost);//UZUPEŁNIĆ
                        }
                        else if(neighbourBox.isWater == true)                            //woda
                        {
                            neighbourBox.k_vCost = expK_vCost + move_water;
                        } else
                        {
                            neighbourBox.k_vCost = expK_vCost;                  //normal
                        }

                    }
                    else if(current.isWater == true)       //jeżeli obecnie znajdujemy sie na wodzie
                    {
                        //przejscie na trudny teren
                        if (neighbourBox.isHardGround == true)       // x2 x2
                        {
                            neighbourBox.k_vCost = expK_vCost + move_hard + move_water;                                         
                        }

                        //przejscie na wodę
                        else if (neighbourBox.isWater == true)                            //woda
                        {
                            neighbourBox.k_vCost = expK_vCost;      //koszt bez zmian z wody na wodę
                        }
                        else
                        {
                            //jeżeli jest to teren normalny
                            neighbourBox.k_vCost = expK_vCost + move_from_water_normal;
                        }
                    }
                    else //if(current.isHardGround == false && current.isWater == false)                                  //jeżeli obecnie znajdujemy się na normalnym terenie
                    {
                        //jeżeli sąsiad do teren trudny
                        if (neighbourBox.isHardGround == true)       
                        {
                            neighbourBox.k_vCost = expK_vCost + move_hard;                                          //UZUPEŁNIĆ
                        }

                        //jeżeli to teren wodny
                        else if (neighbourBox.isWater == true)                            //woda
                        {
                            neighbourBox.k_vCost = expK_vCost + move_water;
                        }
                        else
                        {
                            //jeżeli to teren zwykły
                            neighbourBox.k_vCost = expK_vCost;
                        }
                    }

                    neighbourBox.h_vCost = CalculateDistance(neighbourBox, endBox);     //wyznaczenie dystansu od sąsiada do końcowego pola
                    neighbourBox.CalculateCost();       //obliczenie kosztu k(v) + h(v)

                    //spr czy nie znajduje się na openLiscie
                    //jeżeli nie to dodaj sąsiada do openList - czyli już odwiedzony
                    if (!openList.Contains(neighbourBox))
                    {
                        openList.Add(neighbourBox);
                    }
                }
            }
        }

        //jeżeli path nie jest pusta

        //jeżeli openList.Count >0 == false ---> wyjście z pętli, to znaczy że przeszukaliśmy wszystkie pola i nie znaleźliśmy ścieżki
        return null;
    }

    //funkcja dostarczająca listę sąsiadów dla obecnie wybranego pola
    private List<PathBox> GetNeighbour(PathBox pathBox) {
        //tworzymy nową listę
        List<PathBox> neighbourList = new List<PathBox>();
        //PathBox newBox;

        //sprawdzamy tylko te które mają x większe od 0 - istneiją
        if (pathBox.i >= 1)
        {
            //Lewa strona
            //newBox = new PathBox(boxes[pathBox.i - 1, pathBox.j], pathBox.i - 1, pathBox.j);
            //neighbourList.Add(new PathBox(boxes[pathBox.i - 1, pathBox.j], pathBox.i - 1, pathBox.j));
            neighbourList.Add(listOfBoxes[pathBox.i - 1, pathBox.j]);

            //lewy dół, jeżeli istnieje coś niżej 
            if (pathBox.j >= 1)
            {
                //neighbourList.Add(new PathBox(boxes[pathBox.i - 1, pathBox.j - 1], pathBox.i - 1, pathBox.j - 1));
                neighbourList.Add(listOfBoxes[pathBox.i - 1, pathBox.j - 1]);
            }
            //lewa góra, jeżeli istneije cos wyżej height - ostatni rząd
            if (pathBox.j < height - 1)
            {
                //neighbourList.Add(new PathBox(boxes[pathBox.i - 1, pathBox.j + 1], pathBox.i - 1, pathBox.j + 1));
                neighbourList.Add(listOfBoxes[pathBox.i - 1, pathBox.j + 1]);
            }
        }

        if (pathBox.i < height - 1)
        {
                //prawo
            //neighbourList.Add(new PathBox(boxes[pathBox.i + 1, pathBox.j], pathBox.i + 1, pathBox.j));
            neighbourList.Add(listOfBoxes[pathBox.i + 1, pathBox.j]);

            //prawy dół - jeżeli istnieje
            if (pathBox.j >= 1)
            {
                 //neighbourList.Add(new PathBox(boxes[pathBox.i + 1, pathBox.j - 1], pathBox.i + 1, pathBox.j - 1));
                 neighbourList.Add(listOfBoxes[pathBox.i + 1, pathBox.j - 1]);
            }
            //prawa góra o ile istnieje
            if (pathBox.j < height - 1)
            {
                //neighbourList.Add(new PathBox(boxes[pathBox.i + 1, pathBox.j + 1], pathBox.i + 1, pathBox.j + 1));
                neighbourList.Add(listOfBoxes[pathBox.i + 1, pathBox.j + 1]);
            }
        }

        //Dół prosto
        if(pathBox.j >= 1)
        {
            //neighbourList.Add(new PathBox(boxes[pathBox.i, pathBox.j - 1], pathBox.i, pathBox.j - 1));
            neighbourList.Add(listOfBoxes[pathBox.i, pathBox.j - 1]);
        }
        if(pathBox.j < height - 1)
        {
            //neighbourList.Add(new PathBox(boxes[pathBox.i, pathBox.j + 1], pathBox.i, pathBox.j + 1));
            neighbourList.Add(listOfBoxes[pathBox.i, pathBox.j + 1]);
        }


        return neighbourList;
        
    }

    //metoda zwracająca listę pól
    //zwracająca z CameFromBox
    private List<PathBox> CalculatePath(PathBox endBox)
    {
        //lista zawierająca ścieżkę
        List<PathBox> path = new List<PathBox>();
        path.Add(endBox); //dodajemy ostatnie pole (końcowe)

        //następnie obecne pole na ktorym sie znajdujemy, zaczynamy od końcowego
        PathBox current = endBox;
         
        while(current.cameFromBox != null)          //tak naprawdę to sprawdzamy "rodzica" dla każdego z pól
        {
            path.Add(current.cameFromBox);      //dodajemy rodzica do ścieżki 
            current = current.cameFromBox;      //podmieniamy current na rodzica i szukamy dalej
        }

        path.Reverse();     //odwrócić bo zaczynałam od końca
        //Debug.Log("Ścieżkowo " + path);
        foreach(PathBox p in path)
        {
            Debug.Log("Ścieżkowo " + p);
        }
        return path;


    }
   

    //oblicza h(V) cost - czyli obliczenie dystansu od początku do końca na bloku - ignorując wszystikie wartosci dodadkowe pol i sciany
    //najszybsza z mozliwych droga (czyli jak da radę najszybciej prosto i potem na skos)
    private float CalculateDistance(PathBox x, PathBox y)
    {
        int xDistance = Mathf.Abs(x.i- y.i); //abs - wartosc bezwzględna
        int yDistance = Mathf.Abs(x.j - y.j);
        int remaining = Mathf.Abs(xDistance - yDistance);

        //zwrot kosztow poruszania się w poziomie i na skos
        //do wyboru ruchu skośnego wybieramy najmniejszą wartosc z xDistance i yDistance
        //*w moim przypadku gdy start i end są całkowicie po skosie; remaining = 0, więc rusza się tylko po skosie
        return move_diagonal * Mathf.Min(xDistance, yDistance) + move_normal * remaining;
    }

    //funkcja sprawdzająca następne pole z najniższym kosztem (indexy pola)
    /*private PathBox GetLowestCostHK(List<PathBox> boxesList)
    {
        PathBox lowest = boxesList[0];                  //ustalamy 1 element jako najmniejszy
        for(int i = 1; i < boxesList.Count; i++)        //szukamy najmniejszych kolejnych elementow przechodząc przez całą tablicę
        {
            if(boxesList[i].h_k_vCost < lowest.h_k_vCost)
            {
                lowest = boxesList[i];
            }
            
        }
        return lowest;
    }*/

    private PathBox GetLowestCostHK(List<PathBox> boxesList)
    {
        PathBox lowest = listOfBoxes[boxesList[0].i, boxesList[0].j];                  //ustalamy 1 element jako najmniejszy
        for (int i = 1; i < boxesList.Count; i++)        //szukamy najmniejszych kolejnych elementow przechodząc przez całą tablicę
        {
            if (listOfBoxes[boxesList[i].i, boxesList[i].j].h_k_vCost < lowest.h_k_vCost)
            {
                lowest = listOfBoxes[boxesList[i].i, boxesList[i].j];
            }

        }
        return lowest;
    }

    public List<PathBox> GetFinalList()
    {
        return finalList;
    }

 
}
