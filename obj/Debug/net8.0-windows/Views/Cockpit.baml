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
��16,0q�5-      $��0,0,0,0q�5.      .��50      ��  �� Orientation$ 
Horizontal=�6   .+�51      '  !MaterialDesignThemes.Wpf.PackIcon  	   Kind$	 	ArrowLeft=�6#   $��24��52       $��24��6+   $��Center=�53       �� 54      $��Voltar��6    $��8,0,0,0q�55      $��Center=�56      57      58      �� 5:      $��1��6   $��Cockpit��5;      $��35(�5<      $��Center=�5=       $  MaterialDesignHeadline6TextBlock#��[ 5>      $��Center=�5?      �� 5A      $��2��6   $��80��6(   5B      �� 5D   
   $��1��6   ���	#FF0078D7$��40��69   $��0,4,0,0q�6E   .+�5E      � .+�5F      �� $��Ordens de Serviço��6   $��25(�5G      $
��Bold"�5H      $��Segoe UI)�5I      
�������
 �� Padding$
 0 2 0 0q�5K      $��Center=�5L      5M      5N      � 5P   
   $��0 5 0 0q�6   $��2��6    ���#FFFFFF 5Q      �� 5R      $��* �6#   .+�5U      � -   5         �Name$ ObservationPopUp��5U       � 	StaysOpen$
 True��5W       � AllowsTransparency$
 True��5X      $��Bottom=�5Y      $��Slide=�5Z       � HorizontalOffset$ 10��5[       � VerticalOffset$ 5��5\      ��5V      �� *IsPopupOpen+ �� Mode$ TwoWay=�.��5]      ��  �� CornerRadius$ 10��6   #���  5^       	 MaterialDesignerDivider#��� 	 5_      $��1q�5`       �� Padding$ 10q�5a       
 MaterialDesignShadowDepth2 J� Effect# [
 5b      ._�5c      � $	��300��6   $	��150��6'    5d      �� 5e      $
��Auto �6,   �� 5f      $��* �6,   �� 5g      $
��Auto �6,   .+�5j      �� $��0��6$   $��Observação��5k   $   $��18(�5l   $   $��SemiBold"�5m   $     MaterialDesignBody#���  5n   $   $��0 0 0 5q�5o   $   �� 5q      -   5         ��Name$ 	PopupText��5q   $   $��1��5r   $   #���  5t   $   $��14(�5u   $   $��
0, 0, 0, 5q�5v   $   $
��Wrap=�5w   $    �� TextAlignment$
 Left=�5x   $   ��5s   $   �� *Observation+ �� FallbackValue$  ��5x   $   �� 5z      -   5        $��2��5z   !   #��[ 5{   !   $��Fechar��5|   !   $��Right=�5}   !   5~   !   5      5�      5�      &	   System.Windows.Controls.DataGrid	  5�      -   5         	 Name$ DataGridSales��5�      $��0��5�       	  AutoGenerateColumns$ False��5�        MaterialDesignDataGrid#���  5�        MaterialDesignBackground 	  RowBackground# �  5�       	  AlternatingRowBackground �#F5F5F5 	  CanUserReorderColumns$ False��5�       	  CanUserSortColumns$ False��5�       	  CanUserResizeColumns$ False��5�       	  CanUserResizeRows$ False��5�       	  CanUserAddRows$ False��5�        J� 	AllowDrop$
  True��5�      $��	0 10 0 10q�5�      $��Stretch=�5�      $��Stretch=�5�      $
��Auto=�5�      $
��Auto=�5�      ! %� IsVirtualizing$! False��5�      
���   �" J� RenderTransformOrigin$" 0.5,0.5&�5�      ��5�      �� *	SalesData+# �� SelectedItem# 5�      �� *SelectedSale+  5�        DefaultCellStyle$ �� 
TargetType% �� Property& �� Value=
  7System.Windows.Controls.Primitives.DataGridColumnHeader*  $System.Windows.Controls.DataGridCell' �� 	Converter( �� ConverterParameter��%�  &	       '
  �     '  �    7   7   �� 5�      "$ ��65   .�5�      �� % � ��$& 16(�65   �� 5�      % � ��$
& Wrap=�69   �� 5�      % � ��$& Center=�6@   �� 5�      % � ��$& Center=�6>   5�      �� 5�      "$ 
 6   .�5�      �� % � ��$& 18(�65   �� 5�      % � ��$& SemiBold"�67   �� 5�      % � ��& �	#FF0078D767   �� 5�      % � ��$& Center=�6G   �� 5�      % � ��$& 10q�64   5�      �� 5�      "$  6   .�5�      �� % � ��& 67   �� *.+8'   $( 
Background���� 5�      % � ��& 67   �� *.+8'  $( 
Foreground���� 5�      % � ��$& 10q�64   �� 5�      % � ��$& 16(�65   5�      5�      ) 	  RowStyle) 5�      �� 5�      )  #System.Windows.Controls.DataGridRow"$  6   .�5�      �� % � ��$
& Auto��63   �� 5�      % � ��$& 50��66   * �� Triggers* 5�      P� 5�      + P� Property+ �@J�IsMouseOver, P� Value$
, True��6=   . �5�   "   �� % � ��& �#F5F5F56?   5�      P� 5�      + �@ 
IsSelected$
, True��6<   . �5�   "   �� % � ��& �#E3F2FD6?   5�      ]� 5�      - ]� 
Conditions- 5�   "   �� 5�   &   . �� Property. �@ 
IsSelected/ �� Value$
/ True��6F   �� 5�   &   . �@J�IsMouseOver$
/ True��6G   ./�5�   "   �� % � ��& �#BBDEFB6?   5�      5�      0 	  Columns0 5�      4  .System.Windows.Controls.DataGridTemplateColumn  5�      ,  &System.Windows.Controls.DataGridColumn1   Header$	1 Obs��6-   2   Width5  /System.Windows.Controls.DataGridLengthConverter$2 60 6:   3   MinWidth$3 60Q�6E   4   CellTemplate4 5�      �� 5�      .R�5�   "   �� 5 ��Name$5 ObservationButton��6)   $��30��5�   )   $	��5 0q�5�   )   6 J� IsHitTestVisible$
6 True��5�   )   $��Center=�5�   )   $��Center=�5�   )   ��5�   )   �� ��5�   )   �� *%#DataContext.ShowObservationsCommand+7 �� RelativeSource7 �� 8 �� AncestorType8 UserControl��5�   &   �� 5�   *   "$ ��60     RoundedButton9 �� BasedOn#9 [ 6D   .�5�   .   �� % � y�$& 	Collapsed=�6K   * 5�   .   �� 5�   2   : �� Binding: 6>   �� *Observation+; �� Value; 6^   Z� .a�5�   6   �� % � y�$& 	Collapsed=�6S   5�   3   �� 5�   2   : 6>   �� *Observation+;  6^   .a�5�   6   �� % � y�$& 	Collapsed=�6S   5�   3   �� 5�   2   : 6>   �� *Observation+#' [ ; True6�   .a�5�   6   �� % � y�$& Visible=�6S   5�   3   5�   +   .��5�        $	 Close=�6   $��20��5�   3   $��20��6>   $��Center=�5�   3   5�   #   5�      5�      0  *System.Windows.Controls.DataGridTextColumn  5�      $1 Nº OS��6)   $
2 Auto 68   $	3 100Q�6E   <   
IsReadOnly$
< True��6m   1  +System.Windows.Controls.DataGridBoundColumn=   ElementStyle#= [ 6   >   Binding> 6T   �� *Code+6     5�      $1 Cliente��6)   $
2 Auto 6:   $	3 210Q�6G   $
< True��6u   #= [ 6�   > 6V   �� *
PersonName+6�     5�      $1 Data de Chegada��6)   $2 * 6B   $3 90Q�6L   $
< True��6�   #= [ 6�   > 6Z   �� *DateCreated+? �� StringFormat$? {0:dd/MM/yyyy}��6�     5�      $
1 Tipo��6)   $2 * 67   $	3 140Q�6A   $
< True��6p   #= [ 6�   > 6P   �� *DisplayType+6�     5�      $1 Status��6-   $2 * 6=   $	3 165Q�6G   4 5�      �� 5�      .R�5   "   �� #��[ 6K   ��6,   �� *DisplayStatus+6K   5     @   CellEditingTemplate@ 5     �� 5     .R�5  "   �� -   5        $��16(�5  +   $��Center=�5  +   $��Center=�5  +   $
��Auto��5	  +   ��5  +   �� *
StatusList+# 5
  +   �� *Status+$ TwoWay=�#' [ 5  +   5     5       5     $	1 Tag��6-   $2 * 6:   4 5     �� 5     .R�5  "   �� $ 
