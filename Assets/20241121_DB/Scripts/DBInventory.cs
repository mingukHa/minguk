using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro.Examples;
using UnityEngine.UIElements;
using UnityEngine.TextCore;

public class DBInventory : MonoBehaviour
{
    public class DataItem
    {
        public string id { get; set; }
        public float positionX { get; set; }
        public float positionY { get; set; }
        public float positionZ { get; set; }
        public float rotationX { get; set; }
        public float rotationY { get; set; }
        public float rotationZ { get; set; }
        public float scaleX { get; set; }
        public float scaleY { get; set; }
        public float scaleZ { get; set; }
        public string tag { get; set; }
    }

    [SerializeField]
    private List<GameObject> invenItemHolder = new List<GameObject>(4);

    private int id;
    private string positionX;
    private string positionY;
    private string positionZ;
    private string rotationX;
    private string rotationY;
    private string rotationZ;
    private float scaleX;
    private float scaleY;
    private float scaleZ;
    private string tag;


    private void Start()
    {
        //StartCoroutine(AddItemCoroutine());
        StartCoroutine(GetItemCoroutine());
    }

    private IEnumerator AddItemCoroutine(
        string _id,
        string _positionX, string _positionY, string _positionZ,
        string _rotationX, string _rotationY, string _rotationZ,
        string _scaleX, string _scaleY, string _scaleZ,
        string _tag)
    {
        string uri = "http://127.0.0.1:3307/additem.php";

        WWWForm form = new WWWForm();
        form.AddField("id", _id);
        form.AddField("positionX", _positionX);
        form.AddField("positionY", _positionY);
        form.AddField("positionZ", _positionZ);
        form.AddField("rotationX", _rotationX);
        form.AddField("rotationY", _rotationY);
        form.AddField("rotationZ", _rotationZ);
        form.AddField("scaleX", _scaleX);
        form.AddField("scaleY", _scaleY);
        form.AddField("scaleZ", _scaleZ);
        form.AddField("tag", _tag);

        using (UnityWebRequest www =
            UnityWebRequest.Post(uri, form))
        {
            yield return www.SendWebRequest();

            if (www.result ==
                UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("AddScore Success : " + _id + "(" + _positionX + ")");
            }
        }
    }

    private IEnumerator GetItemCoroutine()
    {
        string uri = "http://127.0.0.1:3307/getitem.php";

        using (UnityWebRequest www =
            UnityWebRequest.PostWwwForm(uri, string.Empty))
        {
            yield return www.SendWebRequest();

            if (www.result ==
                UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                string data = www.downloadHandler.text;

                List<DataItem> dataItems =
                    JsonConvert.DeserializeObject<List<DataItem>>(data);

                foreach (DataItem dataItem in dataItems)
                {
                    Debug.Log(dataItem.id +
                        " : " + dataItem.positionX +
                        " : " + dataItem.positionY +
                        " : " + dataItem.positionZ +
                        " : " + dataItem.rotationX +
                        " : " + dataItem.rotationY +
                        " : " + dataItem.rotationZ +
                        " : " + dataItem.scaleX +
                        " : " + dataItem.scaleY +
                        " : " + dataItem.scaleZ +
                        " : " + dataItem.tag);
                }
            }
        }
    }
}
