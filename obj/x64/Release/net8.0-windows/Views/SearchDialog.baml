   M S B A M L   `   `   `  ���� C  ?NeuroApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:NeuroApp.ViewsNeuroApp.Views  !    NeuroApp.Views.SearchDialog   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     b ^MaterialDesignThemes.Wpf, Version=4.1.0.2556, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  (localclr-namespace:NeuroApp.Views    � WindowStyle$
  None=� � AllowsTransparency$
 True�� � Title$ SearchDialog��$	��480��$	��390����Z�  � 	Resources    	BoolToVis��%   &	        �� .��#  MaterialDesignThemes.Wpf.Card  -      UniformCornerRadius$ 15Q�  MaterialDesignPaper#���  $��25q�+  %MaterialDesignThemes.Wpf.ShadowAssist   ShadowDepth$ Depth4=�.��)  #MaterialDesignThemes.Wpf.DialogHost  -     Name$ 
DialogHost��   CloseOnClickAway$ False��.���  � RowDefinitions �� $
��Auto ��� $
��Auto ��� $��* �.+�]� $��0��$	��Top=�.+��� $��Pesquisar protocolos��$��DemiBold"�$��18(�$
��Left=� $  MaterialDesignHeadline6TextBlock#��[ $��	15 20 0 0q��� -   	 ��Name$	 BtnExit��$��Right=�$��	0 10 15 0q�$��35��$��35��  RoundedButton#��[ +  %MaterialDesignThemes.Wpf.ButtonAssist
   CornerRadius$
 50��.��'  !MaterialDesignThemes.Wpf.PackIcon     Kind$ Close=�$��Center=�$��Center=���  �� Orientation$ 
Horizontal=�$��1��$��0 25 0 0q�.+��� -    ��Name$ 	SearchBar��$��40��$	��260��$��10q� ! MaterialDesignOutlinedTextBox#��[ $��40 0 0 0q�  $ Search=�$��Center=�$��	-40 0 0 0q�$��30��$��30���� $��2��$
��Auto=�.���� -    ��Name$ ProtocolsItemControl��$��Stretch=�$��15q����� *	Protocols+���� .R��� 
�������$��1q� �� CornerRadius$ 5��$��0 5 0 5q� �� Padding$ 10q�$	��260��._���  ��Name$ ProtocolsPanel��$ Vertical=�.+��� $
��Bold"����� *
Customer+�� $��Italic$�$��0 5 0 0q����� *ProtocolCode+�� $��Italic$�$��0 5 0 5q����� *Title+��  ��Name$ SerialNumberTextblock��y��� *
IsExpanded+ �� 	Converter# [  ���� *SerialNumber+�� $ DeviceTextblock��y��� *
IsExpanded+# [  ���� *Device+�� $ DescriptionTextblock��$
��Wrap=�y��� *
IsExpanded+# [  ���� *Description+�� $ 
Horizontal=�$��0 10 0 0q�$��Right=�.+��� -   $��0 0 5 0q�.��  $ Trash=��� -   $��0 0 5 0q�.��  $ ArrowExpandAll=