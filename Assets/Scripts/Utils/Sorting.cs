using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sorting
{
    public static List<int>[] SubsequentSort(List<CardDisplay> cardsToSort, SettingsSO settings, bool AllPossibleCombinations = false)
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

    public static List<int>[] SameValueSort(List<CardDisplay> cardsToSort, SettingsSO settings, bool allPossibleCombinations = false)
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
                        // do not skip already checked cards if we need to return all possible combinations
                        if (!allPossibleCombinations)
                        {
                            i = j - 1;
                        }
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
                // do not skip already checked cards if we need to return all possible combinations
                if (!allPossibleCombinations)
                {
                    // when reached the end of loop = jump to the last element to skip unnecessary loops
                    i = j - 1;
                }
            }
        }
        // remove empty combination lists
        return sortedCardsIds.Where(combList => combList.Count != 0).ToArray();
    }

    public static List<int>[] SmartSorting(List<CardDisplay> cardsToSort, SettingsSO settings)
    {
        // save suit settings
        var cardSettings = settings.cardSettings;
        // calculate max possible number of successful card combinations;
        var maxSortedCardGroups = settings.CardsDealNum / settings.MinLegalCardsCombNum;

        // get combinations from 123 sorting and 777 sorting
        var subsequentValueCombs = SubsequentSort(cardsToSort, settings, true);
        for (int i = 0; i < subsequentValueCombs.Length; i++)
        {
            for (int j = 0; j < subsequentValueCombs[i].Count; j++)
            {
                Debug.Log(cardsToSort[subsequentValueCombs[i][j]].name);
            }
            Debug.Log("-----/------");

        }
        Debug.Log("=======================");
        var sameValueCombs = SameValueSort(cardsToSort, settings, true);
        for (int i = 0; i < sameValueCombs.Length; i++)
        {
            for (int j = 0; j < sameValueCombs[i].Count; j++)
            {
                Debug.Log(cardsToSort[sameValueCombs[i][j]].name);
            }
            Debug.Log("-----/------");
        }
        // combine them together in allCombs array of lists
        List<int>[] allCombs = new List<int>[subsequentValueCombs.Length + sameValueCombs.Length];
        subsequentValueCombs.CopyTo(allCombs, 0);
        sameValueCombs.CopyTo(allCombs, subsequentValueCombs.Length);

        // most optimal number of combinations. list element points to combination in allCombs
        List<int> bestCombIds = new List<int>();
        int bestCombScore = 0;
        for (int i = 0; i < allCombs.Length; i++)
        {
            List<int> currentCombs = new List<int>();
            // current best score keeps current list of combinations score
            int currentBestCombScore = GetCombinationValue(allCombs[i], cardsToSort, settings.cardSettings);
            currentCombs.Add(i);
            for (int j = 0; j < allCombs.Length; j++)
            {
                // if (j != i)
                // {
                //     bool hasSameElements = allCombs[i].Intersect(allCombs[j]).Any();
                //     if (hasSameElements)
                //     {
                //         continue;
                //     }
                //     else
                //     {
                //         currentBestCombScore += GetCombinationValue(allCombs[j], cardsToSort, settings.cardSettings);
                //         currentCombs.Add(j);
                //     }
                //     Debug.Log(currentBestCombScore);
                //     if (currentBestCombScore > bestCombScore)
                //     {
                //         bestCombScore = currentBestCombScore;

                //         bestCombIds = currentCombs;
                //     }
                // }
            }
        }
        List<int>[] mostOptimalCombsIds = new List<int>[bestCombIds.Count].Select(item => new List<int>()).ToArray();
        for (int i = 0; i < bestCombIds.Count; i++)
        {
            mostOptimalCombsIds[i] = allCombs[bestCombIds[i]];
        }
        return mostOptimalCombsIds;
    }

    private static int GetCombinationValue(List<int> combination, List<CardDisplay> cards, CardSettingsSO settings)
    {
        int totalValue = 0;
        foreach (var cardId in combination)
        {
            totalValue += settings.GetCardValue(cards[cardId].Card.Rank);
        }
        return totalValue;
    }
}