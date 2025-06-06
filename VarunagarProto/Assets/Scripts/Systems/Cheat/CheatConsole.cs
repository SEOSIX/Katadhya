using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CheatConsole : MonoBehaviour
{
    public GameObject consolePanel;
    public TMP_InputField inputField;
    public TextMeshProUGUI outputText;

    private Dictionary<string, System.Action<string[]>> commands;

    void Start()
    {
        consolePanel.SetActive(false);
        inputField.onEndEdit.AddListener(HandleInput);

        commands = new Dictionary<string, System.Action<string[]>>();
        RegisterCommand("give_cauris", Custom1);
        RegisterCommand("god_mode", Custom2);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Toggle console panel
            consolePanel.SetActive(!consolePanel.activeSelf);
            if (consolePanel.activeSelf)
            {
                inputField.ActivateInputField();
            }
        }
    }

    void HandleInput(string input)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ExecuteCommand(input);
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }

    void ExecuteCommand(string input)
    {
        AppendOutput($"> {input}");

        string[] parts = input.Split(' ');
        string command = parts[0].ToLower();
        string[] args = new string[parts.Length - 1];
        System.Array.Copy(parts, 1, args, 0, args.Length);

        if (commands.ContainsKey(command))
        {
            commands[command].Invoke(args);
        }
        else
        {
            AppendOutput("Commande inconnue.");
        }
    }

    void RegisterCommand(string name, System.Action<string[]> callback)
    {
        if (!commands.ContainsKey(name.ToLower()))
        {
            commands.Add(name.ToLower(), callback);
        }
    }

    void AppendOutput(string message)
    {
        if (!string.IsNullOrEmpty(outputText.text))
            outputText.text += " / " + message;
        else
            outputText.text = message;
    }
    

    void Custom1(string[] args)
    {
        if (args.Length >= 1 && int.TryParse(args[0], out int amount))
        {
            AppendOutput($"Ajouté {amount} cauris.");
            // TODO : Ajouter l'effet ici (ex: GameManager.instance.AddCauris(amount))
        }
        else
        {
            AppendOutput("Usage : give_cauris [montant]");
        }
    }

    void Custom2(string[] args)
    {
        // Commande : god_mode [on/off]
        if (args.Length >= 1)
        {
            bool isOn = args[0].ToLower() == "on";
            AppendOutput($"God mode {(isOn ? "activé" : "désactivé")}.");
            // TODO : Activer ou désactiver le mode invincible
        }
        else
        {
            AppendOutput("Usage : god_mode [on/off]");
        }
    }
}
