   M S B A M L   `   `   `  ����E  ATcpDemoWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:TcpDemoWPF
TcpDemoWPF  5   	       TcpDemoWPF.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:TcpDemoWPF     5   	   -   5          � Title$  
MainWindow��5   	   $	��553��6   $	��757��6)   .��5	      � .+�5
   
   �� -   5         ��Name$ MakeConnection��5
      $��Open Tcp Server��6)   $
��Left=�6C   $��
446,24,0,0q�6^   $	��Top=�6r   $	��123��6�   $��50��6�   �� 5   
   -   5         ��Name$ Receive��5      $
��Left=�6%   $��213,101,0,0q�6@   $
��Wrap=�6U   $��接受到的消息：��6i   $	��Top=�6x   �� 5   
   -   5        $
 Send��5      $
��Left=�6"   $��213,232,0,0q�6=   $
��Wrap=�6R   $��发送的消息：��6f   $	��Top=�6t   �� 5   
   -   5         ��Name$ SendText��5      $
��Left=�6$   $	��206��6?   $��213,253,0,0q�6L    �� TextWrapping$
 Wrap=�6a   $	��Top=�6u   $	��509��6�    J� RenderTransformOrigin$ 0.5,0.5&�6�   �� 5   
   -   5        $ ReceiveText��5      $
��Left=�6'   $��85��6B   $��213,130,0,0q�6N   $
 Wrap=�6c   $	��Top=�6w   $	��526��6�   �� 5   
   -   5        $ button��5      $��Client Window��6!   $
��Left=�69   $��
591,24,0,0q�6T   $	��Top=�6h   $	��131��6�   $��50��6�   6�   �� 5   
   -   5         ��Name$ label��5      $��客户端列表：��6   $
��Left=�60   $��	20,96,0,0q�6K   $	��Top=�6^   $��29��6v   $	��123��6�   �� 5   
   -	   5         ��Name$ clientLB��5      $
��Left=�6$   $	��351��6?   $��
20,130,0,0q�6L   $	��Top=�6`   $	��140��6x   �� 5   
   -
   5        $ label1��5      $	��IP:��6    $
��Left=�6.   $��	29,24,0,0q�6I   $	��Top=�6\   $��31��6t   $��42��6�   �� 5   
   -   5        $ 
serverIPTB��5      $
��Left=�6&   $��29��6A   $��	71,26,0,0q�6M   $
 Wrap=�6`   $	��Top=�6t   $	��155��6�   $��192.168.1.113��6�   6�   �� 5   
   -   5        $ label2��5      $��端口:��6    $
��Left=�6.   $��
231,26,0,0q�6I   $	��Top=�6]   �� 5   
   -   5        $ serverPortTB��5      $
��Left=�6(   $��31��6C   $��
281,25,0,0q�6O   $
 Wrap=�6c   $
��4000��6w   $	��Top=�6�   $	��128��6�   5      5      