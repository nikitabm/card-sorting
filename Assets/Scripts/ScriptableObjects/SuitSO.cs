using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Suits", menuName = "Settings/Create new suits SO")]

public class SuitSO : ScriptableObject
{
    [Tooltip("List of suits used in code to specify the order of suits for creation and suit operations (e.g. creation of cards, shiffling)")]
    [SerializeField] private List<ESuit> _suitOrder = new List<ESuit>();
    
    public List<ESuit> SuitOrder => _suitOrder;
}
