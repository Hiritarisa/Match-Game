using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Board : MonoBehaviour
{
    public float timeBetweenPieces = 0.05f;
    public int width; // ancho de la pantalla
    public int height; // alto de la pantalla
    public GameObject tileObject; // define un elemento tileObject para su uso en Unity
    public float cameraSizeOffset; // Nos permitira añadir un numero al tamaño ortografico final que tenga la camara
    public float cameraVerticalOffset; // Nos permitirá añadir un valor a la posicion en vertical de la camara
    public int PointsPerMatch; // Variable que contendra los puntos por Matches
    public GameObject[] availablePieces; // Array de GameObjects para definir las piezas disponibles a ser creadas en la cuadricula

    // Se crean arrays de dos dimensiones (x,y) con el fin de poder encontrar por medio de dos indices la posicion de los objetos
    Tile[,] Tiles;
    Piece[,] Pieces;

    // Se crean variables para almacenar las coordenadas de los elementos clickeados
    Tile startTile;
    Tile endTile;

    // Se crea una variable para detectar si ya finalizamosl a busqueda de matches y permitir mover nuevamente elementos
    bool allowMovement = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Tiles = new Tile [width,height];
        Pieces = new Piece [width,height];
        SetupBoard();
        PositionCamera();
        if(GameManager.Instance.gameState == GameManager.GameState.InGame){
            StartCoroutine(SetupPieces());
        }
        GameManager.Instance.OnGameStateUpdated.AddListener(OnGameStateUpdated); // se 'suscribe' al elemento
    }

    private void OnDestroy() {
        GameManager.Instance.OnGameStateUpdated.RemoveListener(OnGameStateUpdated); // elimina la 'suscripcion' del elemento
    }

    private void OnGameStateUpdated(GameManager.GameState newState)
    {
        if(newState == GameManager.GameState.InGame){
            StartCoroutine(SetupPieces());
        }
        if(newState == GameManager.GameState.GameOver){
            ClearAllPieces();
        }
    }

    private IEnumerator SetupPieces()
    {
        int maxTries = 50;
        int currentTry = 0;
        for (int x = 0; x < width; x++){
            for (int y = 0; y < height; y++)
            {
                yield return new WaitForSeconds(timeBetweenPieces);
                if(Pieces[x,y] == null){
                    currentTry = 0;
                    var newPiece = CreatePieceAt(x, y);
                    while(validateMatchesInCreation(x, y))
                    {
                        DestroyPieceAt(x,y);
                        newPiece = (CreatePieceAt(x,y));
                        currentTry++;
                        if (currentTry > maxTries)
                        {
                            break;
                        }
                    }
                }
            }
        }
        yield return null;
    }

    private void DestroyPieceAt(int x, int y)
    {
        var PieceToClear = Pieces[x,y];
        PieceToClear.removePiece(true); // Se llama la nueva funcion creada para eliminar de forma animada
        Pieces[x, y] = null;
    }

    private void ClearAllPieces()
    {
        for(int x = 0; x<width; x++){
            for (int y = 0; y < height; y++)
            {
                DestroyPieceAt(x,y);
            }
        }
    }

    public Piece CreatePieceAt(int x, int y)
    {
        var selectedPiece = availablePieces[UnityEngine.Random.Range(0,availablePieces.Length)];

        // Metodo que instancia el selectedPiece definido aleatoriamnente para llenar la cuadricula
        var o = Instantiate(selectedPiece, new Vector3(x,y+1,-5), Quaternion.identity);
        o.transform.parent = transform;

        // se accede al componente de tipo Pieces del objeto instanciado
        Pieces[x,y] = o.GetComponent<Piece>();
        Pieces[x,y]?.Setup(x, y, this);
        Pieces[x,y]?.Move(x, y);
        return Pieces[x,y];
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
        // Se valida que este permitido mover las fichas y que el estado de la partida sea 'en juego'
        if (allowMovement && GameManager.Instance.gameState == GameManager.GameState.InGame)
        {
            startTile = tile_;
        }
    }

    // funcion para recibir la posición final del arrastrado del elemento
    public void TileOver(Tile tile_)
    {
        // Se valida que este permitido mover las fichas y que el estado de la partida sea 'en juego'
        if (allowMovement && GameManager.Instance.gameState == GameManager.GameState.InGame)
        {
            endTile = tile_;
        }
    }

    // funcion para validar si se modificó la posicion del elemento clickeado con otro
    public void TileUp(Tile tile_)
    {
        // Se valida que este permitido mover las fichas y que el estado de la partida sea 'en juego'
        if(allowMovement && GameManager.Instance.gameState == GameManager.GameState.InGame){
            if(startTile != null && endTile != null && IsCloseTo(startTile,endTile))
            {
                StartCoroutine(SwapTiles());
            }
        }
    }

    // funcion que se encarga de actualizar las coordenadas de los elementos intercambiados
    // Se crea de tipo IEnumerator para que sea sincrono
    IEnumerator SwapTiles()
    {
        allowMovement = false;
        // Se obtiene la pieza inicial
        var StartPiece = Pieces[startTile.x,startTile.y];
        
        // Se obtiene la pieza final
        var EndPiece = Pieces[endTile.x,endTile.y];

        // Se mueven ambas piezas
        AudioManager.Instance.Move();
        StartPiece.Move(endTile.x,endTile.y);
        EndPiece.Move(startTile.x,startTile.y);

        // Se actualiza el sistema de coordenadas
        Pieces[startTile.x,startTile.y] = EndPiece;
        Pieces[endTile.x,endTile.y] = StartPiece;

        // funcion que suspende el codigo un momento para que espere X cantidad de tiempo antes de continuar
        yield return new WaitForSeconds(0.6f);

        // Se llama el metodo para obtener los matches
        var startMatches = GetMatchByPiece(startTile.x,startTile.y,3);
        var endMatches = GetMatchByPiece(endTile.x,endTile.y,3);

        var matchesResult = startMatches.Union(endMatches).ToList();

        if(matchesResult.Count==0)
        {
            AudioManager.Instance.Miss();
            StartPiece.Move(startTile.x,startTile.y);
            EndPiece.Move(endTile.x,endTile.y);
            Pieces[startTile.x, startTile.y] = StartPiece;
            Pieces[endTile.x, endTile.y] = EndPiece;
        }else{
            DestroyPiecesByList(matchesResult);
            AwardPoints(matchesResult);
        }

        startTile = null;
        endTile = null;
        allowMovement = true;
        yield return null;
    }

    private void DestroyPiecesByList(List<Piece> piecesToDestroy)
    {
        piecesToDestroy.ForEach(piece =>{
            DestroyPieceAt(piece.x, piece.y);
        });

        List<int> PiecesDeleted = GetColumns(piecesToDestroy);
        List<Piece> collapsedPieces = CollapseColumns(PiecesDeleted, 0.3f);
        FindMatchsRecusirvely(collapsedPieces);
    }

    private void FindMatchsRecusirvely(List<Piece> collapsedPieces)
    {
        StartCoroutine(FindMatchsRecusirvelyCoroutine(collapsedPieces));
    }

    IEnumerator FindMatchsRecusirvelyCoroutine(List<Piece> collapsedPieces)
    {
        yield return new WaitForSeconds(1f);
        List<Piece> newMatches = new List<Piece>();
        collapsedPieces.ForEach(piece =>
        {
            var matches = GetMatchByPiece(piece.x, piece.y, 3);
            if (matches != null)
            {
                newMatches = newMatches.Union(matches).ToList();
                DestroyPiecesByList(matches);
                AwardPoints(matches);
            }
        });
        if(newMatches.Count>0){
            var newCollapsedPieces = CollapseColumns(GetColumns(newMatches), 0.3f);
            FindMatchsRecusirvelyCoroutine(newCollapsedPieces);
        }else{
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(SetupPieces());
            allowMovement = true;
        }
        yield return null;
    }

    private List<Piece> CollapseColumns(List<int> columns, float timeToCollapse)
    {
        List<Piece> movingPieces = new List<Piece>();
        for (int i = 0; i < columns.Count; i++)
        {
            var column = columns[i];
            for (int y = 0; y < height; y++)
            {
                if(Pieces[column,y] == null)
                {
                    for (int yPlus = y+1; yPlus < height; yPlus++)
                    {
                        if (Pieces[column,yPlus] != null)
                        {
                            Pieces[column,yPlus].Move(column,y);
                            Pieces[column,y] = Pieces[column,yPlus];
                            if(!movingPieces.Contains(Pieces[column,y]))
                            {
                                movingPieces.Add(Pieces[column,y]);
                            }
                            Pieces[column,yPlus] = null;
                            break;
                        }
                    }
                }
            }
        }
        return movingPieces;
    }

    private List<int> GetColumns(List<Piece> piecesToDestroy)
    {
        var result = new List<int>();
        piecesToDestroy.ForEach(piece =>
        {
            if (!result.Contains(piece.x))
            {
                result.Add(piece.x);
            }
        });
        return result;
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

    bool validateMatchesInCreation(int xPos, int yPos)
    {
        var downMatches = GetMatchByDirection(xPos,yPos, new Vector2(0,-1),2);
        var leftMatches = GetMatchByDirection(xPos,yPos, new Vector2(-1,0),2);

        if (downMatches == null) downMatches = new List<Piece>();
        if (leftMatches == null) leftMatches = new List<Piece>();

        return downMatches.Count>0 || leftMatches.Count > 0;
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


    // Funcion que suma puntos.
    // Opera por medio de Instanciar el GameManager
    public void AwardPoints(List<Piece> allMatches)
    {
        GameManager.Instance.AddPoint(allMatches.Count * PointsPerMatch);
    }
}
