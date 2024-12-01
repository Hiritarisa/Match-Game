using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class Piece : MonoBehaviour
{
    public int x;
    public int y;
    public Board board;

    public enum type
    {
        elephant,
        giraffe,
        hippo,
        monkey,
        panda,
        parrot,
        penguin,
        pig,
        rabbit,
        snake
    };

    public type pieceType;

    public void Setup(int x_, int y_, Board board_){
        x = x_;
        y = y_;
        board = board_;
    }

    public void Move(int desX, int desY)
    {
        // codigo que realiza la transformación del elemento utilizando DoMove para desplazarlo a unas coordenadas especificas utilizando el Vector3
        // luego se concatena la funcion setEase para dar un efecto de secuencia de animación
        // finalmente se crea el callback onComplete con una función de flecha de nos permitirá actualizar los valores de las coordenadas
        transform.DOMove(new Vector3(desX,desY,-5), 0.25f).SetEase(Ease.InOutCubic).onComplete = () => 
        {
            x = desX;
            y = desY;
        };
    }

    // Se da un contextMenu al metodo para añadirlo a Unity en el apartado derecho haciendo clic en los 3 punticos de la clase Piece para ejecutar una funcion automaticamente
    // la funcion que sea definida por un contextMenu no debe recibir parámetro, esta se crea para probar el movimiento
    [ContextMenu("Test Move")]
    public void MoveTest()
    {
        Move(0,0);
    }
}
