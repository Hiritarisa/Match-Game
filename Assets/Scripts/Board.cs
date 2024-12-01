using System;
using System.Collections;
using System.Collections.Generic;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

                // se accede al componente de tipo Piece del objeto instanciado
                o.GetComponent<Piece>()?.Setup(x, y, this);
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

                // se accede al componente de tipo Tile del objeto instanciado
                o.GetComponent<Tile>()?.Setup(x, y, this);
            }
        }
     }

    // Update is called once per frame
    void Update()
    {
        
    }
}
