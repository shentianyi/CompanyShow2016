   M S B A M L   `   `   `  ����E  ATcpDemoWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:TcpDemoWPF
TcpDemoWPF  5   	       TcpDemoWPF.ClientWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:TcpDemoWPF     5   	   -   5          � Title$  
MainWindow��5   	   $	��550��6   $	��525��6)   .��5	      � $��
0,0,-660,0q�6   .+�5
   
   �� -   5         ��Name$ MakeConnect��5
      $��连接服务器��6&   $
��Left=�66   $��
161,66,0,0q�6Q   $	��Top=�6e   $��75��6}   $��28��6�   �� 5   
   -   5         ��Name$ ReceiveMessageText��5      $
��Left=�6.   $��94��6I   $��
10,253,0,0q�6U    �� TextWrapping$
 Wrap=�6i   $	��Top=�6}   $	��450��6�   6�   �� 5   
   -   5         ��Name$ ReceiveMessage��5      $
��Left=�6,   $��
10,232,0,0q�6G   $
��Wrap=�6[   $��接收到的消息��6o   $	��Top=�6}   �� 5   
   -   5        $ SendMessage��5      $
��Left=�6)   $��
10,362,0,0q�6D   $
��Wrap=�6X   $��发送的消息��6l   $	��Top=�6y   �� 5   
   -   5        $ SendMessageText��5      $
��Left=�6+   $	��118��6F   $��
10,383,0,0q�6S   $
 Wrap=�6g   $	��Top=�6{   $	��450��6�   6�   �� 5   
   -   5         ��Name$ label��5      $��服务器IP:��6   $
��Left=�60   $��	10,10,0,0q�6K   $	��Top=�6^   �� 5   
   -   5        $ 
serverIPTB��5      $
��Left=�6&   $��26��6A   $��	72,10,0,0q�6M   $
 Wrap=�6`   $��192.168.1.111��6t   $	��Top=�6�   $	��164��6�   �� 5   
   -	   5        $ label1��5      $��端口:��6    $
��Left=�6.   $��
250,10,0,0q�6I   $	��Top=�6]   �� 5   
   -
   5        $ serverPortTB��5      $
��Left=�6(   $��23��6C   $��
301,12,0,0q�6O   $
 Wrap=�6c   $
��4001��6w   $	��Top=�6�   $	��178��6�   �� 5   
   -   5        $ Stabilitybutton��5      $��稳定性测试��6*   $
��Left=�6:   $��161,120,0,0q�6U   $	��Top=�6j   $��75��6�   6�   5      5      