﻿namespace DishesRandomizer.WebAssembly.Components;

using Common;
using Microsoft.AspNetCore.Components;
using Models;
using MoreLinq;

public partial class RandomizerTool {
    private readonly Dice _dice = new();
    private readonly IList<RandomizerCard> _cards = new List<RandomizerCard>();

    private bool? _randomize = true;

    [Inject] private PlannedDishes PlannedDishes { get; set; } = default!;

    private RandomizerCard CardRef {
        set {
            if (_cards.Contains(value)) return;
            _cards.Add(value);
            value.RandomizeChanged.Subscribe(_ => OnCheckboxUpdated());
        }
    }

    protected override void OnInitialized() {
        _dice.OnRotationChanged.Subscribe(_ => StateHasChanged());
    }

    private void UpdateAllCheckboxes(bool? randomizeAll) {
        if (randomizeAll is not { } value) {
            return;
        }

        _cards.ForEach(c => c.Randomize = value);
    }

    private void OnCheckboxUpdated() {
        if (_cards.All(c => c.Randomize)) {
            _randomize = true;
        } else if (_cards.All(c => !c.Randomize)) {
            _randomize = false;
        } else {
            _randomize = null;
        }

        StateHasChanged();
    }

    private void ShuffleAll() {
        var cards = _cards.Where(c => c.Randomize).ToList();
        PlannedDishes.Shuffle(cards.Select(c => c.Day).ToArray());
        _dice.Shuffle();
        cards.Select(c => c.Dice).ForEach(d => d.Shuffle());
    }
}