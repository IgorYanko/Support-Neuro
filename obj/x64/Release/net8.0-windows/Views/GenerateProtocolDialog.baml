   M S B A M L   `   `   `  ���� C  ?NeuroApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:NeuroApp.ViewsNeuroApp.Views  +    %NeuroApp.Views.GenerateProtocolDialog   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     b ^MaterialDesignThemes.Wpf, Version=4.1.0.2556, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  T PMahApps.Metro, Version=2.0.0.0, Culture=neutral, PublicKeyToken=51482d6f650b2b3f;Controls,http://metro.mahapps.com/winfx/xaml/controls  (localclr-namespace:NeuroApp.Views    � WindowStyle$
  None=� � AllowsTransparency$
 True�� � Title$ GenerateProtocolDialog��$	��480��$	��390����Z� .��#  MaterialDesignThemes.Wpf.Card  -      UniformCornerRadius$ 15Q�   MaterialDesignPaper#���   $��25q�+  %MaterialDesignThemes.Wpf.ShadowAssist   ShadowDepth$ Depth4=�.��)  #MaterialDesignThemes.Wpf.DialogHost  -     Name$ 
DialogHost��   CloseOnClickAway$ False��.��� .+��� .+�]� $	��Top=�.+��� $��Protocolo de atendimento��$��DemiBold"�$��18(�$
��Left=� $  MaterialDesignHeadline6TextBlock#��[ $��	15 20 0 0q��� -    ��Name$ BtnExit��$��Right=�$��	0 10 15 0q�$��35��$��35��  RoundedButton#��[ +  %MaterialDesignThemes.Wpf.ButtonAssist   CornerRadius$ 50��.��'  !MaterialDesignThemes.Wpf.PackIcon  	   Kind$	 Close=�$��Center=�$��Center=��� -   
 ��Name$
 CustomerTextBox��$��40��$	��260��$��10q�)  #MaterialDesignThemes.Wpf.HintAssist   Hint$ 	Cliente *��$��
25 20 25 5q� ! MaterialDesignOutlinedTextBox#��[ �� -   $
 SerialNumberTextBox��$��40��$	��260��$��10q�$ Nº de Série *��$
��25 5q�#��[ �� -    ��Name$ DeviceTextBox��$��40��$	��260��$��10q�$ 
Aparelho *�� " MaterialDesignOutlinedComboBox#��[ $
��25 5q�.@��� $��Video Frenzel���� $��VD 360���� $��Sivec Plus Vx���� $��Sivec Plus Ex���� $��OTO VM���� $��OAT 10 Plus���� $��OTO New���� $��OTO 3D���� $��Vecwin Plus���� $
��Veng���� $��
Vecwin STM���� $��Vecto VM���� -   $
 TitleTextBox��$��40��$	��260��$��10q�$ 	Título *��$
��25 5q�#��[ �� -   $
 DescriptionTextBox��$��50��$	��260��$��10q� �� AcceptsReturn$
 True�� �� TextWrapping$
 Wrap=� �� MinLines$ 5��$ Descrição *��$
��25 5q�#��[ ��  �� Orientation$ 
Horizontal=�$��25 8 0 0q�.+��� -	   $
 CodeTextBox��$��15 0 5 0q�$��40��$	��220��$��10q�${�False�����#2196F3$ Nº de protocolo��#��[ �� -
   $  GenerateProtocolCodeButton��$��0q�$��35��$��35�����#2196F3  MahApps.Styles.Button#��[ .��  $	 	Autorenew=�
��������� -   $ SaveProtocolButton��$��10 15 10 10q�$��Salvar��$	��150��