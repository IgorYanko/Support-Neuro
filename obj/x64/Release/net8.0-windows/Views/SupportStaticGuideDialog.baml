   M S B A M L   `   `   `  ���� C  ?NeuroApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:NeuroApp.ViewsNeuroApp.Views  -    'NeuroApp.Views.SupportStaticGuideDialog   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     b ^MaterialDesignThemes.Wpf, Version=4.1.0.2556, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  (localclr-namespace:NeuroApp.Views  .���� 
�������  �� CornerRadius$  8��$��1q����#E0E0E0._��  � RowDefinitions �� $
��Auto ��� $��* �.+�� $��0�����#2196F3 � ColumnDefinitions �� $��* ��� $
��Auto �.+��� $��0��$��24,16q� �� Orientation$ Vertical=�.+��� $��20(�$��SemiBold"�
����������� *Title+�� $��Guia de Instruções��$��14(����#E3F2FD�� $��1��   MaterialDesignFlatButton#��[  $��56��$��56��$��0q�)  #MaterialDesignThemes.Wpf.DialogHost   CloseDialogCommand#��Z .��'  !MaterialDesignThemes.Wpf.PackIcon     Kind$ Close=�$��24��$��24��
�������� $��1�� �� $
��Auto ��� $��* �.+��� $��0��$	��250��$��16q�$��0q�
������ ���� *Instructions+ �� SelectedItem �� *SelectedInstruction+���� .R��� $
��8,12q�$��14(����� *Title+����  �� 
TargetType" ��  MaterialDesignListBoxItem �� BasedOn# [ .��� 	 �� Property	 � ��
 �� Value$	
 8,0q��� 	 � ��$	
 0,4q��� 	 � ��

 ���� �� $��1��$��
0,16,16,16q�$
��Auto=�.���� .+��� $��18(�$��Medium"����#424242$
��Wrap=�$��0,0,0,16q����� *SelectedInstruction.Title+�� ���� *SelectedInstructionSteps+���� .R�#  MaterialDesignThemes.Wpf.Card  $	��0,8q�$��16q����#F5F5F5.���� $
��Wrap=�$��14(����� �� $6��0Selecione uma instrução para ver seu conteúdo��$
��Wrap=�$��14(�$��Center=�$��0,16,0,0q����#757575y��� *ShowEmptyMessage+   BooleanToVisibilityConverter �� 	Converter# [ 