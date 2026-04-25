using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaDemo.Models;

public partial class ComponentNode : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private bool _isChecked;

    [ObservableProperty]
    private bool _isSelected;

    public ObservableCollection<ComponentNode> Children { get; } = new();

    public ComponentNode(string name, bool isChecked = true)
    {
        Name = name;
        IsChecked = isChecked;
    }
}
