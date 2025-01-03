using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class UITimeBar : MonoBehaviour
{
    public RectTransform fillRect; // 
    public UnityEngine.UI.Image fillColor; // Se define de esta manera debido a los problemas que presenta VSC y Unity
    public Gradient gradient; // 

    // Update is called once per frame
    void Update()
    {
        // Se evalua que tan cerca está la división a uno para calcular el porcentaje del tiempo restante y modificar el UI
        float factor = GameManager.Instance.currentTime / GameManager.Instance.timeOut;
        factor = Mathf.Clamp(factor, 0f, 1f); // Capaz de recibir un valor maximo y valor minimos para limitar a dichos valores los posibles cambios
        factor = 1 - factor; // Se invierte el valor de factor para usar el porcentaje en la UI, ejemplo: si factor es 0.3 la idea es mostrar por pantalla que queda un 70% del tiempo
        
        // Se crea la escala en base al porcentaje obtenido en factor
        fillRect.localScale = new Vector3(factor, 1, 1);
        fillColor.color = gradient.Evaluate(factor);
    }
}
