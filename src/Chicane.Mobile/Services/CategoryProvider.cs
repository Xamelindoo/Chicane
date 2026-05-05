using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public class CategoryProvider : ICategoryProvider
{
    public MotorsportCategory CurrentCategory { get; private set; } = MotorsportCategory.F1;

    public event Action<MotorsportCategory>? CategoryChanged;

    public void SetCategory(MotorsportCategory category)
    {
        if (CurrentCategory.Id == category.Id) return;

        CurrentCategory = category;
        CategoryChanged?.Invoke(category);
    }
}
