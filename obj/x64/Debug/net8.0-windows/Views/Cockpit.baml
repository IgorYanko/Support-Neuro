   M S B A M L   `   `   `  ����C  ?NeuroApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:NeuroApp.ClassesNeuroApp.Classes  5      3clr-namespace:NeuroApp.ScreensNeuroApp.Screens  5          NeuroApp.Cockpit   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      2classesclr-namespace:NeuroApp.Classes     5      .sysclr-namespace:NeuroApp.Screens     5      b ^MaterialDesignThemes.Wpf, Version=4.1.0.2556, Culture=neutral, PublicKeyToken=df2a72020bd7962a;mat1http://materialdesigninxaml.net/winfx/xaml/themes  5        � 	Resources  5      4   .NeuroApp.Classes.RemainingTimeToBrushConverter   TimeToBrushConverter-   'NeuroApp.Classes.PauseButtonTextChanger  PauseButtonTextChanger.   (NeuroApp.Classes.StatusEditableConverter  StatusEditableConverter&    NeuroApp.Classes.StatusConverter  StatusConverter4   .NeuroApp.Classes.StringNotNullOrEmptyConverter ! StringNotNullOrEmptyConverter.  (System.Windows.Media.Animation.CubicEase  MaterialDesignEasing7  1System.Windows.Media.Animation.EasingFunctionBase   
EasingMode��%�   &	        &	      &	 &     &	 9     &	 L     &	 _       5   
   60     5   
   6)     5   
   6*     5   
   6"     5   
   60     5   
   $ 	EaseInOut=�61   5      .��5      � ��5   
   �� 5       �� 
StartPoint$	 0,0&�6"    �� EndPoint$	 1,1&�63   .��5      �  � Color$ #F0F4F8��6    � Offset$ 0Q�6/   � 5      $ #E8ECEF��6   $ 1Q�6/   5       � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5       $��* �6   .+�5#   
   � $��0��6     MaterialDesignPaper#���  6    � ColumnDefinitions 5$      �� 5%      $
��Auto �6#   �� 5&      $��* �6#   �� 5'      $
��Auto �6#   .+�5*      �� -   5        $��0��5*        MaterialDesignFlatButton#��[ 5+      $��56��5,      $
��16,0q�5-      $��0,0,0,0q�5.       J� Effect 51      3  -System.Windows.Media.Effects.DropShadowEffect  52      	   
BlurRadius$	 4Q�6'   
   ShadowDepth$
 1Q�66      Opacity$	 0.2Q�6F   .��55      ��  �� Orientation$ 
Horizontal=�6   .+�56      '	  !MaterialDesignThemes.Wpf.PackIcon	   	  Kind$ 	ArrowLeft=�6#   $��24��57       $��24��6+   $��Center=�58       �� 59      $��Voltar��6    $��8,0,0,0q�5:      $��Center=�5;      5<      5=      �� 5?      $��1��6   $��Cockpit��5@      $��35(�5A      $��Center=�5B       $  MaterialDesignHeadline6TextBlock#��[ 5C      $��Center=�5D      �� 5F      -   5         ��Name$ UpdateDataButton��5F      $��2��6/   $��90��6?   $��	Atualizar��6J   ��6^   �� *UpdateCommand+5G      �� 5I   
   $��1��6   ���	#FF0078D7$��40��69   $��0,4,0,0q�6E   .+�5J      � .+�5K      �� $��Ordens de Serviço��6   $��25(�5L      $
��Bold"�5M      $��Segoe UI)�5N      
������� �� Padding$ 0 2 0 0q�5P      $��Center=�5Q      5R      5S      � 5U   
   $��0 5 0 0q�6   $��2��6    ���#FFFFFF 5V      �� 5W      $��* �6#   .+�5Z      � -   5         �Name$ ObservationPopUp��5Z       � 	StaysOpen$
 True��5\       � AllowsTransparency$
 True��5]      $��Bottom=�5^      $��Slide=�5_       � HorizontalOffset$ 10��5`       � VerticalOffset$ 5��5a      ��5[      �� *IsPopupOpen+ �� Mode$ TwoWay=�.��5b      ��  �� CornerRadius$ 10��6   #���  5c       	 MaterialDesignerDivider#��� 	 5d      $��1q�5e       �� Padding$ 10q�5f       
 MaterialDesignShadowDepth2# [
 5g      ._�5h      � $	��300��6   $	��150��6'    5i      �� 5j      $
��Auto �6,   �� 5k      $��* �6,   �� 5l      $
��Auto �6,   .+�5o      �� $��0��6$   $��Observação��5p   $   $��18(�5q   $   $��SemiBold"�5r   $     MaterialDesignBody#���  5s   $   $��0 0 0 5q�5t   $   �� 5v      -   5         ��Name$ 	PopupText��5v   $   $��1��5w   $   #���  5y   $   $��14(�5z   $   $��
0, 0, 0, 5q�5{   $   $
��Wrap=�5|   $    �� TextAlignment$
 Left=�5}   $   ��5x   $   �� *Observation+ �� FallbackValue$  ��5}   $   �� 5      -   5        $��2��5   !   #��[ 5�   !   $��Fechar��5�   !   $��Right=�5�   !   5�   !   5�      5�      5�      &
   System.Windows.Controls.DataGrid
  5�      -   5         
 Name$ DataGridSales��5�      $��0��5�       
  AutoGenerateColumns$ False��5�        MaterialDesignDataGrid#���  5�        MaterialDesignBackground 
  RowBackground# �  5�       
  AlternatingRowBackground �#F5F5F5 
  CanUserReorderColumns$ False��5�        
  CanUserSortColumns$  False��5�      ! 
  CanUserResizeColumns$! False��5�      " 
  CanUserResizeRows$" False��5�      # 
  CanUserAddRows$# False��5�      $ J� 	AllowDrop$
$ True��5�      $��	0 10 0 10q�5�      $��Stretch=�5�      $��Stretch=�5�      $
��Auto=�5�      $
��Auto=�5�      % %� IsVirtualizing$% False��5�      
���   �& J� RenderTransformOrigin$& 0.5,0.5&�5�      ��5�      �� *	SalesData+' �� SelectedItem' 5�      �� *SelectedSale+  5�        DefaultCellStyle( �� 
TargetType) �� Property* �� Value=  7System.Windows.Controls.Primitives.DataGridColumnHeader*  $System.Windows.Controls.DataGridCell+ �� 	Converter, �� ConverterParameter��%�  &	       '  �     '  �    7   7   �� 5�      "( ��65   .�5�      �� ) � ��$* 16(�65   �� 5�      ) � ��$
* Wrap=�69   �� 5�      ) � ��$* Center=�6@   �� 5�      ) � ��$* Center=�6>   5�      �� 5�      "(  6   .�5�      �� ) � ��$* 18(�65   �� 5�      ) � ��$* SemiBold"�67   �� 5�      ) � ��* �	#FF0078D767   �� 5�      ) � ��$* Center=�6G   �� 5�      ) � ��$* 10q�64   5�      �� 5�      "(  6   .�5�      �� ) � ��* 67   �� *.+8+   $, 
Background���� 5�      ) � ��* 67   �� *.+8+  $, 
Foreground���� 5�      ) � ��$* 10q�64   �� 5�      ) � ��$* 16(�65   5�      5�      - 
  RowStyle- 5�      �� 5�      )  #System.Windows.Controls.DataGridRow"(  6   .�5�      �� ) � ��$
