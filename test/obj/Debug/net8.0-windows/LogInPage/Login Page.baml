   M S B A M L   `   `   `  ����?  ;test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:test.ViewModelstest.ViewModels  5      +clr-namespace:test.Helperstest.Helpers  5          test.LoginPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      ,vmclr-namespace:test.ViewModels     5      .helpersclr-namespace:test.Helpers     5        N� Title$  	LoginPage��5       � DataContext 5      $   test.ViewModels.LoginViewModel  5	   
   .��5      � ���	#FF1E1E1E.+�5   
   �� $��Center=�6   $��Center=�60   $	��300��6M   $��20q�6Y   .+�5      �� $��Login��6   $��28(�5      
�������$��Center=�5      $��0,0,0,20q�5      �� 5      $��Username��6   
�������$��5,0,0,2q�6?   �� 5      $��36��6   $��0,0,0,10q�5      $��16(�5      ���	#FF2E2E2E
�������$	��5,6q�5      ��5      �� *
Username+ �� UpdateSourceTrigger$ PropertyChanged=��� 5!      $��Password��6   
�������$��5,0,0,2q�6?   G� 5"      $   test.Helpers.PasswordBoxHelper   AttachBehavior$
 True��5#      $��36��5%      $��0,0,0,10q�5&      $��16(�5'      ���	#FF2E2E2E
�������$	��5,6q�5*         BoundPassword 5$      �� *
Password+ �� Mode$ TwoWay=�$ PropertyChanged=�5*      �� 5.      $��Login��6   $	��200��5/      $��40��50      ���	#FF007ACC
�������$��16(�53      $��	0,10,0,10q�54      ��55      �� *LoginCommand+�� 58      -   5         ��Name$ StatusTextBlock��58      $
��Bold"�5;      $��0,10,0,0q�5<      $
��Wrap=�5=      $��Center=�5>      ��59      �� *StatusMessage+��5:      �� *StatusColor+ J� Opacity 5?      �� *StatusOpacity+�� 5B      $��Center=�6   
�������$��0,20,0,0q�6H   �� 5C       �� Command 6   �� *NavigateToRegisterCommand+.�6J   Don't have an account?6b    �� 5D      -   5        $�Register here��5D      ���	#FF00AEEF$
��Bold"�5F      	 �� TextDecorations$	 	Underline{�5G      
 � Cursor$

 Hand��5H      5I      5J      5K      5L      5M      