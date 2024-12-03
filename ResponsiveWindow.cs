using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NeuroApp
{
    public class ResponsiveWindow : Window
    {
            public ResponsiveBigButtons responsiveBigButtons { get; set; }

            public ResponsiveWindow()
            {
                responsiveBigButtons = new ResponsiveBigButtons();
                DataContext = this;

                this.SizeChanged += (s, e) =>
                {
                    responsiveBigButtons.WindowHeight = this.ActualHeight;
                    responsiveBigButtons.WindowWidth = this.ActualWidth;
                };
            }
    }
}
