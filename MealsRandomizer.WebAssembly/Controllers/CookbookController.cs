﻿namespace MealsRandomizer.WebAssembly.Controllers;

using System.Reactive.Linq;
using System.Reactive.Subjects;
using MoreLinq;

public class CookbookController {
    private readonly Subject<Cookbook> _cookbookSubject = new();
    
    private Cookbook _cookbook;

    public CookbookController(Cookbook cookbook) {
        _cookbook = cookbook;
    }

    public Cookbook Cookbook {
        get => _cookbook;
        set => SetCookbook(value);
    }

    private void SetCookbook(Cookbook value) {
        _cookbook = value;
        _cookbookSubject.OnNext(Cookbook);
    }

    public IObservable<Cookbook> CookbookChanged => _cookbookSubject.AsObservable();

    public void PutMeal(Meal meal) {
        Cookbook.Meals[meal.Id] = meal;
        _cookbookSubject.OnNext(Cookbook);
    }

    public void DeleteMeal(Guid id) {
        Cookbook.Meals.Remove(id);
        var days = Cookbook.PlannedMeals.Where(x => x.Value == id).Select(x => x.Key);
        days.ForEach(day => Cookbook.PlannedMeals.Remove(day));
        _cookbookSubject.OnNext(Cookbook);
    }

    public IEnumerable<Meal> GetMeals() {
        return Cookbook.Meals.Values;
    }

    public void SetPlannedMeal(Day day, Guid id) {
        Cookbook.PlannedMeals[day] = id;
        _cookbookSubject.OnNext(Cookbook);
    }

    public void DeletePlannedMeal(Day day) {
        Cookbook.PlannedMeals.Remove(day);
        _cookbookSubject.OnNext(Cookbook);
    }

    public Meal? GetPlannedMeal(Day day) {
        return Cookbook.PlannedMeals.TryGetValue(day, out var id) ? Cookbook.Meals[id] : null;
    }

    public void ShufflePlannedMeals(params Day[] days) {
        var otherDays = Enum.GetValues<Day>().Except(days).ToList();
        var alreadyPlanned = otherDays.Select(GetPlannedMeal).OfType<Meal>();
        var unplannedMeals = Cookbook.Meals.Values.Except(alreadyPlanned).ToList();
        foreach (var day in days) {
            var mealsSelection = unplannedMeals.Any() ? unplannedMeals : Cookbook.Meals.Values.ToList();
            var selectedMeal = mealsSelection.RandomSubset(1).Single();
            Cookbook.PlannedMeals[day] = selectedMeal.Id;
            unplannedMeals.Remove(selectedMeal);
        }

        _cookbookSubject.OnNext(Cookbook);
    }
}