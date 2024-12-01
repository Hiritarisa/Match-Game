using UnityEngine;

// Este Script/Componente se de representar cada espacio dentro de la cuadricula
public class Tile : MonoBehaviour
{

    public int x;
    public int y;
    public Board board;

    // Con esto podremos llevar cuenta de que espacio representa cada Tile que tenemos
    public void Setup(int x_, int y_, Board board_)
    {
        x = x_;
        y = y_;
        board = board_;
    }
}
