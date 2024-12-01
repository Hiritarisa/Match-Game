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

    // Se agrega el input(click) del cursor para detectar el toque inicial con las funciones de Unity las cuales facilitan el uso en dispositivos mobiles
    public void OnMouseDown()
    {
        board.TileDown(this);
    }

    // Se agrega el input del elemento final (arrastrado/clic)
    public void OnMouseEnter()
    {
        board.TileOver(this);
    }

    // Se agrega el input de soltar o dejar de clickear el elemento
    public void OnMouseUp()
    {
        board.TileUp(this);
    }
}
