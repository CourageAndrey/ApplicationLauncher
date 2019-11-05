# ApplicationLauncher
Simple application wrapper, which allows automatic updates support

How to use:
1. Copy ApplicationLauncher.exe into your application folder.
2. Create LauncherConfig.xml configuration file there (see details below).
3. Create application, which will run ServiceHost with implementation of IUpdateService interface (see implementation details below).
4. Please, run ApplicationLauncher.exe instead of your application (for exaple, use shortcut settings).

Configuration (example):
<LauncherConfig xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <UpdateServerUrl>http://mycompany.com:1234/updateservice</UpdateServerUrl>
  <ApplicationName>MyApplicationName</ApplicationName>
  <ApplicationStartupPath>MyApplication.exe</ApplicationStartupPath>
  <ApplicationVersion>1.0</ApplicationVersion>
  <CheckUpdateStrategy>Automatically</CheckUpdateStrategy><!-- or AskBeforeInstall or Never-->
</LauncherConfig>

Implementation detail:
IUpdateService interface has 2 methods. The firt one is used in order to determine, if application has to be updated or not.
The second one downloads update itself. It is possible to implement update service. which can update any version of your app to any other version.
However, it is still possible to implement the simpliest mechanism, which can only return the latest application version (like unit tests demonstrate).
"ApplicationName" setting can be used in order to implement update service, which can update many different applications of your company.

If you have any questnions, please, check project XML-comments and unit tests first. It is required to run VisualStudion as admin in order to run tests,
because test update service runs it own ServiceHost. If there are still questions, please, feel free to mail bublic-xiii@mail.ru.
Have fun!