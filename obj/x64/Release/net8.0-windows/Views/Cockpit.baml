   M S B A M L   `   `   `  ���� C  ?NeuroApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:NeuroApp.ClassesNeuroApp.Classes  3clr-namespace:NeuroApp.ScreensNeuroApp.Screens      NeuroApp.Cockpit   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     2classesclr-namespace:NeuroApp.Classes     .sysclr-namespace:NeuroApp.Screens     b ^MaterialDesignThemes.Wpf, Version=4.1.0.2556, Culture=neutral, PublicKeyToken=df2a72020bd7962a;mat1http://materialdesigninxaml.net/winfx/xaml/themes    � 	Resources  4   .NeuroApp.Classes.RemainingTimeToBrushConverter   TimeToBrushConverter-   'NeuroApp.Classes.PauseButtonTextChanger  PauseButtonTextChanger.   (NeuroApp.Classes.StatusEditableConverter  StatusEditableConverter&    NeuroApp.Classes.StatusConverter  StatusConverter4   .NeuroApp.Classes.StringNotNullOrEmptyConverter ! StringNotNullOrEmptyConverter.  (System.Windows.Media.Animation.CubicEase  MaterialDesignEasing7  1System.Windows.Media.Animation.EasingFunctionBase   
EasingMode��%j   &	        &	      &	 
     &	      &	      &	                  $ 	EaseInOut=�.��� ����  �� 
StartPoint$	 0,0&� �� EndPoint$	 1,1&�.���  � Color$ #F0F4F8�� � Offset$ 0Q�� $ #E8ECEF��$ 1Q� � RowDefinitions �� $
��Auto ��� $
��Auto ��� $��* �.+�� $��0��  MaterialDesignPaper#���   � ColumnDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+��� -   $��0��  MaterialDesignFlatButton#��[ $��56��$
��16,0q�$��0,0,0,0q� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect  	   
BlurRadius$	 4Q�
   ShadowDepth$
 1Q�   Opacity$	 0.2Q�.����  �� Orientation$ 
Horizontal=�.+�'	  !MaterialDesignThemes.Wpf.PackIcon	   	  Kind$ 	ArrowLeft=�$��24��$��24��$��Center=��� $��Voltar��$��8,0,0,0q�$��Center=��� $��1��$��Cockpit��$��35(�$��Center=� $  MaterialDesignHeadline6TextBlock#��[ $��Center=��� -    ��Name$ UpdateDataButton��$��2��$��90��$��	Atualizar������ *UpdateCommand+�� $��1�����	#FF0078D7$��40��$��0,4,0,0q�.+�� .+��� $��Ordens de Serviço��$��25(�$
��Bold"�$��Segoe UI)�
������� �� Padding$ 0 2 0 0q�$��Center=�� $��0 5 0 0q�$��2�����#FFFFFF �� $��* �.+�� -    �Name$ ObservationPopUp�� � 	StaysOpen$
 True�� � AllowsTransparency$
 True��$��Bottom=�$��Slide=� � HorizontalOffset$ 10�� � VerticalOffset$ 5������ *IsPopupOpen+ �� Mode$ TwoWay=�.����  �� CornerRadius$ 10��#���   	 MaterialDesignerDivider#��� 	 $��1q� �� Padding$ 10q� 
 MaterialDesignShadowDepth2# [
 ._�� $	��300��$	��150�� �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$��Observação��$��18(�$��SemiBold"�  MaterialDesignBody#���  $��0 0 0 5q��� -    ��Name$ 	PopupText��$��1��#���  $��14(�$��
0, 0, 0, 5q�$
��Wrap=� �� TextAlignment$
 Left=����� *Observation+ �� FallbackValue$  ���� -   $��2��#��[ $��Fechar��$��Right=�&
   System.Windows.Controls.DataGrid
  -    
 Name$ DataGridSales��$��0�� 
  AutoGenerateColumns$ False��  MaterialDesignDataGrid#���    MaterialDesignBackground 
  RowBackground# �   
  AlternatingRowBackground �#F5F5F5 
  CanUserReorderColumns$ False��  
  CanUserSortColumns$  False��! 
  CanUserResizeColumns$! False��" 
  CanUserResizeRows$" False��# 
  CanUserAddRows$# False��$ J� 	AllowDrop$
$ True��$��	0 10 0 10q�$��Stretch=�$��Stretch=�$
��Auto=�$
��Auto=�% %� IsVirtualizing$% False��
���   �& J� RenderTransformOrigin$& 0.5,0.5&����� *	SalesData+' �� SelectedItem' �� *SelectedSale+    DefaultCellStyle( �� 
TargetType) �� Property* �� Value=  7System.Windows.Controls.Primitives.DataGridColumnHeader*  $System.Windows.Controls.DataGridCell+ �� 	Converter, �� ConverterParameter��%�  &	       '  o     '  �     7   7   �� "( ��.��� ) � ��$* 16(��� ) � ��$
* Wrap=��� ) � ��$* Center=��� ) � ��$* Center=��� "(  .��� ) � ��$* 18(��� ) � ��$* SemiBold"��� ) � ��* �	#FF0078D7�� ) � ��$* Center=��� ) � ��$* 10q��� "(  .��� ) � ��* �� *.+8+   $, 
Background���� ) � ��* �� *.+8+  $, 
Foreground���� ) � ��$* 10q��� ) � ��$* 16(�- 
  RowStyle- �� )  #System.Windows.Controls.DataGridRow"(  .��� ) � ��$
* Auto���� ) � ��$* 50��. �� Triggers. P� / P� Property/ �@J�IsMouseOver0 P� Value$
0 True��. ��� ) � ��* �#F5F5F5P� / �@ 
IsSelected$
0 True��. ��� ) � ��* �#E3F2FD]� 1 ]� 
Conditions1 �� 2 �� Property2 �@ 
IsSelected3 �� Value$
3 True���� 2 �@J�IsMouseOver$
3 True��./��� ) � ��* �#BBDEFB4 
  Columns4 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn5   Header$	5 Obs��6   Width5  /System.Windows.Controls.DataGridLengthConverter$6 60 7   MinWidth$7 60Q�8   CellTemplate8 �� .R��� $ ObservationButton��$��30��$	��5 0q�9 J� IsHitTestVisible$
9 True��$��Center=�$��Center=����� ���� *%#DataContext.ShowObservationsCommand+: �� RelativeSource: �� ; �� AncestorType; UserControl���� "( ��  RoundedButton< �� BasedOn#< [ .��� ) � y�$* 	Collapsed=�. �� = �� Binding= �� *Observation+> �� Value> Z� .a��� ) � y�$* 	Collapsed=��� = �� *Observation+>  .a��� ) � y�$* 	Collapsed=��� = �� *Observation+#+ [ > True.a��� ) � y�$* Visible=�.��	  $ Close=�$��20��$��20��$��Center=�0  *System.Windows.Controls.DataGridTextColumn  $5 Nº OS��$
6 Auto $	7 100Q�?   
IsReadOnly$
? True��1  +System.Windows.Controls.DataGridBoundColumn@   ElementStyle#@ [ A   BindingA �� *Code+  $5 Cliente��$
6 Auto $	7 210Q�$
? True��#@ [ A �� *
PersonName+  $5 Data de Chegada��$
6 Auto $7 90Q�$
? True��#@ [ A �� *DateCreated+B �� StringFormat$B {0:dd/MM/yyyy}��  $
5 Tipo��$
6 Auto $	7 140Q�$
? True��#@ [ A �� *DisplayType+  $5 Status��$
6 Auto $	7 165Q�8 �� .R��� #��[ ���� *DisplayStatus+C   CellEditingTemplateC �� .R��� $��16(�$��Center=�$��Center=�$
��Auto������ *
StatusList+' �� *Status+$ TwoWay=�#+ [   $	5 Tag��$
6 Auto 8 �� .R��� $ 
Horizontal=�$��Center=�.+��� ���� *
MappedTags+���� .R��� $ 5��$	 5,2q�$��2q����� *Color+._��� $��14(����� *Name+���� *	TextColor+D � ContextMenuD �� 
����������#E0E0E0$��1q�  ��%�   'm�       �� "( m�.��� ) � ��$* 20,10q��� ) � ��$* 14(��� ) � ��$* 40���� ) � ��
* ���� . P� / �@J�IsMouseOver$
0 True��. ��� ) � ��* �#F5F5F5.@�m� -   $��Adicionar observação��$��Segoe UI)����	#FF0078D7m� -   $��
Remover OS��$��Segoe UI)����	#FF0078D7m� -	   $��Segoe UI)����	#FF0078D7���� *'%PlacementTarget.SelectedItem.IsPaused+: �� *FindAncestor+"; ��#+ [ � -
   E �Name$E LoadingOverlay��$��2�����	#80000000$y�	Collapsed=� �� $��* ��� $
��Auto ��� $��* �.+�� $��1�� % !MaterialDesignCircularProgressBar#��[ F � Value$F 0Q�G � IsIndeterminate$
G True��$��50��$��50���� $��1��$��Carregando...��
�������$��16(�$��0,60,0,0q�$��Center=