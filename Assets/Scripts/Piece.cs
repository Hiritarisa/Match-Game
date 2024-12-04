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

        // Se realizan mejoras a las animaciones
        transform.localScale = Vector3.one * 0.35f; // Acá la figura empieza mas pequeñita
        transform.DOScale(Vector3.one, 0.35f); // Acá la figura toma su tamaño original, en un lapso de 0.35s
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

    public void removePiece(bool animated)
    {
        if(animated)
        {
            transform.DORotate(new Vector3(0, 0, -120f), 0.12f);
            transform.DOScale(Vector3.one * 1.2f, 0.085f).onComplete = () =>
            {
                transform.DOScale(Vector3.zero, 0.1f).onComplete = () =>
                {
                    Destroy(gameObject);
                };
            };
        }else
        {
            Destroy(gameObject);
        }
    }

    // Se da un contextMenu al metodo para añadirlo a Unity en el apartado derecho haciendo clic en los 3 punticos de la clase Piece para ejecutar una funcion automaticamente
    // la funcion que sea definida por un contextMenu no debe recibir parámetro, esta se crea para probar el movimiento
    [ContextMenu("Test Move")]
    public void MoveTest()
    {
        Move(0,0);
    }
}
