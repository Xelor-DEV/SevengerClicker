using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DBScoreSender : MonoBehaviour
{
    [SerializeField] private string baseUrl = "http://localhost/";
    [SerializeField] private string phpFile = "db_create_game_3.php";

    public void SendScoreToDatabase(int score, int userId)
    {
        StartCoroutine(SendScoreRequest(score, userId));
    }

    private IEnumerator SendScoreRequest(int score, int userId)
    {
        string url = baseUrl + phpFile;

        // Crear los datos del puntaje
        RecordScoreRequest requestData = new RecordScoreRequest
        {
            userid = userId,
            score = score,
            created_by = "admin"
        };

        // Convertir a JSON
        string jsonString = JsonUtility.ToJson(requestData);
        Debug.Log("JSON Payload: " + jsonString);

        // Enviar solicitud
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Access-Control-Allow-Origin", "*");
        request.SetRequestHeader("Access-Control-Allow-Methods", "POST, GET, OPTIONS, DELETE, PUT");
        request.SetRequestHeader("Access-Control-Allow-Headers", "Content-Type, Authorization");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Request Error: " + request.error);
        }
        else
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("Response: " + responseText);

            ServerResponse response = JsonUtility.FromJson<ServerResponse>(responseText);
            Debug.Log("Server Message: " + response.message);
        }
    }
}

[System.Serializable]
public class RecordScoreRequest
{
    public int userid;
    public int score;
    public string created_by;
}

[System.Serializable]
public class ServerResponse
{
    public string message;
}