   M S B A M L   `   `   `  ����C  ?NeuroApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:NeuroApp.ViewsNeuroApp.Views  5   	   +    %NeuroApp.Views.GenerateProtocolDialog   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   b ^MaterialDesignThemes.Wpf, Version=4.1.0.2556, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	   T PMahApps.Metro, Version=2.0.0.0, Culture=neutral, PublicKeyToken=51482d6f650b2b3f;Controls,http://metro.mahapps.com/winfx/xaml/controls  5   	   (localclr-namespace:NeuroApp.Views  5   	     � WindowStyle$
  None=�5
   	    � AllowsTransparency$
 True��5   	    � Title$ GenerateProtocolDialog��5   	   $	��480��6(   $	��390��65   ��5   	   Z� .��5      #  MaterialDesignThemes.Wpf.Card  -   5           UniformCornerRadius$ 15Q�5         MaterialDesignPaper#���   63   $��25q�5      +  %MaterialDesignThemes.Wpf.ShadowAssist   ShadowDepth$ Depth4=�6U   .��5   
   )  #MaterialDesignThemes.Wpf.DialogHost  -   5          Name$ 
DialogHost��5   =      CloseOnClickAway$ False��6$   .��5      � .+�5      �� .+�5      ]� $	��Top=�6    .+�5      �� $��Protocolo de atendimento��6$   $��DemiBold"�5   $   $��18(�5   $   $
��Left=�5   $    $  MaterialDesignHeadline6TextBlock#��[ 5   $   $��	15 20 0 0q�5   $   �� 5      -   5         ��Name$ BtnExit��5   O   $��Right=�5   !   $��	0 10 15 0q�6=   $��35��6P   $��35��6\     RoundedButton#��[ 6g   +  %MaterialDesignThemes.Wpf.ButtonAssist   CornerRadius$ 50��5   !   .��5      '  !MaterialDesignThemes.Wpf.PackIcon  	   Kind$	 Close=�66   $��Center=�6C   $��Center=�6`   5      5       �� 5"      -   5        
 ��Name$
 CustomerTextBox��5"      $��40��5#      $	��260��5$      $��10q�5%      )  #MaterialDesignThemes.Wpf.HintAssist   Hint$ 	Cliente *��5&      $��
25 20 25 5q�5'       ! MaterialDesignOutlinedTextBox#��[ 5(      �� 5*      -   5        $
 SerialNumberTextBox��5*      $��40��5+      $	��260��5,      $��10q�5-      $ Nº de Série *��5.      $
��25 5q�5/      #��[ 50      �� 52      -   5         ��Name$ DeviceTextBox��52      $��40��53      $	��260��54      $��10q�55      $ 
Aparelho *��56       " MaterialDesignOutlinedComboBox#��[ 57      $
��25 5q�58      .@�59      �� $��Video Frenzel��6'   �� 5:      $��VD 360��6'   �� 5;      $��Sivec Plus Vx��6'   �� 5<      $��Sivec Plus Ex��6'   �� 5=      $��OTO VM��6'   �� 5>      $��OAT 10 Plus��6'   �� 5?      $��OTO New��6'   �� 5@      $��OTO 3D��6'   �� 5A      $��Vecwin Plus��6'   �� 5B      $
��Veng��6'   �� 5C      $��
Vecwin STM��6'   �� 5D      $��Vecto VM��6'   5E      �� 5G      -   5        $
 TitleTextBox��5G      $��40��5H      $	��260��5I      $��10q�5J      $ 	Título *��5K      $
��25 5q�5L      #��[ 5M      �� 5O      -   5        $
 DescriptionTextBox��5O      $��50��5P      $	��260��5Q      $��10q�5R       �� AcceptsReturn$
 True��5S       �� TextWrapping$
 Wrap=�5T       �� MinLines$ 5��5U      $ Descrição *��5V      $
��25 5q�5W      #��[ 5X      �� 5Z       �� Orientation$ 
Horizontal=�6!   $��25 8 0 0q�6:   .+�5[      �� -	   5        $
 CodeTextBox��5[   &   $��15 0 5 0q�5\   &   $��40��5]   &   $	��220��5^   &   $��10q�5_   &   ${�False��5`   &   ���#2196F3$ Nº de protocolo��5b   &   #��[ 5c   &   �� 5e      -
   5        $  GenerateProtocolCodeButton��5e   %   $��0q�5f   %   $��35��5g   %   $��35��5h   %   ���#2196F3  MahApps.Styles.Button#��[ 5k   %   .��5m   "     $	 	Autorenew=�6:   
�������5n   :   5p      5q      �� 5s      -   5        $ SaveProtocolButton��5s      $��10 15 10 10q�5t      $��Salvar��5u      $	��150��5v      6)   5w      5x      5y      5z      5{      