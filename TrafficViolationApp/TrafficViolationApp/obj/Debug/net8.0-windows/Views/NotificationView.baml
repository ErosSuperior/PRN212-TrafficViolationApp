   M S B A M L   `   `   `  ����N  JTrafficViolationApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullO,clr-namespace:TrafficViolationApp.ConvertersTrafficViolationApp.Converters  5      0    *TrafficViolationApp.Views.NotificationView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      C
converters,clr-namespace:TrafficViolationApp.Converters     5        � 	Resources  5      :   4TrafficViolationApp.Converters.BoolToStringConverter   BoolToStringConverter��%   &	          5   
   6+   5      .��5      � $��20q�6   .+�5	   
   �� .+�5
      �� $��16(�5      $
��Bold"�6&   $��0,0,0,10q�68   ��5
      �� *UnreadNotificationCount+ �� StringFormat$# Thông báo chưa đọc: {0}��5   8   �� 5      $��Thông báo��6   $��14(�6)   $
��Bold"�67   $��0,10,0,5q�6I   �� 5      $��0,0,0,10q�6=   $	��300��6O   ��6   �� *Notifications+ �� View 5      �� 5      .P�5      �� $��
Nội dung��6)    �� Width$	 300��6d    �� DisplayMemberBinding 6;   �� *	Message+6d   �� 5      $��Biển số��6)   $	 100��6g    6:   �� *PlateNumber+6g   �� 5      $��Ngày gửi��6)   $	 150��6�    6;   �� *
SentDate+$ {0:dd/MM/yyyy HH:mm}��6�   �� 5      $��Đã đọc��6)   $ 80��69    �� CellTemplate 5      �� 5   "   .R�5   &   �� $��Center=�6�   ��60   �� *IsRead+ �� 	Converter# [   �� ConverterParameter$ IsNotification��6�   5   #   5      5      5      �� 5      $��Đánh dấu đã đọc��6   $��0,10,0,5q�6U   $
��10,5q�5      ���#3498db
�������$��0q�6L   ��5   /   �� *MarkAsReadCommand+  5       �� 
TargetType	 �� Property
 �� Value��%^   '��       �� 5       " ��6   .�5!      �� 	 �@��CornerRadius$
 4��69   5"      5#      5$      �� 5%      $��
Quay lại��6   $��0,5,0,0q�6H   $
��10,5q�5&      ���#7f8c8d
�������$��0q�6L   ��5%   (   �� *BackCommand+  5'      ��%^   '��       �� 5(      " ��6   .�5)      �� 	 �@��CornerRadius$
 4��69   5*      5+      5,      5-      5.      5/      