   M S B A M L   `   `   `  ����C  ?NeuroApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:NeuroApp.ViewsNeuroApp.Views  5      -    'NeuroApp.Views.SupportStaticGuideDialog   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      b ^MaterialDesignThemes.Wpf, Version=4.1.0.2556, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      (localclr-namespace:NeuroApp.Views  5      .��5      �� 
�������  �� CornerRadius$  8��5      $��1q�5      ���#E0E0E0._�5   
   �  � RowDefinitions 5      �� 5      $
��Auto �6    �� 5      $��* �6    .+�5      � $��0��6   ���#2196F3 � ColumnDefinitions 5      �� 5      $��* �6'   �� 5      $
��Auto �6'   .+�5      �� $��0��6   $��24,16q�5       �� Orientation$ Vertical=�5      .+�5      �� $��20(�5       $��SemiBold"�5!      
���������5       �� *Title+5"      �� 5#      $��Guia de Instruções��6    $��14(�5$      ���#E3F2FD5%      5&      �� 5(      $��1��6      MaterialDesignFlatButton#��[  5)      $��56��5*      $��56��5+      $��0q�5,      )  #MaterialDesignThemes.Wpf.DialogHost   CloseDialogCommand#��Z 5-      .��5.      '  !MaterialDesignThemes.Wpf.PackIcon     Kind$ Close=�6.   $��24��5/   ,   $��24��50   ,   
�������51   ,   52      53      � 55      $��1��6    56      �� 57      $
��Auto �6'   �� 58      $��* �6'   .+�5;      �� $��0��6   $	��250��5<      $��16q�5=      $��0q�5@      
������ ��5>      �� *Instructions+ �� SelectedItem 5?      �� *SelectedInstruction+��5B      �� 5C      .R�5D      �� $
��8,12q�5E   &   $��14(�5F   &   ��5D   (   �� *Title+5F   &   5G      ��5I      �� 5J       �� 
TargetType" ��6      MaterialDesignListBoxItem �� BasedOn# [ 69   .�5K      �� 	 �� Property	 � ��
 �� Value$	
 8,0q�68   �� 5L      	 � ��$	
 0,4q�67   �� 5M      	 � ��

 ���� 6;   5N      5P      �� 5R      $��1��6   $��
0,16,16,16q�5S      $
��Auto=�5T      .��5U      �� .+�5V      �� $��18(�5W   "   $��Medium"�5X   "   ���#424242$
��Wrap=�5Z   "   $��0,0,0,16q�5[   "   ��5V   $   �� *SelectedInstruction.Title+5[   "   �� 5]      ��6'   �� *SelectedInstructionSteps+��5^      �� 5_   "   .R�5`   &   #  MaterialDesignThemes.Wpf.Card  $	��0,8q�6:   $��16q�5a   8   ���#F5F5F5.��5c   *   �� $
��Wrap=�5d   2   $��14(�5e   2   ��5c   4   �� 5e   2   5f   '   5g   #   5i      �� 5k      $6��0Selecione uma instrução para ver seu conteúdo��6$   $
��Wrap=�5l   "   $��14(�5m   "   $��Center=�5n   "   $��0,16,0,0q�5o   "   ���#757575y�5q   "   �� *ShowEmptyMessage+   BooleanToVisibilityConverter �� 	Converter# [ 5r      5s      5t      5u      5v      5w      