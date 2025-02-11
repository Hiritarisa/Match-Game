using UnityEngine;

public class UIStartScreen : MonoBehaviour
{
    public void startBTNClicked(){
        GameManager.Instance.StartGame();
    }
}