* Auto��63   �� 5�      ) � ��$* 50��66   . �� Triggers. 5�      P� 5�      / P� Property/ �@J�IsMouseOver0 P� Value$
0 True��6=   . �5�   "   �� ) � ��* �#F5F5F56?   5�      P� 5�      / �@ 
IsSelected$
0 True��6<   . �5�   "   �� ) � ��* �#E3F2FD6?   5�      ]� 5�      1 ]� 
Conditions1 5�   "   �� 5�   &   2 �� Property2 �@ 
IsSelected3 �� Value$
3 True��6F   �� 5�   &   2 �@J�IsMouseOver$
3 True��6G   ./�5�   "   �� ) � ��* �#BBDEFB6?   5�      5�      4 
  Columns4 5�      4  .System.Windows.Controls.DataGridTemplateColumn  5�      ,  &System.Windows.Controls.DataGridColumn5   Header$	5 Obs��6-   6   Width5  /System.Windows.Controls.DataGridLengthConverter$6 60 6:   7   MinWidth$7 60Q�6E   8   CellTemplate8 5�      �� 5�      .R�5�   "   �� $ ObservationButton��6)   $��30��5�   )   $	��5 0q�5�   )   9 J� IsHitTestVisible$
9 True��5�   )   $��Center=�5�   )   $��Center=�5�   )   ��5�   )   �� ��5�   )   �� *%#DataContext.ShowObservationsCommand+: �� RelativeSource: �� ; �� AncestorType; UserControl��5�   &   �� 5�   *   "( ��60     RoundedButton< �� BasedOn#< [ 6D   .�5�   .   �� ) � y�$* 	Collapsed=�6K   . 5�   .   �� 5�   2   = �� Binding= 6>   �� *Observation+> �� Value> 6^   Z� .a�5�   6   �� ) � y�$* 	Collapsed=�6S   5�   3   �� 5�   2   = 6>   �� *Observation+>  6^   .a�5�   6   �� ) � y�$* 	Collapsed=�6S   5�   3   �� 5�   2   = 6>   �� *Observation+#+ [ > True6�   .a�5�   6   �� ) � y�$* Visible=�6S   5�   3   5�   +   .��5�      	  $ Close=�6   $��20��5�   3   $��20��6>   $��Center=�5�   3   5�   #   5�      5�      0  *System.Windows.Controls.DataGridTextColumn  5�      $5 Nº OS��6)   $
6 Auto 68   $	7 100Q�6E   ?   
IsReadOnly$
? True��6m   1  +System.Windows.Controls.DataGridBoundColumn@   ElementStyle#@ [ 6   A   BindingA 6T   �� *Code+6     5�      $5 Cliente��6)   $6 * 6:   $	7 210Q�6D   $
? True��6r   #@ [ 6�   A 6S   �� *
PersonName+6�     5�      $5 Data de Chegada��6)   $
6 Auto 6B   $7 90Q�6O   $
? True��6�   #@ [ 6�   A 6]   �� *DateCreated+B �� StringFormat$B {0:dd/MM/yyyy}��6�     5      $
5 Tipo��6)   $
6 Auto 67   $	7 140Q�6D   $
? True��6s   #@ [ 6�   A 6S   �� *DisplayType+6�     5     $5 Status��6-   $
6 Auto 6=   $	7 165Q�6J   8 5     �� 5     .R�5  "   �� #��[ 6K   ��6,   �� *DisplayStatus+6K   5     C   CellEditingTemplateC 5     �� 5	     .R�5
  "   �� $��16(�5  +   $��Center=�5  +   $��Center=�5  +   $
