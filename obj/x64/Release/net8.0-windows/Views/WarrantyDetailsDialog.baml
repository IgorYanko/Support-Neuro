   M S B A M L   `   `   `  ���� C  ?NeuroApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:NeuroAppNeuroApp  $    NeuroApp.WarrantyDetailsDialog   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     b ^MaterialDesignThemes.Wpf, Version=4.1.0.2556, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  "localclr-namespace:NeuroApp    � Title$  Detalhes da Garantia��$	��480��$	��380�� � WindowStyle$
 None=� � AllowsTransparency$
 True�� � 
ResizeMode$ NoResize=� � WindowStartupLocation$ CenterOwner=�   MaterialDesignBody#���   $��Regular"�$��13(���Z� ��:  4MaterialDesignThemes.Wpf.MaterialDesignFontExtension   � 	Resources  �� MergedDictionaries �� Source��  �� $e _pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml?��� $h bpack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml?��� $t npack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Blue.xaml?��� $s mpack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Accent/MaterialDesignColor.Lime.xaml?�%    .��#  MaterialDesignThemes.Wpf.Card  -      UniformCornerRadius$ 15Q�  MaterialDesignPaper#���  $��25q�+  %MaterialDesignThemes.Wpf.ShadowAssist	   ShadowDepth$	 Depth4=�.��)  #MaterialDesignThemes.Wpf.DialogHost  -   
  Name$
 
DialogHost��   CloseOnClickAway$ False��.���� .+�]� $	��Top=�.+��� $��Detalhes da Garantia��$��DemiBold"�$��18(�$
��Left=� $  MaterialDesignHeadline6TextBlock#��[ $��	15,20,0,0q��� -    ��Name$ BtnExit��$��Right=�$��	0 10 15 0q�$��35��$��35��  RoundedButton#��[ +  %MaterialDesignThemes.Wpf.ButtonAssist   CornerRadius$ 50��.��'  !MaterialDesignThemes.Wpf.PackIcon     Kind$ Close=�$��Center=�$��Center=�� $��16q� � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��1��$
��Auto=�$��5 0 5 10q�$	��300��.���� .+��� -    ��Name$ ClientNameTextBox��)  #MaterialDesignThemes.Wpf.HintAssist   Hint$ Nome do Cliente *�� ! MaterialDesignOutlinedTextBox#��[ $
��10 8q��� -   $ OsCodeTextBox��$ Nº Os��#��[ $
��10 8q��� -   $ SerialNumberTextBox��$ Número de Série *��#��[ $
��10 8q��� -    ��Name$ DeviceTextBox��$ 
Aparelho *�� " MaterialDesignOutlinedComboBox#��[ $
��10 8q�.@��� $��Video Frenzel���� $��VD 360���� $��Sivec Plus Vx���� $��Sivec Plus Ex���� $��OTO VM���� $��OAT 10 Plus���� $��OTO New���� $��OTO 3D���� $��Vecwin Plus���� $
��Veng���� $��
Vecwin STM���� $��Vecto VM��(  "System.Windows.Controls.DatePicker  -     Name$ ServiceDatePicker��$ Data do Serviço *�� $  MaterialDesignOutlinedDatePicker#��[ $
��10 8q��� -	   $ WarrantyTypeComboBox��$ Tipo de Garantia *��#��[ $
��10 8q�.@��� $��Serviço���� $��Calibração���� $��Personalizada���� -
   $ WarrantyMonthsTextBox��$" Período de Garantia (meses)��#��[ $
��10 8q� �� 
IsReadOnly$
 True���� -   $ ObservationsTextBox��$ Descrição do serviço *��#��[  �� TextWrapping$
 Wrap=� �� AcceptsReturn$
 True�� �� 	IsEnabled$
 True��" �� VerticalScrollBarVisibility$
 Auto=�$��80��$
��10 8q��� $��2�� �� Orientation$ 
Horizontal=�$��Right=�$��0 10 0 0q�.+��� -     MaterialDesignFlatButton#��[ ���	#FF0078D7$��0 0 8 0q�.��
Cancelar�� -     MaterialDesignRaisedButton#��[ ���	#FF0078D7.��Salvar