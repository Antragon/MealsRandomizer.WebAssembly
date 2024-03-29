﻿namespace MealsRandomizer.WebAssembly.Components;

using Controllers;
using Microsoft.AspNetCore.Components;

public partial class CookbookComponent {
    [Inject] private CookbookProvider CookbookProvider { get; set; } = null!;
    
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;

    private CookbookController? _cookbookController;

    protected override async Task OnInitializedAsync() {
        _cookbookController = await CookbookProvider.Get();
    }
}