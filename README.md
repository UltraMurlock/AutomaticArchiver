# AutomaticArchiver
Программа для автоматического создания zip-архивов с бэкапами папок и файлов.
### Принцип работы
Работа программы основана на выполнении задач архивации, описанных пользователем в файле `tasklist.json`. При каждом запуске программа проходит по всем задачам и создаёт новые архивы, которые помещаются в указанные целевые директории и именуются с использованием имени, указанного в задаче, и текущей даты.

Программа задумывалась для работы при включении ОС (для этого её нужно поместить в автозагрузку), но может запускаться вручную/иными способами.
### Задачи
Задачи определяются в файле `tasklist.json`, при этом делятся на два типа:
- `DirectoryTasks` - архивация директории целиком
- `FileTasks` - архивация одиночного файла

У задач есть следующие параметры:
- `SourceDirectory` - папка, файлы из которой будут архивироваться
- `IncludeSourceDirectory` (только `DirectoryTask`) - определяет, нужно ли помещать саму папку `SourceDirectory` в архив (`true`) или нужно архивировать только файлы/папки, лежащие в `SourceDirectory` (`false`)
- `SourceFileName` (только `FileTask`) - файла в папке `SourceDirectory`, который нужно архивировать
- `TargetDirectory` - папка, в которую будут помещаться архивы
- `TargetName` - имя, использующееся в названии архива `<TargetName>_<текущая-дата>.zip`
- `IgnorePattern` - названия паттерна, по которому будут игнорироваться файлы (подробнее ниже)

Пример оформления задач можно посмотреть в файле `tasklist-template.json`

### Паттерны игнорирования
Паттерны игнорирования - это указания архиватору, какие файлы нужно пропустить при бэкапировании. Паттерны определяются в файле `ignore-patterns.json`. В файле может быть несколько паттернов, а у каждого из них есть два параметра:
- `Name` - название паттерна, которое нужно указывать в поле `IgnorePattern` задачи, чтобы использовать данный паттерн для неё
- `StringPatterns` - строковые паттерны, по которым будет проводиться проверка файлов и папок.
Они могут быть двух типов:
  - Паттерны для директорий. Если строка оканчивается на `/`, то программа распознаёт этот паттерн, как паттерн названия директории
  - Паттерны для файлов. Все остальные строковые паттерны будут рассмотрены как паттерны названия файлов

Пример оформления паттернов игнорирования можно посмотреть в файле `ignore-patterns-template.json`

#### Принцип работы паттернов
Принцип работы паттернов игнорирования заключается в создании из строковых паттернов регулярных выражений и проверке названий файлов и папок на соответствие хотя бы одному из паттернов. Если есть совпадения, то файл/директория игнорируется. Таким образом, в паттернах игнорирования можно использовать все возможности регулярных выражений.

### Очистка старых архивов
Для того, чтобы со временем не накапливалось слишком большое количество архивов, в программе реализована очистка старых. Во время очистки удаляются все архивы за месяц, кроме самого последнего.

Таким образом, в папке с архивами будут архивы за каждый день текущего месяца и по одному на каждый прошедший месяц.
