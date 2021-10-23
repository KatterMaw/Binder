using System.Windows.Controls;
using System.Windows.Shapes;
using MahApps.Metro.IconPacks;

namespace Binder.UI
{
    class Menu : ViewModel
    {
        public string Label { get; init; }
        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(Icon));
            }
        }
        public PackIconMaterial Icon
        {
            get
            {
                if (IsSelected)
                {
                    return _activatedIcon;
                }
                else
                {
                    return _deactivatedIcon;
                }
            }
        }
        public UserControl Content { get; init; }


        public Menu(string label, PackIconMaterialKind activatedIcon, PackIconMaterialKind deactivatedIcon, UserControl content)
        {
            Label = label;
            _activatedIcon = new PackIconMaterial() { Kind = activatedIcon, VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch, Width = 30, Height = 30 };
            _deactivatedIcon = new PackIconMaterial() { Kind = deactivatedIcon, VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch, Width = 30, Height = 30 };
            Content = content;
        }


        
        private PackIconMaterial _activatedIcon;
        private PackIconMaterial _deactivatedIcon;
    }
}