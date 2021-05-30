using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class sendbyte : MonoBehaviour
{
    public AudioClip clip;
    string savedPath;

    void Start()
    {
        // Mono default behavior does not trust any server;
        // the following is a workaround (there is a better solution to this issue which can be found in the Internet.  
        System.Net.ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => {
            return true;
        };
    }

    public void convertSpeech()
    {
        SavWav.Save("myfile", clip);
        savedPath = Path.Combine(Application.persistentDataPath, "myfile.wav");
        StartCoroutine(converting());
    }

    public IEnumerator converting()
    {
        var url = @"http://127.0.0.1:8000/sendVoice/handling/";
        byte[] postData = File.ReadAllBytes(savedPath);
        Dictionary<string, string> headers = new Dictionary<string, string>();

        WWWForm form = new WWWForm();
        form.AddBinaryData("audio file", postData, "filename", "audio/wav");
        form.headers.Add("Content-Length", postData.Length.ToString());
        form.headers.Add("Transfer-Encoding", "chunked");
        headers = form.headers;
        headers["Content-Type"] = "audio/wav";
        Debug.Log("sending");
        WWW www = new WWW(url, postData, headers);

        yield return www;
        Debug.Log(www.text);

    }
}