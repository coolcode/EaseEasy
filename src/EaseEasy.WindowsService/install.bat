@echo ��װ����
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil.exe EaseEasy.WindowsService.exe
@echo ��������
net start "EaseEasy.WindowsService"
pause
@echo off