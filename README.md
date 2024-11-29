Инструкция по развёртыванию.

1. Клонировать репозиторий.
2. Открыть PracticeFinal.sln в Visual Studio 2022.
3. В Package Manager Console последовательно ввести следующие команды:
```
Add-Migration InitialCreate
```
```
Update-Database
```
4. Если не настроен запуск нескольких проектов, настроить, как указано [здесь](https://learn.microsoft.com/en-us/visualstudio/ide/how-to-set-multiple-startup-projects?view=vs-2022), так, чтоб первым запускался PracticeFinal.WebAPI, потом PracticeFinal.Web.
5. Запустить (F5).
