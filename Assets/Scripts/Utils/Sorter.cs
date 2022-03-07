using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sorter : MonoBehaviour
{
    public List<int>[] SubsequentSort(List<CardDisplay> cardsToSort, SettingsSO settings)
    {
        // save suit settings
        var cardSettings = settings.cardSettings;
        // calculate max possible number of successful card combinations;
        var maxSortedCardGroups = settings.CardsDealNum / settings.MinLegalCardsCombNum;
        // array of lists that will store valid card combinations + LINQ initialization
        var sortedCardsIds = new List<int>[maxSortedCardGroups].Select(item => new List<int>()).ToArray();
        // array of lists that will store sorted by suit cards + LINQ initialization
        var cardsInSuits = new List<CardDisplay>[cardSettings.SuitOrder.Count].Select(item => new List<CardDisplay>()).ToArray();

        int numOfCompletedCombinations = 0;
        for (int i = 0; i < cardsInSuits.Length; i++)
        {
            // sort cards of every suit to separate list
            cardsInSuits[i] = cardsToSort.Where(cardDisplay => cardSettings.SuitOrder.IndexOf(cardDisplay.Card.Suit) == i).ToList();
            if (cardsInSuits[i].Count == 0)
            {
                continue;
            }
            // order list by rank of card (A,2,3,4...)
            List<CardDisplay> sortedCardDisplays = cardsInSuits[i].OrderBy(CardDisplay => CardDisplay.Card.Rank).ToList();
            var currentCombination = new List<int>();
            // add first card to the current combination list
            currentCombination.Add(cardsToSort.IndexOf(sortedCardDisplays[0]));
            // loop over the rest of cards
            for (int j = 1; j < sortedCardDisplays.Count; j++)
            {
                // if next card has subsequent (rank+1) rank - add it to the current combination list
                if (cardsToSort[currentCombination[currentCombination.Count - 1]].Card.Rank + 1 == sortedCardDisplays[j].Card.Rank)
                {
                    currentCombination.Add(cardsToSort.IndexOf(sortedCardDisplays[j]));
                }
                else
                {
                    // if combination is valid - add it to the result array, increment num of completed combinations
                    if (currentCombination.Count >= settings.MinLegalCardsCombNum)
                    {
                        sortedCardsIds[numOfCompletedCombinations] = new List<int>(currentCombination);
                        numOfCompletedCombinations++;
                    }
                    currentCombination.Clear();
                    // add current card as the first in the current combination
                    currentCombination.Add(cardsToSort.IndexOf(sortedCardDisplays[j]));
                    continue;
                }
            }
            // check if last cards we were looping over form legal combination
            if (currentCombination.Count >= settings.MinLegalCardsCombNum)
            {
                sortedCardsIds[numOfCompletedCombinations] = new List<int>(currentCombination);
                numOfCompletedCombinations++;
            }
            currentCombination.Clear();
        }
        // remove empty combinations
        return sortedCardsIds.Where(combList => combList.Count != 0).ToArray();
    }

    public List<int>[] SameValueSort(List<CardDisplay> cardsToSort, SettingsSO settings)
    {
        int numOfCompletedCombinations = 0;
        // save suit settings
        var cardSettings = settings.cardSettings;
        // calculate max possible number of successful card combinations;
        var maxSortedCardGroups = settings.CardsDealNum / settings.MinLegalCardsCombNum;
        // array of lists that will store valid card combinations + LINQ initialization
        var sortedCardsIds = new List<int>[maxSortedCardGroups].Select(item => new List<int>()).ToArray();
        // sort cards in hand by card rank
        List<CardDisplay> sortedCardDisplays = cardsToSort.OrderBy(CardDisplay => CardDisplay.Card.Rank).ToList();

        for (int i = 0; i < sortedCardDisplays.Count; i++)
        {
            int currentCardRank = sortedCardDisplays[i].Card.Rank;
            if (sortedCardDisplays.Count - i < settings.MinLegalCardsCombNum)
            {
                break;
            }
            // add first card to combination list, make sure we don't add already added card id
            if (!sortedCardsIds[numOfCompletedCombinations].Contains(cardsToSort.IndexOf(sortedCardDisplays[i])))
            {
                sortedCardsIds[numOfCompletedCombinations].Add(cardsToSort.IndexOf(sortedCardDisplays[i]));
            }
            // loop over next 3 cards to see if there are any cards of the same rank
            // if can't form combination - clear current combination list, continue looping over rest of the cards
            // set i to next card of different rank to skip cards of the same rank
            for (int j = i + 1; j < i + 4; j++)
            {
                if (j < sortedCardDisplays.Count)
                {
                    if (sortedCardDisplays[j].Card.Rank == currentCardRank)
                    {
                        sortedCardsIds[numOfCompletedCombinations].Add(cardsToSort.IndexOf(sortedCardDisplays[j]));
                    }
                    else
                    {
                        if (sortedCardsIds[numOfCompletedCombinations].Count >= settings.MinLegalCardsCombNum)
                        {
                            numOfCompletedCombinations++;
                        }
                        else
                        {
                            sortedCardsIds[numOfCompletedCombinations].Clear();
                        }
                        i = j - 1;
                        break;
                    }
                }
                else
                {
                    if (sortedCardsIds[numOfCompletedCombinations].Count < settings.MinLegalCardsCombNum)
                    {
                        sortedCardsIds[numOfCompletedCombinations].Clear();
                    }
                    break;
                }
                // when reached the end of loop = jump to the last element to skip unnecessary loops
                i = j - 1;
            }
        }
        // remove empty combination lists
        return sortedCardsIds.Where(combList => combList.Count != 0).ToArray();
    }

    public List<int>[] SmartSorting(List<CardDisplay> cardsToSort, SettingsSO settings)
    {
        List<CardDisplay> cardsCopy = new List<CardDisplay>(cardsToSort);
        // save suit settings
        var cardSettings = settings.cardSettings;
        // calculate max possible number of successful card combinations;
        var maxSortedCardGroups = settings.CardsDealNum / settings.MinLegalCardsCombNum;

        // get combinations from 123 sorting
        var subsequentValueCombs = SubsequentSort(cardsToSort, settings);
        // split subsequent value combinations into all possible combinations
        var splitSubsequentValueCombs = SplitSubsequentValueCombinations(subsequentValueCombs, settings);
        // get combinations from 777 sorting
        var sameValueCombs = SameValueSort(cardsToSort, settings);
        // split same value combinations into all possible minimum card num combinations
        var splitSameValueCombs = SplitSameValueCombinations(sameValueCombs);

        List<List<int>> allCombs = subsequentValueCombs.Concat(splitSubsequentValueCombs).Concat(sameValueCombs).Concat(splitSameValueCombs).ToList();
        // sort all combinations by its value (low to high)
        List<List<int>> sortedCombs = allCombs.OrderBy(combination => GetCombinationValue(combination, cardsToSort, cardSettings)).ToList();
        // reverse all combinations so that it goes (high to low)
        sortedCombs.Reverse();

        // most optimal number of combinations. list element points to combination in allCombs
        int bestCombScore = 0;

        List<List<int>> bestCombs = new List<List<int>>();

        int totalCardsValue = GetCardsValue(cardsCopy, cardSettings);
        int sum = 0;
        /*
        loop over all possible combinations, for every combination go through the rest non intersecting combinations
        keep track of best score and best combinations you got so far (score has to be <= total cards score)
        */
        for (int i = 0; i < sortedCombs.Count; i++)
        {
            List<List<int>> testCombs = new List<List<int>>();
            testCombs.Add(sortedCombs[i]);

            for (int j = 0; j < sortedCombs.Count; j++)
            {
                if (i != j)
                {
                    bool hasSameElements = false;
                    foreach (var comb in testCombs)
                    {
                        hasSameElements = comb.Intersect(sortedCombs[j]).Any();
                        if (hasSameElements)
                        {
                            break;
                        }
                    }
                    if (!hasSameElements)
                    {
                        testCombs.Add(sortedCombs[j]);
                        int currentCombinationSum = GetCombinationsValue(testCombs, cardsToSort, cardSettings);
                        if (currentCombinationSum > totalCardsValue)
                        {
                            testCombs.Remove(sortedCombs[j]);
                        }
                        sum = GetCombinationsValue(testCombs, cardsToSort, cardSettings);
                        if (sum > bestCombScore)
                        {
                            bestCombScore = sum;
                            bestCombs = testCombs;
                        }
                    }
                }
            }

            sum = GetCombinationsValue(testCombs, cardsToSort, cardSettings);
            if (sum > bestCombScore)
            {
                bestCombScore = sum;
                bestCombs = testCombs;
            }
        }
        return bestCombs.ToArray();
    }

    /*
    Split full consecutive combinations into all possible minimum combinations
    with min possible combination num = 3, [2,3,4,5] will be split into [2,3,4],[3,4,5]
    */
    private List<int>[] SplitSubsequentValueCombinations(List<int>[] combs, SettingsSO settings)
    {
        List<List<int>> splitCombinations = new List<List<int>>();
        for (int i = 0; i < combs.Length; i++)
        {
            if (combs[i].Count > settings.MinLegalCardsCombNum)
            {
                for (int j = 0; j < combs[i].Count; j++)
                {
                    int chunkSize = settings.MinLegalCardsCombNum;
                    while ((j + chunkSize) <= combs[i].Count)
                    {
                        var newCombination = combs[i].GetRange(j, chunkSize);
                        if (combs[i].Count > newCombination.Count)
                        {
                            splitCombinations.Add(newCombination);
                        }
                        chunkSize++;
                    }
                }
            }
        }
        return splitCombinations.ToArray();
    }

    /*
    Splits same value combinations into all possible minimum combinations of 3 cards
    would be a good improvement to implement variable minimum combination of cards num.
    */
    private List<int>[] SplitSameValueCombinations(List<int>[] combs)
    {
        List<List<int>> splitCombinations = new List<List<int>>();
        for (int i = 0; i < combs.Length; i++)
        {
            if (combs[i].Count > 3)
            {
                for (int j = 0; j < combs[i].Count; j++)
                {
                    var tempComb = new List<int>(combs[i]);
                    tempComb.RemoveAt(j);
                    splitCombinations.Add(tempComb);
                }
            }
        }
        return splitCombinations.ToArray();
    }

    private int GetCardsValue(List<CardDisplay> cards, CardSettingsSO settings)
    {
        int totalValue = 0;
        foreach (var card in cards)
        {
            totalValue += settings.GetCardValue(card.Card.Rank);
        }
        return totalValue;
    }
    private int GetCombinationsValue(List<List<int>> combinations, List<CardDisplay> cards, CardSettingsSO settings)
    {
        int totalValue = 0;
        foreach (var combination in combinations)
        {
            foreach (var cardId in combination)
            {
                totalValue += settings.GetCardValue(cards[cardId].Card.Rank);
            }
        }
        return totalValue;
    }

    private int GetCombinationValue(List<int> combination, List<CardDisplay> cards, CardSettingsSO settings)
    {
        int totalValue = 0;
        foreach (var cardId in combination)
        {
            totalValue += settings.GetCardValue(cards[cardId].Card.Rank);
        }
        return totalValue;
    }
}