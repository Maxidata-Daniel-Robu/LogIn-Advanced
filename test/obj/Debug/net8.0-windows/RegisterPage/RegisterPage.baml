   M S B A M L   `   `   `  ����?  ;test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:test.ViewModelstest.ViewModels  5          test.RegisterPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      ,vmclr-namespace:test.ViewModels     5        N� Title$  RegisterPage��5       � DataContext 5      '   !test.ViewModels.RegisterViewModel  5   
   .��5      � ���	#FF1E1E1E.+�5   
   �� $��Center=�6   $��Center=�60   $	��300��6M   $��20q�6Y   .+�5      �� $��Register��6   $��28(�5      
�������$��Center=�5      $��0,0,0,20q�5      �� 5      $��Username��6   
�������$��5,0,0,2q�6?   �� 5      $��36��6   $��0,0,0,10q�5      $��16(�5      ���	#FF2E2E2E
�������$	��5,6q�5      ��5      �� *
Username+ �� UpdateSourceTrigger$ PropertyChanged=��� 5      $��Password��6   
�������$��5,0,0,2q�6?   G� 5      -   5         G�Name$ PasswordBox��5      $��36��5       $��0,0,0,10q�5!      $��16(�5"      ���	#FF2E2E2E
�������$	��5,6q�5%      5&      �� 5)      $��Confirm Password��6   
�������$��5,0,0,2q�6G   G� 5*      -   5        $ ConfirmPasswordBox��5*      $��36��5+      $��0,0,0,10q�5,      $��16(�5-      ���	#FF2E2E2E
�������$	��5,6q�50       J� RenderTransformOrigin$ 0.5,0.5&�51   O    G� Password$  ��6o   z�52      V� 53      .~�54      �� �� 55      �� 56       �� Angle$ -360.115Q�6*   T� 57      58      5:      �� 5=      $��Register��6   $	��200��5>      $��40��5?      ���	#FF28A745
�������$��16(�5B      $��	0,10,0,10q�5C      ��5D      �� *RegisterCommand+�� 5G      $
��Bold"�5I      $��0,10,0,0q�5J      $
��Wrap=�5K      $��Center=�5L      ��5G      �� *StatusMessage+��5H      �� *StatusColor+5L      �� 5O      $��Center=�6   
�������$��0,20,0,0q�6H   �� 5P       �� Command 6   �� *NavigateToLoginCommand+.�6G    Already have an account? Login5R      5S      5T      5U      5V      