Horizontal=�6-   $��Center=�6F   .+�5  &   �� ��63   �� *
MappedTags+��5  *   �� 5  .   .R�5  2   �� $ 5��5  5   $	 5,2q�5  5   $��2q�5  5   ��5  9   �� *Color+._�5  6   �� $��14(�5  4   ��5  @   �� *Name+��5  4   �� *	TextColor+5  3   5  /   5!  '   5"  #   5#     5%     A � ContextMenuA 5(     �� 5)     
����������#E0E0E0$��1q�6K     5*     ��%8  'm�       �� 5+     "$ m�6$   .�5,  "   �� % � ��$& 20,10q�6<   �� 5-  "   % � ��$& 14(�6=   �� 5.  "   % � ��$& 40��6;   �� 5/  "   % � ��
& ���� 6?   * 50  "   P� 51  &   + �@J�IsMouseOver$
, True��6E   . �52  *   �� % � ��& �#F5F5F56G   53  '   55     56     .@�57     m� -   5        $��Adicionar observação��57  #   $��Segoe UI)�6^   ���	#FF0078D76t   m� 58     -   5        $��
Remover OS��58  #   $��Segoe UI)�6N   ���	#FF0078D76d   m� 59     -	   5        $��Segoe UI)�5;  r   ���	#FF0078D7��59  #   �� *'%PlacementTarget.SelectedItem.IsPaused+7 �� *FindAncestor+"8 ��#' [ 5;  �   5<     5>     5?     � 5A  
   -
   5        B �Name$B LoadingOverlay��5D     $��2��5A     ���	#80000000$y�	Collapsed=�5C      5E     �� 5F     $��* �6    �� 5G     $
��Auto �6    �� 5H     $��* �6    .+�5K     � $��1��6    % !MaterialDesignCircularProgressBar#��[ 5L     C � Value$C 0Q�5M     D � IsIndeterminate$
D True��5N     $��50��5O     $��50��5P     �� 5R     $��1��6   $��Carregando...��5S     
�������$��16(�5U     $��0,60,0,0q�5V     $��Center=�5W     5X     5Y     5Z     