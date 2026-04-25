using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using AvaloniaDemo.Models;

namespace AvaloniaDemo.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ComponentNode? _selectedNode;

    public ObservableCollection<ComponentNode> RootNodes { get; } = new();

    public string SelectedNodeInfo =>
        SelectedNode is null
            ? "No item selected."
            : $"Selected: {SelectedNode.Name}\nEnabled: {SelectedNode.IsChecked}";

    public MainWindowViewModel()
    {
        var moduleA = new ComponentNode("Module A");
        moduleA.Children.Add(new ComponentNode("Sub-component A1"));
        moduleA.Children.Add(new ComponentNode("Sub-component A2", false));

        var moduleB = new ComponentNode("Module B");
        moduleB.Children.Add(new ComponentNode("Sub-component B1"));

        var moduleC = new ComponentNode("Module C");
        moduleC.Children.Add(new ComponentNode("Sub-component C1"));
        moduleC.Children.Add(new ComponentNode("Sub-component C2"));
        moduleC.Children.Add(new ComponentNode("Sub-component C3", false));

        RootNodes.Add(moduleA);
        RootNodes.Add(moduleB);
        RootNodes.Add(moduleC);
    }

    partial void OnSelectedNodeChanged(ComponentNode? value)
    {
        OnPropertyChanged(nameof(SelectedNodeInfo));
    }
}

