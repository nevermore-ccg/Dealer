# ПО "Dealer"
## Важная информация :pushpin:
Проект был разработан в качестве выпускной квалифицированной работы во время практики в АО "ОНИИП". <br> Все данные в базе данных DealerDB являются вымышленными с целью показать как работает программа на их примере.
## Описание
Данная система предназначена для создания и управления заказами на предприятии научно-производственного комплекса, специализирующегося на разработке и производстве радиоэлектронной продукции. <br>
Практическая значимость разработанного приложения заключается в автоматизации процессов связанных с заказами.
## Необходимые действия для работоспособности проекта на другом ПК:
1. Необходимо создать БД в SQL Server Management Studio из скрипта по пути: Dealer/DataBaseScript.sql
2. Дальше необходимо изменить, следуя инструкциям в комментарии к коду, соединение sqlconnection по пути: Dealer/DataBaseFolder/DataBase.cs
3. После стоит таким же образом изменить соединение DealerDBEntities по пути: Dealer/App.config
___
## ER-диаграмма

![devenv_OpnEKtQRZx](https://github.com/nevermore-ccg/Dealer/assets/84433601/84cc8567-b88d-4216-a730-05b5d4019665)

   
## Скриншоты

### Общий функционал

Авторизация 

![image](https://github.com/nevermore-ccg/Dealer/assets/84433601/61cc7046-2520-45de-b2c0-9fced4515538)

### Функционал доступный дилеру

1. Просмотр характеристики доступной продукции и составление заказа

![SOoHljpFVn](https://github.com/nevermore-ccg/Dealer/assets/84433601/a337e0aa-07f2-4e2b-aca7-236d22285b24)

2. Просмотр всех заказов по статусу с возможностью удалить новый заказ или отменить уже действующий

![Uo9sew2aHw](https://github.com/nevermore-ccg/Dealer/assets/84433601/9c06ca0d-9ec8-4800-8f46-d2465bc74650)

### Функционал доступный администратору

1. Навигация через главное меню администратора

![JJeTT7KvPV](https://github.com/nevermore-ccg/Dealer/assets/84433601/9f0dc78e-89ca-4a2e-a124-7a0c68c170cc)

2. Администрирование базы данных

![K1Xk1QiKUQ](https://github.com/nevermore-ccg/Dealer/assets/84433601/4b05888f-10a5-4ae5-8e6d-d7ee753b8af0)

3. Регистрация новых пользователей

![QKfyVOFrOX](https://github.com/nevermore-ccg/Dealer/assets/84433601/be591dc8-ebd5-45f7-add8-ee37c8dbc6ce)

4. Добавление новой продукции

![u6FdnnmC1a](https://github.com/nevermore-ccg/Dealer/assets/84433601/c5a7df3d-fe1b-4d63-8dbb-333aa5144176)

5. Управление заказами

![LphoTwawbX](https://github.com/nevermore-ccg/Dealer/assets/84433601/372f6021-ded3-4753-895a-e8c9f0872f10)

