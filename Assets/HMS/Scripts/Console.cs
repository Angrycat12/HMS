using UnityEngine;
using TMPro;


public class Console : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField InputField;
    [SerializeField] private GameObject CommandMessagePrefab;
    [SerializeField] private float MinSize;
    [SerializeField] private Vector3 LastMessagePosition = new(0, -7, 0); 
    [SerializeField] private float MessageBiasForce = 3;

    private void Start()
    {
        InputField.onSubmit.AddListener(SendCommand);
    }
    
    public void SendCommand(string value)
    {
        InputField.text = "";
        string[] commandvalue = value.Split(' ');
        string commandAnswer = "Command not found";
        if(commandvalue[0] == "echo")
        {
            commandAnswer = value;
        }

        if(commandvalue[0] == "svcheat") 
        {
            try
            {
                if ((int)commandAnswer[1] == 1) commandAnswer = "Yes common, <color=green>yes</color>";
                if ((int)commandAnswer[1] == 0) commandAnswer = "the is standart";
            }
            catch {commandAnswer = "<color=red>the specified value are incorrect</color>";}
        }
        ShowCommand(commandAnswer);
    }

    private void ShowCommand(string value)
    {
        Debug.Log(value);
        LastMessagePosition.Set(LastMessagePosition.x, LastMessagePosition.y * MessageBiasForce, LastMessagePosition.z);
        GameObject message = Instantiate(CommandMessagePrefab, LastMessagePosition, Quaternion.identity, gameObject.transform);
        message.GetComponent<TMP_Text>().text = value;
    }
}
