   M S B A M L   `   `   `  ���� C  ?NeuroApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:NeuroApp.ClassesNeuroApp.Classes      NeuroApp.WarrantyScreen   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     b ^MaterialDesignThemes.Wpf, Version=4.1.0.2556, Culture=neutral, PublicKeyToken=df2a72020bd7962a;mat1http://materialdesigninxaml.net/winfx/xaml/themes  ,classesclr-namespace:NeuroApp.Classes    � 	Resources  5   /NeuroApp.Classes.WarrantyRemainingDaysConverter "  WarrantyRemainingDaysConverter  ContextMenuAnimation �� TargetProperty W� From	 W� To g� Duration��%Z   &	        &	        �� .��W� $ (UIElement.Opacity)� 0 1$ 0:0:0.3D�.��� ����  �� 
StartPoint$	 0,0&� �� EndPoint$	 1,1&�.���  � Color$ #F0F4F8�� � Offset$ 0Q�� $ #E8ECEF��$ 1Q�	 � RowDefinitions	 �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $��* �.+�� $��0��  MaterialDesignPaper#���  
 � ColumnDefinitions
 �� $
��Auto ��� $��* ��� $
��Auto �.+��� -   $��0��  MaterialDesignFlatButton#��[ $��56��$
��16,0q�$��0,0,0,0q� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect     
BlurRadius$ 4Q�   ShadowDepth$ 1Q�   Opacity$	 0.2Q�.����  �� Orientation$ 
Horizontal=�.+�'  !MaterialDesignThemes.Wpf.PackIcon     Kind$ 	ArrowLeft=�$��24��$��24��$��Center=��� $��Voltar��$��8,0,0,0q�$��Center=��� $��1��$��	Garantias��$��35(�$��Center=� $  MaterialDesignHeadline6TextBlock#��[ $��Center=��� -   $��2��  MaterialDesignRaisedButton#��[ $��36��$��0,0,16,0q�.���� $ 
Horizontal=�.+�  $
 Plus=�$��20��$��20��$��Center=��� $��Nova Garantia��$��8,0,0,0q�$��Center=��� $��1�����	#FF0078D7$��25��$��0,4,0,0q�   $ 10Q�$ 2Q�$	 0.3Q��� $��2��$��	0 10 0 10q�.+��� -    ��Name$ 	SearchBar��$��Center=�$��13(�$��35��$��10 5 0 5q�$��	15 0 15 0q�&   System.Windows.Controls.DataGrid  -     Name$ WarrantyDataGrid��$��3��   AutoGenerateColumns$ False��  MaterialDesignDataGrid#���    MaterialDesignBackground   RowBackground# �     AlternatingRowBackground �#F5F5F5   CanUserAddRows$ False��   CanUserDeleteRows$ False��   CanUserResizeColumns$ False��   CanUserResizeRows$ False��   CanUserSortColumns$
 True��$��10q�$
��Auto=�$
��Auto=����� *
Warranties+  =  7System.Windows.Controls.Primitives.DataGridColumnHeader �� 
TargetType �� Property �� Value*  $System.Windows.Controls.DataGridCell �� Triggers �� Binding  �� RelativeSource! �� AncestorType,  &System.Windows.Controls.DataGridColumn" �� Path# �� Value$ P� Property% P� Value��%�  '        '      '�� �    '�� �    �� "  .���  � ��$ 18(���  � ��$ Segoe UI)���  � ��$ SemiBold"���  � �� �	#FF0078D7��  � ��
 ���� ��  � ��$
 Auto����  � ��$ Center=���  � ��$ 10 10q���  � ��$ 0,0,1,0q���  � �� �#E0E0E0�� "  .���  � ��$ 10q���  � ��$ 16(���  � ��$ 0,0,1,0q���  � �� �#E0E0E0 ��  ��   �� "!  $" DisplayIndex�# 4.a���  � ��$ 0q��� " ��.���  � ��$ 20����  � ��$ 0,0,0,0q���  � ��
 ���� ��  � ��$ 0q� P� $ �@��Orientation$% 
Horizontal=�. ���  � ��$ 20���� " ��.���  � ��$
 Auto=���  � ��$
 Auto=�&   RowStyle& �� )  #System.Windows.Controls.DataGridRow"  .���  � ��$
 Auto����  � ��$ 50�� P� $ �@J�IsMouseOver$
% True��. ���  � �� �#F5F5F5P� $ �@ 
IsSelected$
% True��. ���  � �� �#E3F2FD]� ' ]� 
Conditions' �� ( �� Property( �@ 
IsSelected) �� Value$
) True���� ( �@J�IsMouseOver$
) True��./���  � �� �#BBDEFB*   Columns* 0	  *System.Windows.Controls.DataGridTextColumn	  +   Header$+ Cliente��,   Width5
  /System.Windows.Controls.DataGridLengthConverter$, *
 -   
IsReadOnly$
- True��1  +System.Windows.Controls.DataGridBoundColumn.   Binding. �� *
Customer+/   ElementStyle/ �� " ��.���  � ��$ 16(���  � ��$
 Wrap=���  � ��$ Center=���  � ��$ Center=�	  $+ 
Nº Série��$, *
 $
- True��. �� *SerialNumber+/ �� " ��.���  � ��$ 16(���  � ��$
 Wrap=���  � ��$ Center=���  � ��$ Center=�	  $+ Data Serviço��$, *
 $
- True��. �� *ServiceDate+0 �� StringFormat$0 
dd/MM/yyyy��/ �� " ��.���  � ��$ 16(���  � ��$
 Wrap=���  � ��$ Center=���  � ��$ Center=�	  $+ 
Vencimento��$, *
 $
- True��. �� *WarrantyEndDate+$0 
dd/MM/yyyy��/ �� " ��.���  � ��$ 16(���  � ��$
 Wrap=���  � ��$ Center=���  � ��$ Center=�	  $+ Dias Restantes��$, *
 $
- True��. �� $" .�1 �� 	Converter#1 [  / �� " ��.���  � ��$ 16(���  � ��$
 Wrap=���  � ��$ Center=���  � ��$ Center=�2 � ContextMenu2 �� 
����������#E0E0E0$��1q�3 � Triggers3 :� 4 :� RoutedEvent$4 ContextMenu.Opened��.[��� #��[   ��%�   'm�       �� " m�.���  � ��$ 20, 10q���  � ��$ 14(���  � ��$ 40����  � ��
 ����  P� $ �@J�IsMouseOver$
% True��. ���  � �� �#F5F5F5.@�m� -   $��Remover Garantia��$��Segoe UI)����	#FF0078D7m� -   $��Editar Garantia��$��Segoe UI)����	#FF0078D7� -   5 �Name$5 LoadingOverlay��$��2�����	#80000000$y�	Collapsed=�	 �� $��* ��� $
��Auto ��� $��* �.+�� $��1�� % !MaterialDesignCircularProgressBar#��[ 6 � Value$6 0Q�7 � IsIndeterminate$
7 True��$��50��$��50���� $��1��$��Carregando...��
�������$��16(�$��0,60,0,0q�$��Center=