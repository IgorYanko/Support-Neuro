   M S B A M L   `   `   `  ���� C  ?NeuroApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:NeuroAppNeuroApp  !    NeuroApp.SupportGuideDialog   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     b ^MaterialDesignThemes.Wpf, Version=4.1.0.2556, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  "localclr-namespace:NeuroApp  .���� 
�������  �� CornerRadius$  8��$��1q����#E0E0E0._��  � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+�� $��0�����#2196F3 � ColumnDefinitions �� $��* ��� $
��Auto �.+��� $��0��$��24,16q� �� Orientation$ Vertical=�.+��� $��20(�$��SemiBold"�
����������� *Title+�� $$��Guia de Solução de Problemas��$��14(����#E3F2FD�� $��1��   MaterialDesignFlatButton#��[  $��56��$��56��$��0q�)  #MaterialDesignThemes.Wpf.DialogHost   CloseDialogCommand#��Z .��'  !MaterialDesignThemes.Wpf.PackIcon     Kind$ Close=�$��24��$��24��
��������� $��1��$
��Auto=�$��24q�.���� .+��� $��18(�$��Medium"����#424242$
��Wrap=�$��0,0,0,24q����� *CurrentQuestion+�� $ 
Horizontal=�$��Center=�$��0,0,0,24q�y��� *ShowOptions+   BooleanToVisibilityConverter �� 	Converter# [ .+���   MaterialDesignRaisedButton#��[ $	��120��$��40��$	��8,0q����� *
YesCommand+.���� $ 
Horizontal=�.+�  $ Check=�$��20��$��20��$��0,0,8,0q��� $	��Sim���� #��[ $	��120��$��40��$	��8,0q����� *	NoCommand+.���� $ 
Horizontal=�.+�  $ Close=�$��20��$��20��$��0,0,8,0q��� $
��Não���� ���� *	Solutions+y��� *ShowSolutions+# [ ���� .R�#  MaterialDesignThemes.Wpf.Card  $	��0,8q�$��16q����#F5F5F5.���� $
��Wrap=����� � $��2�����#F5F5F5.+��� #��[  $
��Left=����� *RestartCommand+.���� $ 
Horizontal=�.+�  $ Restart=�$��20��$��20��$��0,0,8,0q��� $��
Recomeçar��