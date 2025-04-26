using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static DataEntity;
using static UnityEditor.Experimental.GraphView.Port;
using static UnityEngine.GraphicsBuffer;

public class CombatManager : MonoBehaviour
{
    public static CombatManager SINGLETON { get; private set; }

    [Header("Entity Handler")]
    [SerializeField]
    public EntityHandler entityHandler;
    public EntiityManager entiityManager;

    [Header("Entity UI")]
    public TextMeshProUGUI textplayer;
    public TextMeshProUGUI speed;
    public TextMeshProUGUI atck;
    public TextMeshProUGUI def;
    public TextMeshProUGUI lifeText;
    public Image playerPortrait;
    public Image[] ImagePortrait;

    public Button[] capacityButtons;
    public Button[] capacityAnimButtons;
    public GameObject[] capacityPage;
    public GameObject[] Banderoles;
    public Sprite[] Pictos;
    public Sprite[] TargetType;
    public Sprite[] PictoBuffs;
    private DataEntity currentPlayer;

    [Header("Turn Management")]
    public Button endTurnButton;
    public Slider LifePlayers;

    [Header("TurnObject")]
    public GameObject ennemyTurn;
    public GameObject TurnUI;
    public CanvasGroup canvasGroup;

    [Header("Target Indicators")]
    public List<GameObject> circlesEnnemy;
    public List<GameObject> circlesPlayer;

    [Header("Ultimate")]
    public Ultimate ultimateScript;

    [HideInInspector] public List<DataEntity> currentTurnOrder = new List<DataEntity>();
    [HideInInspector] public List<DataEntity> unitPlayedThisTurn = new List<DataEntity>();
    private System.Random r = new System.Random();
    [HideInInspector] public bool PlayerClickable;
    [HideInInspector] public bool EnemyClickable;

    public static class GlobalVars
    {
        public static CapacityData currentSelectedCapacity;
    }
    public bool isEnnemyTurn;

   /* private class Encart
    {
        public string Name;
        public string Description;
        public int Type;
        public int TypeValue;
        public int CritValue;
        public int PrecisionValue;
        public int TargetType;
        public int CoolDown;
    }*/

    void Awake()
    {
        if (SINGLETON != null)
        {
            Debug.LogError("Trying to instantiate another CombatManager SINGLETON");
            Destroy(gameObject);
            return;
        }
        SINGLETON = this;
    }

    void Start()
    {
        currentTurnOrder = GetUnitTurn();
        InitializeStaticUI();
        StartUnitTurn();
        SetupBaseStat();
    }

    void Update()
    {
        InitializeStaticUI();
    }

