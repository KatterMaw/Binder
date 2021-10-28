using System.Windows.Controls;
using System.Windows.Shapes;
using MahApps.Metro.IconPacks;

namespace Binder.UI
{
    class Menu : ViewModel
    {
        private const int IconsSize = 25;

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


        public PackIconControlBase Icon => BootstrapIcon;

        private PackIconMaterial MaterialIcon
        {
            get
            {
                return IsSelected ? _activatedMaterialIcon : _deactivatedMaterialIcon;
            }
        }
        private PackIconBootstrapIcons BootstrapIcon
        {
            get
            {
                return IsSelected ? _activatedBootstrapIcon : _deactivatedBootstrapIcon;
            }
        }


        public UserControl Content { get; init; }


        public Menu(string label, PackIconMaterialKind activatedMaterialIcon, PackIconMaterialKind deactivatedMaterialIcon, PackIconBootstrapIconsKind activatedBootstrapIcon, PackIconBootstrapIconsKind deactivatedBootstrapIcon, UserControl content)
        {
            Label = label;
            _activatedMaterialIcon = new PackIconMaterial() { Kind = activatedMaterialIcon, VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch, HorizontalAlignment = System.Windows.HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center, Width = IconsSize, Height = IconsSize };
            _deactivatedMaterialIcon = new PackIconMaterial() { Kind = deactivatedMaterialIcon, VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch, HorizontalAlignment = System.Windows.HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center, Width = IconsSize, Height = IconsSize };
            _activatedBootstrapIcon = new PackIconBootstrapIcons() { Kind = activatedBootstrapIcon, VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch, HorizontalAlignment = System.Windows.HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center, Width = IconsSize, Height = IconsSize };
            _deactivatedBootstrapIcon = new PackIconBootstrapIcons() { Kind = deactivatedBootstrapIcon, VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch, HorizontalAlignment = System.Windows.HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center, Width = IconsSize, Height = IconsSize };
            Content = content;
        }


        
        private PackIconMaterial _activatedMaterialIcon;
        private PackIconMaterial _deactivatedMaterialIcon;

        private PackIconBootstrapIcons _activatedBootstrapIcon;
        private PackIconBootstrapIcons _deactivatedBootstrapIcon;
    }
}