��Auto��5  +   ��5
  +   �� *
StatusList+' 5  +   �� *Status+$ TwoWay=�#+ [ 5     5       5     $	5 Tag��6-   $
6 Auto 6:   8 5     �� 5     .R�5  "   �� $ 
Horizontal=�6-   $��Center=�6F   .+�5  &   �� ��63   �� *
MappedTags+��5  *   �� 5  .   .R�5  2   �� $ 5��5  5   $	 5,2q�5  5   $��2q�5  5   ��5  9   �� *Color+._�5  6   �� $��14(�5   4   ��5  @   �� *Name+��5!  4   �� *	TextColor+5"  3   5#  /   5%  '   5&  #   5'     5)     D � ContextMenuD 5,     �� 5-     
����������#E0E0E0$��1q�6K     5.     ��%8  'm�       �� 5/     "( m�6$   .�50  "   �� ) � ��$* 20,10q�6<   �� 51  "   ) � ��$* 14(�6=   �� 52  "   ) � ��$* 40��6;   �� 53  "   ) � ��
* ���� 6?   . 54  "   P� 55  &   / �@J�IsMouseOver$
0 True��6E   . �56  *   �� ) � ��* �#F5F5F56G   57  '   59     5:     .@�5;     m� -   5        $��Adicionar observação��5;  #   $��Segoe UI)�6^   ���	#FF0078D76t   m� 5<     -   5        $��
Remover OS��5<  #   $��Segoe UI)�6N   ���	#FF0078D76d   m� 5=     -	   5        $��Segoe UI)�5?  r   ���	#FF0078D7��5=  #   �� *'%PlacementTarget.SelectedItem.IsPaused+: �� *FindAncestor+"; ��#+ [ 5?  �   5@     5B     5C     � 5E  
   -
   5        E �Name$E LoadingOverlay��5H     $��2��5E     ���	#80000000$y�	Collapsed=�5G      5I     �� 5J     $��* �6    �� 5K     $
��Auto �6    �� 5L     $��* �6    .+�5O     � $��1��6    % !MaterialDesignCircularProgressBar#��[ 5P     F � Value$F 0Q�5Q     G � IsIndeterminate$
G True��5R     $��50��5S     $��50��5T     �� 5V     $��1��6   $��Carregando...��5W     
�������$��16(�5Y     $��0,60,0,0q�5Z     $��Center=�5[     5\     5]     5^     