    public void SetupBaseStat()
    {
        for (int i = 0; i < entityHandler.ennemies.Count; i++)
        {
            entityHandler.ennemies[i].UnitAtk = entityHandler.ennemies[i].BaseAtk;
            entityHandler.ennemies[i].UnitDef = entityHandler.ennemies[i].BaseDef;
            entityHandler.ennemies[i].UnitSpeed = entityHandler.ennemies[i].BaseSpeed;
            entityHandler.ennemies[i].ActiveBuffs.Clear();
        }
        for (int i = 0; i < entityHandler.players.Count; i++)
        {
            entityHandler.players[i].UnitAtk = entityHandler.players[i].BaseAtk;
            entityHandler.players[i].UnitDef = entityHandler.players[i].BaseDef;
            entityHandler.players[i].UnitSpeed = entityHandler.players[i].BaseSpeed;
            entityHandler.players[i].ActiveBuffs.Clear();
        }
    }
    public void InitializeStaticUI()
    {
        if (currentTurnOrder == null || currentTurnOrder.Count == 0) return;

        DataEntity currentEntity = currentTurnOrder[0];

        textplayer.text = currentEntity.name;
        speed.text = "Speed :" + currentEntity.UnitSpeed;
        def.text = "Defence :" + currentEntity.UnitDef;
        atck.text = "Attack :" + currentEntity.UnitAtk;
        lifeText.text = currentEntity.UnitLife + "/" + currentEntity.BaseLife;
        playerPortrait.sprite = currentEntity.bandeauUI;
        LifePlayers.maxValue = currentEntity.BaseLife;
        LifePlayers.value = currentEntity.UnitLife;

        List<DataEntity> initialTurnOrder = GetUnitTurn();
        for (int i = 0; i < ImagePortrait.Length; i++)
        {
            if (i < initialTurnOrder.Count && initialTurnOrder[i].UnitLife > 0)
            {
                ImagePortrait[i].enabled = true;
                ImagePortrait[i].sprite = initialTurnOrder[i].portraitUI;
                ImagePortrait[i].transform.parent.gameObject.SetActive(true);
            }
            else
            {
                ImagePortrait[i].enabled = false;
                ImagePortrait[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public List<DataEntity> GetUnitTurn()
    {
        var speedValue = new List<DataEntity>();
        speedValue.AddRange(entityHandler.ennemies.Where(e => e != null && e.UnitLife > 0));
        speedValue.AddRange(entityHandler.players.Where(p => p != null && p.UnitLife > 0));
        return speedValue.OrderByDescending(x => x.UnitSpeed).ToList();
    }

    public void EndUnitTurn()
    {
        if (currentTurnOrder.Count == 0) return;

        DataEntity currentCombatData = currentTurnOrder[0];
        currentTurnOrder.RemoveAt(0);
        unitPlayedThisTurn.Add(currentCombatData);

        if (currentTurnOrder.Count == 0)
        {
            EndGlobalTurn();
        }
        StartUnitTurn();
    }

    public void EndGlobalTurn()
    {
        currentTurnOrder.AddRange(unitPlayedThisTurn);
        unitPlayedThisTurn.Clear();
    }

    public void StartUnitTurn()
    {
        DetectEnnemyTurn();

        if (!entityHandler.ennemies.Contains(currentTurnOrder[0]))
        {
            currentPlayer = currentTurnOrder[0];
            SetupCapacityButtons(currentPlayer);
        }
    }

    public void RemoveUnitFromList(DataEntity _currentUnit)
    {
        if (_currentUnit == null || !currentTurnOrder.Contains(_currentUnit)) return;
        currentTurnOrder.Remove(_currentUnit);
    }

    public void DetectEnnemyTurn()
    {
        DataEntity currentEntity = currentTurnOrder[0];
        if (entityHandler.ennemies.Contains(currentEntity))
        {
            isEnnemyTurn = true;
            //Debug.Log($"🔴 C'est au tour de l'ennemi {currentEntity.namE} !");
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            ennemyTurn.SetActive(true);
            AI.SINGLETON.Attack(currentEntity, 50);
        }
        else
        {
            isEnnemyTurn = false;
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            ennemyTurn.SetActive(false);
        }
    }
    void HideTargetIndicators()
    {
        foreach (var circle in circlesEnnemy)
        {
            circle.SetActive(false);
        }
        foreach (var circle in circlesPlayer)
            circle.SetActive(false);
    }
    public void UseCapacity(CapacityData cpt)
    {
        StartTargetSelectionMode(cpt);
    }

    public void StartTargetSelectionMode(CapacityData capacity)
    {
        HideTargetIndicators();
        GlobalVars.currentSelectedCapacity = capacity;

        if (capacity.MultipleHeal)
        {
            foreach (var ally in entityHandler.players)
            {
                if (ally.UnitLife > 0)
                {
                    ApplyCapacityToTarget(capacity, ally);
                }
            }

            GlobalVars.currentSelectedCapacity = null;
            Debug.Log("Capacité de soin appliquée à tous les alliés !");
            EndUnitTurn();
            return;
        }

        if (capacity.MultipleAttack)
        {
            foreach (var enemy in entityHandler.ennemies)
            {
                if (enemy.UnitLife > 0)
                {
                    ApplyCapacityToTarget(capacity, enemy);
                }
            }

            GlobalVars.currentSelectedCapacity = null;
            Debug.Log("Capacité de zone appliquée à tous les ennemis !");
            EndUnitTurn();
            return;
        }

        //Debug.Log("Sélectionnez une ou plusieurs cibles pour " + capacity.name);
        ShowTargetIndicators(capacity);
    }

    public void SelectEnemy(int enemyIndex)
    {
        if (enemyIndex < 0 || enemyIndex >= entityHandler.ennemies.Count)
            return;
        DataEntity target = entityHandler.ennemies[enemyIndex];

        if (target == null || target.UnitLife <= 0)
        {
            Debug.LogError("Cible invalide ou morte.");
            return;
        }
        ApplyCapacityToTarget(GlobalVars.currentSelectedCapacity, target);
        GlobalVars.currentSelectedCapacity = null;
        HideTargetIndicators();
        EndUnitTurn();
    }

    public void SelectAlly(int allyIndex)
    {
        if (allyIndex < 0 || allyIndex >= entityHandler.players.Count)
        {
            Debug.LogError("Index d'allié invalide !");
            return;
        }
        DataEntity target = entityHandler.players[allyIndex];
        ApplyCapacityToTarget(GlobalVars.currentSelectedCapacity, target);
        GlobalVars.currentSelectedCapacity = null;
        EndUnitTurn();
        HideTargetIndicators();
    }

    public void ApplyCapacityToTarget(CapacityData capacity, DataEntity target)
    {
        DataEntity caster = currentTurnOrder[0];
        float réussite = lancer(capacity.précision, 2f, 1f);
        int DamageDone = 0;
        if (réussite == 2)
        {
            Debug.Log("échec de la compétence");
            DecrementBuffDurations(caster);
            return;
        }
        float modifier = lancer(capacity.critique, 1f, 1.5f);
        if (capacity.atk > 0)
        {
            float calculatedDamage = (((caster.UnitAtk + 1) * capacity.atk * modifier) / (2 + caster.UnitAtk + target.UnitDef));
            int icalculatedDamage = Mathf.RoundToInt(calculatedDamage);
            DamageDone += icalculatedDamage;
            if (target.UnitShield > 0)
            {
                if (target.UnitShield < icalculatedDamage)
                {
                    icalculatedDamage -= target.UnitShield;
                    target.UnitShield = 0;
                    Debug.Log($"{caster.namE} a brisé le shield de {target.namE}");
                }
                else
                {
                    target.UnitShield -= icalculatedDamage;
                    Debug.Log($"{caster.namE} inflige {icalculatedDamage} dégâts au bouclier de {target.namE}");
                    icalculatedDamage = 0;
                }
            }
            if (icalculatedDamage > 0)
            {
                target.UnitLife -= icalculatedDamage;
                Debug.Log($"{caster.namE} inflige {icalculatedDamage} dégâts à {target.namE}");
            }
        }
        if (capacity.heal > 0)
        {
            int healAmount = Mathf.RoundToInt((((caster.UnitAtk) + capacity.heal) / 2) * modifier);
            target.UnitLife = Mathf.Min(target.UnitLife + healAmount, target.BaseLife);
            Debug.Log($"{caster.namE} soigne {target.namE} pour {healAmount} PV");

        }
        if (capacity.Shield > 0)
        {
            target.UnitShield += Mathf.RoundToInt(capacity.Shield * modifier);
            Debug.Log($"{target.namE} se donne {capacity.Shield} de bouclier");
        }
        if (capacity.buffType > 0)
        {
            GiveBuff(capacity, target);
            if (capacity.buffValue > 1)
            {
                Debug.Log($"{target.namE} gagne un buff ({capacity.buffType})");
            }
            else
            {
                Debug.Log($"{target.namE} prend un nerf ({capacity.buffType})");
            }
        }
        if (capacity.Shock > 0)
        {
            ShockProc(capacity, target);
        }
        if (capacity.ShieldRatioAtk > 0)
        {
            int Shielding = Mathf.RoundToInt(((float)capacity.ShieldRatioAtk / 100) * ((float)DamageDone));
            caster.UnitShield += Shielding;
        }
        DecrementBuffDurations(caster);
        caster.ActiveCooldowns.Add(new CooldownData(capacity, capacity.cooldown));
        InitializeStaticUI();
    }

    private void SetupCapacityButtons(DataEntity player)
    {
        for (int i = 0; i < Banderoles.Count(); i++)
        {
            Banderoles[i].SetActive(false);
        }
        if (player.Affinity > 0)
        {
            Banderoles[player.Affinity - 1].SetActive(true);
        }
        for (int i = 0; i < capacityButtons.Length; i++)
        {
            capacityButtons[i].gameObject.SetActive(false);
            capacityAnimButtons[i].onClick.RemoveAllListeners();
            capacityAnimButtons[i].animator.SetTrigger("Normal");
        }

        if (player.capacity1 != null)
        {
            capacityButtons[0].gameObject.SetActive(true);
            capacityButtons[0].GetComponent<Image>().sprite = player.capacity1;
            capacityAnimButtons[0].onClick.AddListener(() => UseCapacity(player._CapacityData1));
        }

        if (player.capacity2 != null)
        {
            capacityButtons[1].gameObject.SetActive(true);
            capacityButtons[1].GetComponent<Image>().sprite = player.capacity2;
            capacityAnimButtons[1].onClick.AddListener(() => UseCapacity(player._CapacityData2));
        }

        if (player.capacity3 != null)
        {
            capacityButtons[2].gameObject.SetActive(true);
            capacityButtons[2].GetComponent<Image>().sprite = player.capacity3;
            capacityAnimButtons[2].onClick.AddListener(() => UseCapacity(player._CapacityData3));
        }

        if (player.Ultimate != null)
        {
            capacityButtons[3].gameObject.SetActive(true);
            capacityButtons[3].GetComponent<Image>().sprite = player.Ultimate;
            capacityAnimButtons[3].onClick.AddListener(() => ultimateScript.QTE_Start());
        }
        UpdatePage(player);
    }

    private void UpdatePage(DataEntity player)
    {
        List<CapacityData> PCapacities = new List<CapacityData>{player._CapacityData1,player._CapacityData2,player._CapacityData3,player._CapacityDataUltimate};
        for (int i=0; i < PCapacities.Count(); i++)
        {
            CapacityData CData = PCapacities[i];
            Transform Parent = capacityPage[i].GetComponent<Transform>();
            Transform Text = Parent.GetChild(4);
            Sprite Target = TargetType[CData.TargetType];
            Sprite PictoType = Pictos[CData.PictoType];
            TextMeshProUGUI Description = Parent.GetChild(1).GetComponent<TextMeshProUGUI>();
            String Sbuff = "";
            Text.GetChild(0).GameObject().SetActive(false);
            Parent.GetChild(3).GetChild(0).GameObject().SetActive(false);
            Parent.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(CData.Name);
            Description.SetText(CData.Description);
            Parent.GetChild(2).GetComponent<Image>().sprite = Target;
            Parent.GetChild(3).GetChild(1).GetComponent<Image>().sprite = PictoType;
            if(CData.buffType != 0)
            {
                Parent.GetChild(3).GetChild(0).GameObject().SetActive(true);
                Text.GetChild(0).GameObject().SetActive(true);
                Parent.GetChild(3).GetChild(0).GetComponent<Image>().sprite = PictoBuffs[CData.buffType-1];
                if (CData.buffValue > 1)
                {
                    Sbuff = $"{Mathf.RoundToInt((CData.buffValue-1)*100)}";
                }
                else
                {
                    Sbuff = $"- {Mathf.RoundToInt((1 - CData.buffValue) * 100)}";
                }
                Text.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(Sbuff);

            }
            
            Text.GetChild(2).GetComponent<TextMeshProUGUI>().SetText($"{CData.précision}%");
            Text.GetChild(3).GetComponent<TextMeshProUGUI>().SetText($"{CData.critique}%");

        }
    }

    void ShowTargetIndicators(CapacityData capacity)
    {
        HideTargetIndicators();

        if (capacity.atk > 0 && !capacity.MultipleAttack)
        {
            for (int i = 0; i < entityHandler.ennemies.Count && i < circlesEnnemy.Count; i++)
            {
                DataEntity enemy = entityHandler.ennemies[i];

                // Vérifie si l'ennemi est valide et vivant
                if (enemy == null || enemy.UnitLife <= 0)
                    continue;
                circlesEnnemy[i].SetActive(true);

            // Active le collider si le GameObject existe
            if (enemy.instance != null)
            {
                PolygonCollider2D enemyColl = enemy.instance.GetComponent<PolygonCollider2D>();
                if (enemyColl != null)
                {
                    enemyColl.enabled = true;
                }
                else
                {
                    Debug.LogWarning($"Pas de PolygonCollider2D sur {enemy.namE}");
                }
            }
        }
    }
    else if (capacity.heal > 0 && !capacity.MultipleHeal)
    {
        for (int i = 0; i < entityHandler.players.Count && i < circlesPlayer.Count; i++)
        {
            DataEntity player = entityHandler.players[i];
            
            if (player == null || player.UnitLife <= 0)
                    continue;
            circlesPlayer[i].SetActive(true);
            if (player.instance != null)
            {
                PolygonCollider2D playerColl = player.instance.GetComponent<PolygonCollider2D>();
                if (playerColl != null)
                {
                    playerColl.enabled = true;
                }
                else
                {
                    Debug.LogWarning($"Pas de PolygonCollider2D sur {player.namE}");
                }
            }
        }
    }
}



    public void GiveBuff(CapacityData capacity, DataEntity target)
    {
        // buffType; 1 = Atk, 2 = Def, 3 = Speed
        //float calculatedBuff;
        if (capacity.buffType > 0)
        {
            ActiveBuff existingBuff = target.ActiveBuffs.Find(b => b.type == capacity.buffType);
            if (existingBuff != null)
            {
                existingBuff.value *= capacity.buffValue;
                existingBuff.duration = Mathf.Max(existingBuff.duration, capacity.buffDuration);
            }
            else
            {
                target.ActiveBuffs.Add(new ActiveBuff(capacity.buffType, capacity.buffValue, capacity.buffDuration));
            }

            RecalculateStats(target);
        }
    }

    public void ShockProc(CapacityData capacity, DataEntity target)
    {
        DataEntity caster = currentTurnOrder[0];
        if (capacity.Shock > 0)
        {
            target.ShockMark += capacity.Shock;
            Debug.Log($"{caster.name} a appliqué {capacity.Shock} marque(s) à {target.namE}");
            if (target.ShockMark >= 3)
            {

                float calculatedDamage = caster.UnitSpeed - 20 / 2;
                float fcalculatedDamage = (float)calculatedDamage * 150 / 100;
                int ishieldDamage = Mathf.RoundToInt(fcalculatedDamage);
                if (target.UnitShield > 0)
                {
                    if (target.UnitShield < ishieldDamage)
                    {
                        ishieldDamage -= target.UnitShield;
                        target.UnitShield = 0;
                        Debug.Log($"{caster.namE} a brisé le shield de {target.namE} grâce au choc");
                    }
                    else
                    {
                        target.UnitShield -= ishieldDamage;
                        ishieldDamage = 0;
                        Debug.Log($"{caster.namE} inflige {ishieldDamage} dégâts au bouclier de {target.namE} grâce au choc");
                    }

                }
                if (ishieldDamage > 0)
                {
                    target.UnitLife -= ishieldDamage;
                    Debug.Log($"{caster.namE} inflige {ishieldDamage} dégâts à {target.namE} grâce au choc");
                }

            }
        }
    }
    public float lancer(int valeur, float above, float under)
    {
        int lancer = UnityEngine.Random.Range(0, 101);
        if (lancer > valeur)
        {
            return above;
        }
        return under;
    }

    public void RecalculateStats(DataEntity target)
    {
        float atkMultiplier = 1f;
        float defMultiplier = 1f;
        float speedMultiplier = 1f;

        foreach (var buff in target.ActiveBuffs)
        {
            switch (buff.type)
            {
                case 1: atkMultiplier *= buff.value; break;
                case 2: defMultiplier *= buff.value; break;
                case 3: speedMultiplier *= buff.value; break;
            }
        }

        target.UnitAtk = Mathf.RoundToInt(target.BaseAtk * atkMultiplier);
        target.UnitDef = Mathf.RoundToInt(target.BaseDef * defMultiplier);
        target.UnitSpeed = Mathf.RoundToInt(target.BaseSpeed * speedMultiplier);
    }
    public void DecrementBuffDurations(DataEntity target)
    {
        for (int i = target.ActiveBuffs.Count - 1; i >= 0; i--)
        {
            Debug.Log($"{target.ActiveBuffs[i]}");
            target.ActiveBuffs[i].duration--;
            if (target.ActiveBuffs[i].duration <= 0)
                target.ActiveBuffs.RemoveAt(i);
        }

        RecalculateStats(target);
    }
    public void SetupNewAffinity(int NewAffinity)
    {
        currentPlayer.Affinity = NewAffinity;
        entiityManager.UpdateSpellData(currentPlayer);
    }

    public void isOnCD(CapacityData capacity)
    {

    }
}