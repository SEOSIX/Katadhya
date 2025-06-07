using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CheatConsole : MonoBehaviour
{
    public GameObject consolePanel;
    public TMP_InputField inputField;
    public TextMeshProUGUI outputText;

    private Dictionary<string, System.Action<string[]>> commands;
    private Dictionary<DataEntity, (int baseLife, int unitLife)> godModeOriginalValues = new();

    void Start()
    {
        consolePanel.SetActive(false);
        ResetCheats();
        inputField.onEndEdit.AddListener(HandleInput);
        commands = new Dictionary<string, System.Action<string[]>>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
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

        if (string.IsNullOrWhiteSpace(input))
            return;

        string[] parts = input.Split(' ');
        string command = parts[0].ToLower();
        string[] args = new string[parts.Length - 1];
        System.Array.Copy(parts, 1, args, 0, args.Length);
        if (!commands.ContainsKey(command))
        {
            switch (command)
            {
                case "god_mode":
                    RegisterCommand("god_mode", ToggleGodMode);
                    break;
                case "kill":
                    RegisterCommand("kill", KillEnemy);
                    break;
                case "skipfight":
                    RegisterCommand("skipfight", SkipFight);
                    break;
                case "caurisinfinite":
                    RegisterCommand("caurisinfinite", SetInfiniteCauris);
                    break;
                default:
                    AppendOutput("Commande inconnue.");
                    return;
            }
        }
        if (commands.TryGetValue(command, out var action))
        {
            action.Invoke(args);
        }
    }

    void RegisterCommand(string name, System.Action<string[]> callback)
    {
        name = name.ToLower();
        if (!commands.ContainsKey(name))
        {
            commands.Add(name, callback);
        }
    }

    void AppendOutput(string message)
    {
        outputText.text += string.IsNullOrEmpty(outputText.text) ? message : " / " + message;
    }

    void ToggleGodMode(string[] args)
    {
        if (args.Length < 1)
        {
            AppendOutput("Usage: god_mode [on/off]");
            return;
        }

        bool enable = args[0].ToLower() == "on";

        var entityHandler = FindObjectOfType<EntiityManager>()?.entityHandler;
        if (entityHandler == null || entityHandler.players == null || entityHandler.players.Count == 0)
        {
            AppendOutput("Aucun joueur trouvé.");
            return;
        }

        foreach (var player in entityHandler.players)
        {
            if (player == null) continue;

            if (enable)
            {
                if (!godModeOriginalValues.ContainsKey(player))
                {
                    godModeOriginalValues[player] = (player.BaseLife, player.UnitLife);
                }

                player.BaseLife = 9999;
                player.UnitLife = 9999;
            }
            else
            {
                if (godModeOriginalValues.TryGetValue(player, out var originalValues))
                {
                    player.BaseLife = originalValues.baseLife;
                    player.UnitLife = Mathf.Min(originalValues.unitLife, originalValues.baseLife);
                    godModeOriginalValues.Remove(player);
                }
            }
        }

        AppendOutput($"God mode {(enable ? "activé" : "désactivé")} pour {entityHandler.players.Count} joueur(s).");
    }

    void KillEnemy(string[] args)
    {
        if (args.Length < 1 || !int.TryParse(args[0], out int index))
        {
            AppendOutput("Usage: kill [index]");
            return;
        }

        var entityHandler = FindObjectOfType<EntityHandler>();
        if (entityHandler == null || entityHandler.ennemies == null)
        {
            AppendOutput("EntityHandler non trouvé.");
            return;
        }

        if (index < 0 || index >= entityHandler.ennemies.Count)
        {
            AppendOutput($"Index {index} hors limites.");
            return;
        }

        var enemy = entityHandler.ennemies[index];
        if (enemy != null)
        {
            enemy.UnitLife = 0;
            AppendOutput($"Ennemi {index} tué.");

            var entityManager = FindObjectOfType<EntiityManager>();
            entityManager?.DestroyDeadEnemies();
        }
    }

    void SkipFight(string[] args)
    {
        var entityHandler = FindObjectOfType<EntityHandler>();
        if (entityHandler != null && entityHandler.ennemies != null)
        {
            foreach (var enemy in entityHandler.ennemies)
            {
                if (enemy != null)
                {
                    enemy.UnitLife = 0;
                }
            }

            var entityManager = FindObjectOfType<EntiityManager>();
            entityManager?.DestroyDeadEnemies();

            AppendOutput("Combat terminé.");
        }
        else
        {
            AppendOutput("EntityHandler non trouvé.");
        }
    }

    void SetInfiniteCauris(string[] args)
    {
        var globalData = FindObjectOfType<ExplorationManager>()?.BigData;
        if (globalData != null)
        {
            for (int i = 0; i < globalData.caurisPerAffinity.Length; i++)
            {
                globalData.caurisPerAffinity[i] = 9999;
            }
            globalData.caurisCount = 9999;

            AppendOutput("Cauris infinis ajoutés.");
        }
        else
        {
            AppendOutput("GlobalPlayerData non trouvé.");
        }
    }
    
    void ResetCheats()
    {
        var entityHandler = FindObjectOfType<EntiityManager>()?.entityHandler;
        if (entityHandler == null || entityHandler.players == null)
            return;

        foreach (var player in entityHandler.players)
        {
            if (player == null) continue;
            if (godModeOriginalValues.TryGetValue(player, out var originalValues))
            {
                player.BaseLife = originalValues.baseLife;
                player.UnitLife = Mathf.Min(originalValues.unitLife, originalValues.baseLife);
            }
            else
            {
                player.UnitLife = player.BaseLife;
            }
        }

        godModeOriginalValues.Clear();
        outputText.text = "";
    }
}
