using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Board : MonoBehaviour
{
    public int width; // ancho de la pantalla
    public int height; // alto de la pantalla
    public GameObject tileObject; // define un elemento tileObject para su uso en Unity
    public float cameraSizeOffset; // Nos permitira añadir un numero al tamaño ortografico final que tenga la camara
    public float cameraVerticalOffset; // Nos permitirá añadir un valor a la posicion en vertical de la camara
    public GameObject[] availablePieces; // Array de GameObjects para definir las piezas disponibles a ser creadas en la cuadricula

    // Se crean arrays de dos dimensiones (x,y) con el fin de poder encontrar por medio de dos indices la posicion de los objetos
    Tile[,] Tiles;
    Piece[,] Pieces;

    // Se crean variables para almacenar las coordenadas de los elementos clickeados
    Tile startTile;
    Tile endTile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Tiles = new Tile [width,height];
        Pieces = new Piece [width,height];
        SetupBoard();
        PositionCamera();
        SetupPieces();
    }

    private void SetupPieces()
    {
        for (int x = 0; x < width; x++){
            for (int y = 0; y < height; y++)
            {
                var selectedPiece = availablePieces[UnityEngine.Random.Range(0,availablePieces.Length)];

                // Metodo que instancia el selectedPiece definido aleatoriamnente para llenar la cuadricula
                var o = Instantiate(selectedPiece, new Vector3(x,y,-5), Quaternion.identity);
                o.transform.parent = transform;

                // se accede al componente de tipo Pieces del objeto instanciado
                Pieces[x,y] = o.GetComponent<Piece>();
                Pieces[x,y]?.Setup(x, y, this);
            }
        }
    }

    private void PositionCamera()
    {
        float newPositionX = (float)width / 2f;
        float newPositionY = (float)height / 2f;

        // Se crea vector3 para posicionar la camara, se resta 0.5f debido a como esta unity renderizando los elementos
        Camera.main.transform.position = new Vector3(newPositionX - 0.5f, (newPositionY - 0.5f) + cameraVerticalOffset , -10);

        // Se obtiene el tamaño ortografico para las dimensiones verticales y horizontales
        // Tamaño ortografico: Es la mitad de distancia vertical de la pantalla desde el centro a uno de los extremos, similar al radio en un circulo
        float horizontal = width+1;
        float vertical = (height/2)+1;

        // Se hace una validación ternaria para definir cual espacio es mayor, si el vertical u horizontal y definir el tamaño ortografico en base al mayor de estos
        Camera.main.orthographicSize = horizontal > vertical ? (horizontal + cameraSizeOffset) : (vertical + cameraSizeOffset);
    }

    private void SetupBoard(){
        for (int x = 0; x < width; x++){
            for (int y = 0; y < height; y++)
            {
                // Metodo que instancia el tileObject definido dese unity recorriendo el tamaño de la pantalla
                var o = Instantiate(tileObject, new Vector3(x,y,-5), Quaternion.identity);
                o.transform.parent = transform;
                // Se guarda una referencia al componente de Tile dentro del array de 2 dimensiones 
                Tiles[x,y] = o.GetComponent<Tile>();

                // se accede al componente de tipo Tiles del objeto instanciado
                Tiles[x,y]?.Setup(x, y, this);
            }
        }
     }

    // funcion para recibir el espacio inicial que fue clickeado
    public void TileDown(Tile tile_)
    {
        startTile = tile_;
    }

    // funcion para recibir la posición final del arrastrado del elemento
    public void TileOver(Tile tile_)
    {
        endTile = tile_;
    }

    // funcion para validar si se modificó la posicion del elemento clickeado con otro
    public void TileUp(Tile tile_)
    {
        if(startTile != null && endTile != null && IsCloseTo(startTile,endTile))
        {
            SwapTiles();
        }
        startTile = null;
        endTile = null;
    }

    // funcion que se encarga de actualizar las coordenadas de los elementos intercambiados
    private void SwapTiles()
    {
        // Se obtiene la pieza inicial
        var StartPiece = Pieces[startTile.x,startTile.y];
        
        // Se obtiene la pieza final
        var EndPiece = Pieces[endTile.x,endTile.y];

        // Se mueven ambas piezas
        StartPiece.Move(endTile.x,endTile.y);
        EndPiece.Move(startTile.x,startTile.y);

        // Se actualiza el sistema de coordenadas
        Pieces[startTile.x,startTile.y] = EndPiece;
        Pieces[endTile.x,endTile.y] = StartPiece;
    }


    // Metodo para saber si el elemento que se desea mover está a 1 casilla, no aplican diagonales
    public bool IsCloseTo(Tile start, Tile end)
    {
        // Busca si la diferencia entre las posiciones X es 1
        // Busca tambien si la posición en y de la casilla a mover Y a donde será movida es la misma
        if(Math.Abs((start.x-end.x)) == 1 && start.y == end.y)
        {
            return true;
        }

        // Busca si la diferencia entre las posiciones Y es 1
        // Busca tambien si la posición en y de la casilla a mover X a donde será movida es la misma
        if(Math.Abs((start.y-end.y)) == 1 && start.x == end.x)
        {
            return true;
        }

        return false;
    }

    // Metodo para buscar match de elementos en base a la dirección
    public List<Piece> GetMatchByDirection(int xPos, int yPos, Vector2 direction, int matchPieces = 3)
    {
        List<Piece> matches = new List<Piece>();    // Se define un listado de elementos tipo Piece
        Piece startPiece = Pieces[xPos,yPos];   // Se define la posición inicial en base a las coordenadas
        matches.Add(startPiece);    // Se agrega la posición inicial al listado tipo Piece

        // Se crean las variables que almacenarán las próximas posiciones
        int nextX;
        int nextY;

        // Se otorga el valor maximo dependiendo si es mas alto o mas ancho el elemento board
        int maxVal = width > height ? width : height;

        // Se iteran las siguientes posiciones siempre y cuando sean menores a el valor maximo
        for (int i = 1; i < maxVal; i++)
        {
            // Se obtienes las siguientes posiciones
            nextX = xPos + ((int)direction.x * i);
            nextY = yPos + ((int)direction.y * i);

            // Valida que la proxima posicion no salga del maximo de altura, ni de anchura
            if(nextX >= 0 && nextX < width && nextY >= 0 && nextY < height)
            {
                // Vamos a revisar si la proxima posicion tiene el mismo tipo que la pieza inicial
                var nextPiece = Pieces[nextX,nextY];
                if(nextPiece != null && nextPiece.pieceType == startPiece.pieceType){
                    matches.Add(nextPiece); 
                }else{
                    break;
                }
            }
        }
        if (matches.Count >= matchPieces)
        {
            return matches;
        }else{
            return null;
        }
    }

    // Metodo para encontrar los matches en todas las direcciones (up,down,left,right)
    public List<Piece> GetMatchByPiece(int xPos, int yPos, int matchPieces=3)
    {
        // Se obtienen los matches en cada dirección
        var upMatches = GetMatchByDirection(xPos, yPos, new Vector2(0,1), 2);
        var downMatches = GetMatchByDirection(xPos, yPos, new Vector2(0,-1), 2);
        var rightMatches = GetMatchByDirection(xPos, yPos, new Vector2(1,0), 2);
        var leftMatches = GetMatchByDirection(xPos, yPos, new Vector2(-1,0), 2);

        // Se valida que no hayamos obtenido ningun listado nulo, en caso de que si, se deja vacío
        if(upMatches == null) upMatches = new List<Piece>();
        if(downMatches == null) downMatches = new List<Piece>();
        if(rightMatches == null) rightMatches = new List<Piece>();
        if(leftMatches == null) leftMatches = new List<Piece>();

        // Se suman la cantidad de matches encontrados en cada dirección
        var verticalMatches = upMatches.Union(downMatches).ToList();
        var horizontalMatches = rightMatches.Union(leftMatches).ToList();

        // Se crea el listado de match encontrados
        var foundedMatches = new List<Piece>();
        // Se suman los matches verticales y horizontales si encontró la cantidad minima o más
        if (verticalMatches.Count>= matchPieces)
        {
            foundedMatches = foundedMatches.Union(verticalMatches).ToList();
        }
        if (horizontalMatches.Count>= matchPieces)
        {
            foundedMatches = foundedMatches.Union(horizontalMatches).ToList();
        }

        return foundedMatches;
    }
}
