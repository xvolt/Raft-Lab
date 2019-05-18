Ноды на AspNetCore. Конфиги нод в файлах Nodes.json. В конфигах указаны URL других нод и Url текущей ноды, а также id.

Запуск:
1) Запустить 3 ноды из папок R1,R2,R3 (NodeConsole.exe)
2) Запустить клиента из папки Client (ClientConsole.exe)
Клиент делает запрос на 'localhost:10001'
R1 - слушает 'localhost:10001'
R2 - слушает 'localhost:10002'
R3 - слушает 'localhost:10003'
В зависимости от порядка запуска лидер будет первый запущенный (или новый выбранный)

Есть две машины состояний. Одна просто складывает числа (NumeralStateMachine), вторая Dictionary<string,string>.
R1,R2,R3 скомпилированы под NumeralStateMachine.
В проекте есть тесты на Dictionary.