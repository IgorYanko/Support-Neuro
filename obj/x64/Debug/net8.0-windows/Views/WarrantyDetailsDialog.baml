   M S B A M L   `   `   `  ����C  ?NeuroApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:NeuroAppNeuroApp  5   	   $    NeuroApp.WarrantyDetailsDialog   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   b ^MaterialDesignThemes.Wpf, Version=4.1.0.2556, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	   "localclr-namespace:NeuroApp  5   	     � Title$  Detalhes da Garantia��5	   	   $	��480��5
   	   $	��380��5   	    � WindowStyle$
 None=�5   	    � AllowsTransparency$
 True��5   	    � 
ResizeMode$ NoResize=�5   	    � WindowStartupLocation$ CenterOwner=�5   	      MaterialDesignBody#���   5   	   $��Regular"�5   	   $��13(�5   	   ��5   	   Z� ��5   	   :  4MaterialDesignThemes.Wpf.MaterialDesignFontExtension   � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $e _pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml?�6%   �� 5      $h bpack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml?�6%   �� 5      $t npack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Blue.xaml?�6%   �� 5      $s mpack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Accent/MaterialDesignColor.Lime.xaml?�6%   %    5      .��5       #  MaterialDesignThemes.Wpf.Card  -   5           UniformCornerRadius$ 15Q�5         MaterialDesignPaper#���  63   $��25q�5!      +  %MaterialDesignThemes.Wpf.ShadowAssist	   ShadowDepth$	 Depth4=�6U   .��5"   
   )  #MaterialDesignThemes.Wpf.DialogHost  -   5        
  Name$
 
DialogHost��5"   =      CloseOnClickAway$ False��6$   .��5#      �� .+�5$      ]� $	��Top=�6   .+�5%      �� $��Detalhes da Garantia��6    $��DemiBold"�5&       $��18(�5'       $
��Left=�5(        $  MaterialDesignHeadline6TextBlock#��[ 5)       $��	15,20,0,0q�5*       �� 5,      -   5         ��Name$ BtnExit��5-   K   $��Right=�5,      $��	0 10 15 0q�69   $��35��6L   $��35��6X     RoundedButton#��[ 6c   +  %MaterialDesignThemes.Wpf.ButtonAssist   CornerRadius$ 50��5-      .��5.      '  !MaterialDesignThemes.Wpf.PackIcon     Kind$ Close=�62   $��Center=�6?   $��Center=�6\   5/      50      � 52      $��16q�6    � RowDefinitions 53      �� 54      $
��Auto �6(   �� 55      $��* �6(   �� 56      $
��Auto �6(   .+�59      �� $��1��6#   $
��Auto=�5:      $��5 0 5 10q�5;      $	��300��5<      .��5=      �� .+�5>      �� -   5         ��Name$ ClientNameTextBox��5>   &   )  #MaterialDesignThemes.Wpf.HintAssist   Hint$ Nome do Cliente *��5?   &    ! MaterialDesignOutlinedTextBox#��[ 5@   &   $
��10 8q�5A   &   �� 5C      -   5        $ OsCodeTextBox��5C   &   $ Nº Os��5D   &   #��[ 5F   &   $
��10 8q�5G   &   �� 5I      -   5        $ SerialNumberTextBox��5I   &   $ Número de Série *��5J   &   #��[ 5K   &   $
��10 8q�5L   &   �� 5N      -   5         ��Name$ DeviceTextBox��5N   '   $ 
Aparelho *��5O   '    " MaterialDesignOutlinedComboBox#��[ 5P   '   $
��10 8q�5Q   '   .@�5R   "   �� $��Video Frenzel��6/   �� 5S   "   $��VD 360��6/   �� 5T   "   $��Sivec Plus Vx��6/   �� 5U   "   $��Sivec Plus Ex��6/   �� 5V   "   $��OTO VM��6/   �� 5W   "   $��OAT 10 Plus��6/   �� 5X   "   $��OTO New��6/   �� 5Y   "   $��OTO 3D��6/   �� 5Z   "   $��Vecwin Plus��6/   �� 5[   "   $
��Veng��6/   �� 5\   "   $��
Vecwin STM��6/   �� 5]   "   $��Vecto VM��6/   5^      (  "System.Windows.Controls.DatePicker  5`      -   5          Name$ ServiceDatePicker��5`   )   $ Data do Serviço *��5a   )    $  MaterialDesignOutlinedDatePicker#��[ 5b   )   $
��10 8q�5c   )   �� 5e      -	   5        $ WarrantyTypeComboBox��5e   '   $ Tipo de Garantia *��5f   '   #��[ 5g   '   $
��10 8q�5h   '   .@�5j   "   �� $��Serviço��6/   �� 5k   "   $��Calibração��6/   �� 5l   "   $��Personalizada��6/   5m      �� 5o      -
   5        $ WarrantyMonthsTextBox��5o   &   $" Período de Garantia (meses)��5p   &   #��[ 5q   &   $
��10 8q�5r   &    �� 
IsReadOnly$
 True��5t   &   �� 5v      -   5        $ ObservationsTextBox��5v   &   $ Descrição do serviço *��5w   &   #��[ 5x   &    �� TextWrapping$
 Wrap=�5y   &    �� AcceptsReturn$
 True��5z   &    �� 	IsEnabled$
 True��5{   &   " �� VerticalScrollBarVisibility$
 Auto=�5|   &   $��80��5}   &   $
��10 8q�5~   &   5      5�      �� 5�      $��2��6!    �� Orientation$ 
Horizontal=�5�   !   $��Right=�5�   !   $��0 10 0 0q�5�   !   .+�5�      �� -   5          MaterialDesignFlatButton#��[ 5�   !   ���	#FF0078D7$��0 0 8 0q�5�   !   .��62   
Cancelar5�      �� 5�      -   5          MaterialDesignRaisedButton#��[ 5�   !   ���	#FF0078D7.��5�   8   Salvar5�      5�      5�      5�      5�      5�      5�      