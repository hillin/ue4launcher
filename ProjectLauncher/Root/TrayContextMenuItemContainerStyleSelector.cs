using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UE4Launcher.Root
{
    class TrayContextMenuItemContainerStyleSelector : StyleSelector
    {
        public Style TitleStyle { get; set; }
        public Style DynamicItemStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is string)
                return this.TitleStyle;

            if (item is ITrayContextMenuItem)
                return this.DynamicItemStyle;

            return null;
        }

    }
}
