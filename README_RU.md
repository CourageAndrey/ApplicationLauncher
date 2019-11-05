# ApplicationLauncher
Простая оболочка для приложений, добавляющая поддержку автоматических обновлений

Как использовать:
1. Положить в папку со своим приложением ApplicationLauncher.exe.
2. Положить рядом файл конфигурации LauncherConfig.xml (смотри описание настроек ниже).
3. Написать приложение, в котором запустить ServiceHost со своей реализацией интерфейса IUpdateService (смотри описание ниже).
4. Вместо своего приложения запускать ApplicationLauncher.exe (например, в настройках ярлыка программы).

Конфигурация (типичная):
<LauncherConfig xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <UpdateServerUrl>http://mycompany.com:1234/updateservice</UpdateServerUrl>
  <ApplicationName>MyApplicationName</ApplicationName>
  <ApplicationStartupPath>MyApplication.exe</ApplicationStartupPath>
  <ApplicationVersion>1.0</ApplicationVersion>
  <CheckUpdateStrategy>Automatically</CheckUpdateStrategy><!-- or AskBeforeInstall or Never-->
</LauncherConfig>

Особенности реализации:
Интерфейс IUpdateService имеет 2 метода, один из которых отвечает на вопрос "А обновилась ли программа со времени своего последнего запуска?",
а второй - собственно скачивает апдейт. Контракт сделан так, чтобы можно было написать сервис, который обновляет приложение с любой версии на любую другую.
Но никто не мешает реализовать простейший автоответчик, всегда выдающий последнюю версию приложения (как сделано в тестах). Настройка "имя приложения"
позволяет реализовать сервис для обновления нескольких приложений компании.

При возникновении вопросов - смотри в XML-комментарии и функциональные тесты к проекту. Для их запуска требуется запустить VisualStudio с правами администратора,
так как один из компонентов тестов запускает свой ServiceHost. Если не помогает - можно написать автору на bublic-xiii@mail.ru.
Приятного использования!