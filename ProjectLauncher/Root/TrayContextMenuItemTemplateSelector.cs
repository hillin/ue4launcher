using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace UE4Launcher.Root
{
    class TrayContextMenuItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TitleTemplate { get; set; }
        public DataTemplate DynamicItemTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is string)
                return this.TitleTemplate;

            if (item is ITrayContextMenuItem)
                return this.DynamicItemTemplate;

            return null;
        }
    }
}
