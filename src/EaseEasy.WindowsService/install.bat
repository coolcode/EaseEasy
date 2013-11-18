@echo 安装服务
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil.exe EaseEasy.WindowsService.exe
@echo 启动服务
net start "EaseEasy.WindowsService"
pause
@echo off