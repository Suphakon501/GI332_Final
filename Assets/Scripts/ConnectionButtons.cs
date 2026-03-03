using Unity.Netcode;
using UnityEngine;

public class ConnectionButtons : MonoBehaviour
{
    public GameObject connectionCanvas;

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        connectionCanvas.SetActive(false);
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        connectionCanvas.SetActive(false);
    }

}