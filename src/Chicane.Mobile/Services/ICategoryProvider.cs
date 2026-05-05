using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public interface ICategoryProvider
{
    MotorsportCategory CurrentCategory { get; }
    event Action<MotorsportCategory>? CategoryChanged;
    void SetCategory(MotorsportCategory category);
}
